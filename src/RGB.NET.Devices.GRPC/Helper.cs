﻿using RGB.NET.Core;

namespace RGB.NET.Devices.GRPC;

internal static class Helper
{
    public static LedId GetInitialLedIdForDeviceType(RGBDeviceType type)
        => type switch
        {
            RGBDeviceType.Mouse => LedId.Mouse1,
            RGBDeviceType.Headset => LedId.Headset1,
            RGBDeviceType.Mousepad => LedId.Mousepad1,
            RGBDeviceType.LedStripe => LedId.LedStripe1,
            RGBDeviceType.LedMatrix => LedId.LedMatrix1,
            RGBDeviceType.Mainboard => LedId.Mainboard1,
            RGBDeviceType.GraphicsCard => LedId.GraphicsCard1,
            RGBDeviceType.DRAM => LedId.DRAM1,
            RGBDeviceType.HeadsetStand => LedId.HeadsetStand1,
            RGBDeviceType.Keypad => LedId.Keypad1,
            RGBDeviceType.Fan => LedId.Fan1,
            RGBDeviceType.Speaker => LedId.Speaker1,
            RGBDeviceType.Cooler => LedId.Cooler1,
            RGBDeviceType.Keyboard => LedId.Keyboard_Custom1,
            _ => LedId.Custom1
        };

