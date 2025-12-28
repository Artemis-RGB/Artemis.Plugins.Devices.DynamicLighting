using System;
using Windows.Devices.Enumeration;
using Windows.Devices.Lights;
using Microsoft.UI.Xaml;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Artemis.DynamicLighting.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Artemis.DynamicLighting
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private DeviceWatcher? _deviceWatcher;
        private DeviceManager? _deviceManager;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            _deviceManager = new DeviceManager();
            Initialize();

            var builder = WebApplication.CreateBuilder(args.Arguments.Split(" "));
            
            // Add services to the container
            builder.Services.AddSingleton(_deviceManager);
            builder.Services.AddGrpc();
            builder.Services.AddLogging(loggingBuilder => loggingBuilder.SetMinimumLevel(LogLevel.Warning));

            builder.WebHost.ConfigureKestrel(serverOptions =>
            {
                serverOptions.ListenNamedPipe("Artemis.DynamicLighting", listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
            });

            var webApp = builder.Build();
            
            // Configure the HTTP request pipeline
            webApp.MapGrpcService<DynamicLightingService>();
            webApp.Run();
        }

        private void Initialize()
        {
            var selector = LampArray.GetDeviceSelector();

            _deviceWatcher = DeviceInformation.CreateWatcher(selector);
            _deviceWatcher.Added += OnDeviceAdded;
            _deviceWatcher.Removed += OnDeviceRemoved;
            
            _deviceWatcher.Start();
        }
        
        private void OnDeviceRemoved(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            Console.WriteLine($"Device removed: {args.Id}");
            _deviceManager?.RemoveDevice(args.Id);
        }

        private async void OnDeviceAdded(DeviceWatcher sender, DeviceInformation args)
        {
            Console.WriteLine($"Device added: {args.Name} (ID: {args.Id})");
            try
            {
                var lampArray = await LampArray.FromIdAsync(args.Id);
                _deviceManager?.AddDevice(args.Id, args, lampArray);
                Console.WriteLine($"Successfully added device: {lampArray.DeviceId} with {lampArray.LampCount} lamps");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to add device {args.Id}: {ex.Message}");
            }
        }
    }
}