    public static LedId? GetLedIdForVirtualKey(int virtualKey)
    {
        return virtualKey switch
        {
            // Mouse buttons
            1 => LedId.Mouse1,
            2 => LedId.Mouse2,
            3 => LedId.Mouse3,
            4 => LedId.Mouse4,
            5 => LedId.Mouse5,
            6 => LedId.Mouse6,
            
            // Navigation and control keys
            8 => LedId.Keyboard_Backspace, // Back
            9 => LedId.Keyboard_Tab, // Tab
            12 => null, // Clear - TODO: no direct mapping
            13 => LedId.Keyboard_Enter, // Enter
            16 => LedId.Keyboard_LeftShift, // Shift (general)
            17 => LedId.Keyboard_LeftCtrl, // Control (general)
            18 => LedId.Keyboard_LeftAlt, // Menu (Alt)
            19 => LedId.Keyboard_PauseBreak, // Pause
            20 => LedId.Keyboard_CapsLock, // CapitalLock
            
            // Asian input method keys
            21 => null, // Hangul/Kana - TODO: no direct mapping
            23 => null, // Junja - TODO: no direct mapping
            24 => null, // Final - TODO: no direct mapping
            25 => null, // Hanja/Kanji - TODO: no direct mapping
            
            27 => LedId.Keyboard_Escape, // Escape
            28 => null, // Convert - TODO: no direct mapping
            29 => null, // NonConvert - TODO: no direct mapping
            30 => null, // Accept - TODO: no direct mapping
            31 => null, // ModeChange - TODO: no direct mapping
            
            // Navigation keys
            32 => LedId.Keyboard_Space, // Space
            33 => LedId.Keyboard_PageUp, // PageUp
            34 => LedId.Keyboard_PageDown, // PageDown
            35 => LedId.Keyboard_End, // End
            36 => LedId.Keyboard_Home, // Home
            37 => LedId.Keyboard_ArrowLeft, // Left
            38 => LedId.Keyboard_ArrowUp, // Up
            39 => LedId.Keyboard_ArrowRight, // Right
            40 => LedId.Keyboard_ArrowDown, // Down
            
            // Other control keys
            41 => null, // Select - TODO: no direct mapping
            42 => null, // Print - TODO: no direct mapping
            43 => null, // Execute - TODO: no direct mapping
            44 => LedId.Keyboard_PrintScreen, // Snapshot
            45 => LedId.Keyboard_Insert, // Insert
            46 => LedId.Keyboard_Delete, // Delete
            47 => null, // Help - TODO: no direct mapping
            
            // Number row (0-9)
            48 => LedId.Keyboard_0, // Number0
            49 => LedId.Keyboard_1, // Number1
            50 => LedId.Keyboard_2, // Number2
            51 => LedId.Keyboard_3, // Number3
            52 => LedId.Keyboard_4, // Number4
            53 => LedId.Keyboard_5, // Number5
            54 => LedId.Keyboard_6, // Number6
            55 => LedId.Keyboard_7, // Number7
            56 => LedId.Keyboard_8, // Number8
            57 => LedId.Keyboard_9, // Number9
            
            // Letters (A-Z)
            65 => LedId.Keyboard_A, // A
            66 => LedId.Keyboard_B, // B
            67 => LedId.Keyboard_C, // C
            68 => LedId.Keyboard_D, // D
            69 => LedId.Keyboard_E, // E
            70 => LedId.Keyboard_F, // F
            71 => LedId.Keyboard_G, // G
            72 => LedId.Keyboard_H, // H
            73 => LedId.Keyboard_I, // I
            74 => LedId.Keyboard_J, // J
            75 => LedId.Keyboard_K, // K
            76 => LedId.Keyboard_L, // L
            77 => LedId.Keyboard_M, // M
            78 => LedId.Keyboard_N, // N
            79 => LedId.Keyboard_O, // O
            80 => LedId.Keyboard_P, // P
            81 => LedId.Keyboard_Q, // Q
            82 => LedId.Keyboard_R, // R
            83 => LedId.Keyboard_S, // S
            84 => LedId.Keyboard_T, // T
            85 => LedId.Keyboard_U, // U
            86 => LedId.Keyboard_V, // V
            87 => LedId.Keyboard_W, // W
            88 => LedId.Keyboard_X, // X
            89 => LedId.Keyboard_Y, // Y
            90 => LedId.Keyboard_Z, // Z
            
            // Windows keys and application key
            91 => LedId.Keyboard_LeftGui, // LeftWindows
            92 => LedId.Keyboard_RightGui, // RightWindows
            93 => LedId.Keyboard_Application, // Application
            95 => null, // Sleep - TODO: no direct mapping
            
            // Numeric pad
            96 => LedId.Keyboard_Num0, // NumberPad0
            97 => LedId.Keyboard_Num1, // NumberPad1
            98 => LedId.Keyboard_Num2, // NumberPad2
            99 => LedId.Keyboard_Num3, // NumberPad3
            100 => LedId.Keyboard_Num4, // NumberPad4
            101 => LedId.Keyboard_Num5, // NumberPad5
            102 => LedId.Keyboard_Num6, // NumberPad6
            103 => LedId.Keyboard_Num7, // NumberPad7
            104 => LedId.Keyboard_Num8, // NumberPad8
            105 => LedId.Keyboard_Num9, // NumberPad9
            106 => LedId.Keyboard_NumAsterisk, // Multiply
            107 => LedId.Keyboard_NumPlus, // Add
            108 => LedId.Keyboard_NumComma, // Separator
            109 => LedId.Keyboard_NumMinus, // Subtract
            110 => LedId.Keyboard_NumPeriodAndDelete, // Decimal
            111 => LedId.Keyboard_NumSlash, // Divide
            
            // Function keys (F1-F24)
            112 => LedId.Keyboard_F1, // F1
            113 => LedId.Keyboard_F2, // F2
            114 => LedId.Keyboard_F3, // F3
            115 => LedId.Keyboard_F4, // F4
            116 => LedId.Keyboard_F5, // F5
            117 => LedId.Keyboard_F6, // F6
            118 => LedId.Keyboard_F7, // F7
            119 => LedId.Keyboard_F8, // F8
            120 => LedId.Keyboard_F9, // F9
            121 => LedId.Keyboard_F10, // F10
            122 => LedId.Keyboard_F11, // F11
            123 => LedId.Keyboard_F12, // F12
            124 => null, // F13 - TODO: no direct mapping
            125 => null, // F14 - TODO: no direct mapping
            126 => null, // F15 - TODO: no direct mapping
            127 => null, // F16 - TODO: no direct mapping
            128 => null, // F17 - TODO: no direct mapping
            129 => null, // F18 - TODO: no direct mapping
            130 => null, // F19 - TODO: no direct mapping
            131 => null, // F20 - TODO: no direct mapping
            132 => null, // F21 - TODO: no direct mapping
            133 => null, // F22 - TODO: no direct mapping
            134 => null, // F23 - TODO: no direct mapping
            135 => null, // F24 - TODO: no direct mapping
            
            // Navigation view buttons (Windows 10+)
            136 => null, // NavigationView - TODO: no direct mapping
            137 => null, // NavigationMenu - TODO: no direct mapping
            138 => null, // NavigationUp - TODO: no direct mapping
            139 => null, // NavigationDown - TODO: no direct mapping
            140 => null, // NavigationLeft - TODO: no direct mapping
            141 => null, // NavigationRight - TODO: no direct mapping
            142 => null, // NavigationAccept - TODO: no direct mapping
            143 => null, // NavigationCancel - TODO: no direct mapping
            
            // Lock keys
            144 => LedId.Keyboard_NumLock, // NumberKeyLock
            145 => LedId.Keyboard_ScrollLock, // Scroll
            
            // Left/Right specific modifier keys
            160 => LedId.Keyboard_LeftShift, // LeftShift
            161 => LedId.Keyboard_RightShift, // RightShift
            162 => LedId.Keyboard_LeftCtrl, // LeftControl
            163 => LedId.Keyboard_RightCtrl, // RightControl
            164 => LedId.Keyboard_LeftAlt, // LeftMenu
            165 => LedId.Keyboard_RightAlt, // RightMenu
            
            // Browser navigation keys (Windows 10+)
            166 => null, // GoBack - TODO: no direct mapping
            167 => null, // GoForward - TODO: no direct mapping
            168 => null, // Refresh - TODO: no direct mapping
            169 => null, // Stop - TODO: no direct mapping
            170 => null, // Search - TODO: no direct mapping
            171 => null, // Favorites - TODO: no direct mapping
            172 => null, // GoHome - TODO: no direct mapping
            
            // Gamepad buttons (Windows 10+) - not keyboard keys
            195 => LedId.GameController1, // GamepadA 
            196 => LedId.GameController2, // GamepadB 
            197 => LedId.GameController3, // GamepadX 
            198 => LedId.GameController4, // GamepadY 
            199 => LedId.GameController5, // GamepadRightShoulder
            200 => LedId.GameController6, // GamepadLeftShoulder 
            201 => LedId.GameController7, // GamepadLeftTrigger 
            202 => LedId.GameController8, // GamepadRightTrigger 
            203 => LedId.GameController9, // GamepadDPadUp 
            204 => LedId.GameController10, // GamepadDPadDown 
            205 => LedId.GameController11, // GamepadDPadLeft 
            206 => LedId.GameController12, // GamepadDPadRight 
            207 => LedId.GameController13, // GamepadMenu
            208 => LedId.GameController14, // GamepadView
            209 => LedId.GameController15, // GamepadLeftThumbstickButton 
            210 => LedId.GameController16, // GamepadRightThumbstickButton
            211 => LedId.GameController17, // GamepadLeftThumbstickUp 
            212 => LedId.GameController18, // GamepadLeftThumbstickDown 
            213 => LedId.GameController19, // GamepadLeftThumbstickRight 
            214 => LedId.GameController20, // GamepadLeftThumbstickLeft
            215 => LedId.GameController21, // GamepadRightThumbstickUp 
            216 => LedId.GameController22, // GamepadRightThumbstickDown 
            217 => LedId.GameController23, // GamepadRightThumbstickRight 
            218 => LedId.GameController24, // GamepadRightThumbstickLeft 
            
            _ => null
        };
    }
    
    public static RGBDeviceType GetDeviceType(string deviceType)
    {
        return deviceType switch
        {
            "Keyboard" => RGBDeviceType.Keyboard,
            "Mouse" => RGBDeviceType.Mouse,
            "GameController" => RGBDeviceType.GameController,
            "Wearable" => RGBDeviceType.Headset,
            _ => RGBDeviceType.Unknown
        };
    }

    public static string GetVendorDisplayName(uint vendorId)
    {
        // These are mentioned on the LampArray documentation page, more can be added as needed and missing ones still work fine
        return vendorId switch
        {
            0x046D => "Logitech",
            0x042E => "Acer",
            0x0B05 => "ASUS",
            0x1532 => "Razer",
            0x1038 => "SteelSeries",
            0x258A => "HyperX",
            0x03F0 => "HP",
            0x413F => "HP",
            0x2E8C => "Twinkly",
            _ => $"Unknown Vendor (0x{vendorId:X4})"
        };
    }
}
