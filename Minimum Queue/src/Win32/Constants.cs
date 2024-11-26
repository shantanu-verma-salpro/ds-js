namespace ImGuiNET
{
    public static class WinConstants
    {
        #region Error Codes
        public enum Errors : int
        {
            ERROR_CLASS_ALREADY_EXISTS = 1410,
            ERROR_INVALID_WINDOW_HANDLE = 1400,
            ERROR_ACCESS_DENIED = 5,
            ERROR_NOT_ENOUGH_MEMORY = 8,
            ERROR_OUTOFMEMORY = 14
        }
        #endregion

        #region Cursors
        public enum SystemCursors : int
        {
            IDC_ARROW = 32512,
            IDC_IBEAM = 32513,
            IDC_WAIT = 32514,
            IDC_CROSS = 32515,
            IDC_UPARROW = 32516,
            IDC_SIZE = 32640,
            IDC_ICON = 32641,
            IDC_SIZENWSE = 32642,
            IDC_SIZENESW = 32643,
            IDC_SIZEWE = 32644,
            IDC_SIZENS = 32645,
            IDC_SIZEALL = 32646,
            IDC_NO = 32648,
            IDC_HAND = 32649,
            IDC_APPSTARTING = 32650,
            IDC_HELP = 32651
        }
        #endregion

        #region TME
        [Flags]
        public enum TME : uint
        {
            TME_LEAVE = 0x00000002,
            TME_NONCLIENT = 0x00000010,
            TME_CANCEL = 0x80000000
        }
        #endregion

        #region Window Styles
        [Flags]
        public enum WindowStyles : uint
        {
            WS_OVERLAPPED = 0x00000000,
            WS_POPUP = 0x80000000,
            WS_CHILD = 0x40000000,
            WS_MINIMIZE = 0x20000000,
            WS_VISIBLE = 0x10000000,
            WS_DISABLED = 0x08000000,
            WS_CLIPSIBLINGS = 0x04000000,
            WS_CLIPCHILDREN = 0x02000000,
            WS_MAXIMIZE = 0x01000000,
            WS_CAPTION = 0x00C00000,
            WS_BORDER = 0x00800000,
            WS_DLGFRAME = 0x00400000,
            WS_VSCROLL = 0x00200000,
            WS_HSCROLL = 0x00100000,
            WS_SYSMENU = 0x00080000,
            WS_THICKFRAME = 0x00040000,
            WS_GROUP = 0x00020000,
            WS_TABSTOP = 0x00010000,
            WS_MINIMIZEBOX = 0x00020000,
            WS_MAXIMIZEBOX = 0x00010000,
            WS_TILED = WS_OVERLAPPED,
            WS_ICONIC = WS_MINIMIZE,
            WS_SIZEBOX = WS_THICKFRAME,
            WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW,
            WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
            WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
            WS_CHILDWINDOW = WS_CHILD
        }

        [Flags]
        public enum WindowStylesEx : int
        {
            WS_EX_DLGMODALFRAME = 0x00000001,
            WS_EX_NOPARENTNOTIFY = 0x00000004,
            WS_EX_TOPMOST = 0x00000008,
            WS_EX_ACCEPTFILES = 0x00000010,
            WS_EX_TRANSPARENT = 0x00000020,
            WS_EX_TOOLWINDOW = 0x00000080,
            WS_EX_APPWINDOW = 0x00040000,
            WS_EX_LAYERED = 0x00080000,
            WS_EX_NOACTIVATE = 0x08000000,
            WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE,
            WS_EX_WINDOWEDGE = 0x00000100,
            WS_EX_CLIENTEDGE = 0x00000200
        }
        #endregion

        #region LangId to Code Page Mapping
        public static readonly Dictionary<int, int> LangIdToCodePageMap = new()
        {
            { 0x0409, 1252 }, // English (United States)
            { 0x0809, 1252 }, // English (United Kingdom)
            { 0x0411, 932 },  // Japanese
            { 0x0412, 949 },  // Korean
            { 0x0419, 1251 }, // Russian
            { 0x0804, 936 },  // Chinese (Simplified, PRC)
            { 0x0404, 950 },  // Chinese (Traditional, Taiwan)
            { 0x0407, 1252 }, // German
            { 0x040C, 1252 }, // French
            { 0x0410, 1252 }, // Italian
            { 0x0416, 1252 }, // Portuguese (Brazil)
            { 0x0816, 1252 }, // Portuguese (Portugal)
            { 0x041D, 1252 }, // Swedish
            { 0x0413, 1252 }, // Dutch (Netherlands)
            { 0x040A, 1252 }, // Spanish (Spain)
            { 0x0C0A, 1252 }, // Spanish (Mexico)
            { 0x0420, 1256 }, // Arabic
            { 0x0427, 1250 }, // Lithuanian
            { 0x0415, 1250 }, // Polish
            { 0x040E, 1250 }, // Hungarian
            { 0x0406, 1252 }, // Danish
            { 0x0408, 1253 }, // Greek
            { 0x0422, 1251 }, // Ukrainian
            { 0x041B, 1250 }, // Slovak
            { 0x0425, 1250 }, // Estonian
        };
        #endregion

        [Flags]
        public enum WindowClassStyles
        {
            CS_BYTEALIGNCLIENT = 0x1000,
            CS_BYTEALIGNWINDOW = 0x2000,
            CS_CLASSDC = 0x0040,
            CS_DBLCLKS = 0x0008,
            CS_DROPSHADOW = 0x00020000,
            CS_GLOBALCLASS = 0x4000,
            CS_HREDRAW = 0x0002,
            CS_NOCLOSE = 0x0200,
            CS_OWNDC = 0x0020,
            CS_PARENTDC = 0x0080,
            CS_SAVEBITS = 0x0800,
            CS_VREDRAW = 0x0001
        }

        #region Show Window Commands
        public enum SW : int
        {
            Hide = 0,
            Normal = 1,
            ShowMinimized = 2,
            Maximize = 3,
            ShowMaximized = 3,
            ShowNoActivate = 4,
            Show = 5,
            Minimize = 6,
            ShowMinNoActive = 7,
            ShowNA = 8,
            Restore = 9,
            ShowDefault = 10,
            ForceMinimize = 11
        }
        #endregion

        #region Virtual Keys
        public enum VK : int
        {
            VK_LBUTTON = 0x01, VK_RBUTTON = 0x02, VK_CANCEL = 0x03, VK_MBUTTON = 0x04,
            VK_XBUTTON1 = 0x05, VK_XBUTTON2 = 0x06, VK_BACK = 0x08, VK_TAB = 0x09,
            VK_CLEAR = 0x0C, VK_RETURN = 0x0D, VK_SHIFT = 0x10, VK_CONTROL = 0x11,
            VK_MENU = 0x12, VK_PAUSE = 0x13, VK_CAPITAL = 0x14, VK_KANA = 0x15,
            VK_HANGUEL = 0x15, VK_HANGUL = 0x15, VK_IME_ON = 0x16, VK_JUNJA = 0x17,
            VK_FINAL = 0x18, VK_HANJA = 0x19, VK_KANJI = 0x19, VK_IME_OFF = 0x1A,
            VK_ESCAPE = 0x1B, VK_CONVERT = 0x1C, VK_NONCONVERT = 0x1D, VK_ACCEPT = 0x1E,
            VK_MODECHANGE = 0x1F, VK_SPACE = 0x20, VK_PRIOR = 0x21, VK_NEXT = 0x22,
            VK_END = 0x23, VK_HOME = 0x24, VK_LEFT = 0x25, VK_UP = 0x26, VK_RIGHT = 0x27,
            VK_DOWN = 0x28, VK_SELECT = 0x29, VK_PRINT = 0x2A, VK_EXECUTE = 0x2B,
            VK_SNAPSHOT = 0x2C, VK_INSERT = 0x2D, VK_DELETE = 0x2E, VK_HELP = 0x2F,
            VK_0 = 0x30, VK_1 = 0x31, VK_2 = 0x32, VK_3 = 0x33, VK_4 = 0x34,
            VK_5 = 0x35, VK_6 = 0x36, VK_7 = 0x37, VK_8 = 0x38, VK_9 = 0x39,
            VK_A = 0x41, VK_B = 0x42, VK_C = 0x43, VK_D = 0x44, VK_E = 0x45,
            VK_F = 0x46, VK_G = 0x47, VK_H = 0x48, VK_I = 0x49, VK_J = 0x4A,
            VK_K = 0x4B, VK_L = 0x4C, VK_M = 0x4D, VK_N = 0x4E, VK_O = 0x4F,
            VK_P = 0x50, VK_Q = 0x51, VK_R = 0x52, VK_S = 0x53, VK_T = 0x54,
            VK_U = 0x55, VK_V = 0x56, VK_W = 0x57, VK_X = 0x58, VK_Y = 0x59,
            VK_Z = 0x5A, VK_LWIN = 0x5B, VK_RWIN = 0x5C, VK_APPS = 0x5D, VK_SLEEP = 0x5F,
            VK_NUMPAD0 = 0x60, VK_NUMPAD1 = 0x61, VK_NUMPAD2 = 0x62, VK_NUMPAD3 = 0x63,
            VK_NUMPAD4 = 0x64, VK_NUMPAD5 = 0x65, VK_NUMPAD6 = 0x66, VK_NUMPAD7 = 0x67,
            VK_NUMPAD8 = 0x68, VK_NUMPAD9 = 0x69, VK_MULTIPLY = 0x6A, VK_ADD = 0x6B,
            VK_SEPARATOR = 0x6C, VK_SUBTRACT = 0x6D, VK_DECIMAL = 0x6E, VK_DIVIDE = 0x6F,
            VK_F1 = 0x70, VK_F2 = 0x71, VK_F3 = 0x72, VK_F4 = 0x73, VK_F5 = 0x74,
            VK_F6 = 0x75, VK_F7 = 0x76, VK_F8 = 0x77, VK_F9 = 0x78, VK_F10 = 0x79,
            VK_F11 = 0x7A, VK_F12 = 0x7B, VK_F13 = 0x7C, VK_F14 = 0x7D, VK_F15 = 0x7E,
            VK_F16 = 0x7F, VK_F17 = 0x80, VK_F18 = 0x81, VK_F19 = 0x82, VK_F20 = 0x83,
            VK_F21 = 0x84, VK_F22 = 0x85, VK_F23 = 0x86, VK_F24 = 0x87, VK_NUMLOCK = 0x90,
            VK_SCROLL = 0x91, VK_OEM_NEC_EQUAL = 0x92, VK_OEM_FJ_JISHO = 0x92,
            VK_OEM_FJ_MASSHOU = 0x93, VK_OEM_FJ_TOUROKU = 0x94, VK_OEM_FJ_LOYA = 0x95,
            VK_OEM_6 = 0xDD, VK_OEM_7 = 0xDE, VK_OEM_8 = 0xDF, VK_OEM_4 = 0xDB, VK_OEM_5 = 0xDC,
            VK_OEM_FJ_ROYA = 0x96, VK_LSHIFT = 0xA0, VK_RSHIFT = 0xA1, VK_LCONTROL = 0xA2,
            VK_RCONTROL = 0xA3, VK_LMENU = 0xA4, VK_RMENU = 0xA5, VK_BROWSER_BACK = 0xA6,
            VK_BROWSER_FORWARD = 0xA7, VK_BROWSER_REFRESH = 0xA8, VK_BROWSER_STOP = 0xA9,
            VK_BROWSER_SEARCH = 0xAA, VK_BROWSER_FAVORITES = 0xAB, VK_BROWSER_HOME = 0xAC,
            VK_VOLUME_MUTE = 0xAD, VK_VOLUME_DOWN = 0xAE, VK_VOLUME_UP = 0xAF,
            VK_MEDIA_NEXT_TRACK = 0xB0, VK_MEDIA_PREV_TRACK = 0xB1, VK_MEDIA_STOP = 0xB2,
            VK_MEDIA_PLAY_PAUSE = 0xB3, VK_LAUNCH_MAIL = 0xB4, VK_LAUNCH_MEDIA_SELECT = 0xB5,
            VK_LAUNCH_APP1 = 0xB6, VK_LAUNCH_APP2 = 0xB7, VK_OEM_1 = 0xBA, VK_OEM_PLUS = 0xBB,
            VK_OEM_COMMA = 0xBC, VK_OEM_MINUS = 0xBD, VK_OEM_PERIOD = 0xBE, VK_OEM_2 = 0xBF,
            VK_OEM_3 = 0xC0, VK_GAMEPAD_A = 0xC3, VK_GAMEPAD_B = 0xC4, VK_GAMEPAD_X = 0xC5,
            VK_GAMEPAD_Y = 0xC6, VK_GAMEPAD_RIGHT_SHOULDER = 0xC7, VK_GAMEPAD_LEFT_SHOULDER = 0xC8,
            VK_GAMEPAD_LEFT_TRIGGER = 0xC9, VK_GAMEPAD_RIGHT_TRIGGER = 0xCA,
            VK_GAMEPAD_DPAD_UP = 0xCB, VK_GAMEPAD_DPAD_DOWN = 0xCC, VK_GAMEPAD_DPAD_LEFT = 0xCD,
            VK_GAMEPAD_DPAD_RIGHT = 0xCE, VK_GAMEPAD_MENU = 0xCF, VK_GAMEPAD_VIEW = 0xD0,
            VK_GAMEPAD_LEFT_THUMBSTICK_BUTTON = 0xD1, VK_GAMEPAD_RIGHT_THUMBSTICK_BUTTON = 0xD2,
            VK_GAMEPAD_LEFT_THUMBSTICK_UP = 0xD3, VK_GAMEPAD_LEFT_THUMBSTICK_DOWN = 0xD4,
            VK_GAMEPAD_LEFT_THUMBSTICK_RIGHT = 0xD5, VK_GAMEPAD_LEFT_THUMBSTICK_LEFT = 0xD6,
            VK_GAMEPAD_RIGHT_THUMBSTICK_UP = 0xD7, VK_GAMEPAD_RIGHT_THUMBSTICK_DOWN = 0xD8,
            VK_GAMEPAD_RIGHT_THUMBSTICK_RIGHT = 0xD9, VK_GAMEPAD_RIGHT_THUMBSTICK_LEFT = 0xDA
        }
        #endregion

        #region System Metrics
        public enum SM : int
        {
            SM_CXSCREEN = 0,
            SM_CYSCREEN = 1,
            SM_CXVSCROLL = 2,
            SM_CYHSCROLL = 3,
            SM_CYCAPTION = 4,
            SM_CXBORDER = 5,
            SM_CYBORDER = 6,
            SM_CXFIXEDFRAME = 7,
            SM_CYFIXEDFRAME = 8,
            SM_CYVTHUMB = 9,
            SM_CXHTHUMB = 10,
            SM_CXICON = 11,
            SM_CYICON = 12,
            SM_CXCURSOR = 13,
            SM_CYCURSOR = 14,
            SM_CYMENU = 15,
            SM_CXFULLSCREEN = 16,
            SM_CYFULLSCREEN = 17,
            SM_CYKANJIWINDOW = 18,
            SM_MOUSEPRESENT = 19

        }
        #endregion

        #region Brush Constants
        public enum Brush : int
        {
            WHITE_BRUSH = 0,
            LTGRAY_BRUSH = 1,
            GRAY_BRUSH = 2,
            DKGRAY_BRUSH = 3,
            BLACK_BRUSH = 4,
            NULL_BRUSH = 5,
            HOLLOW_BRUSH = NULL_BRUSH
        }
        #endregion

        #region Window Messages
        public enum WM : int
        {
            WM_MOUSEMOVE = 0x0200,
            WM_NCMOUSEMOVE = 0x00A0,
            WM_MOUSELEAVE = 0x02A3,
            WM_NCMOUSELEAVE = 0x00A2,
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_LBUTTONDBLCLK = 0x0203,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205,
            WM_RBUTTONDBLCLK = 0x0206,
            WM_MBUTTONDOWN = 0x0207,
            WM_MBUTTONUP = 0x0208,
            WM_MBUTTONDBLCLK = 0x0209,
            WM_XBUTTONDOWN = 0x020B,
            WM_XBUTTONUP = 0x020C,
            WM_XBUTTONDBLCLK = 0x020D,
            WM_KEYDOWN = 0x0100,
            WM_KEYUP = 0x0101,
            WM_SYSKEYDOWN = 0x0104,
            WM_SYSKEYUP = 0x0105,
            WM_CHAR = 0x0102,
            WM_SETFOCUS = 0x0007,
            WM_KILLFOCUS = 0x0008,
            WM_DESTROY = 0x0002,
            WM_INPUTLANGCHANGE = 0x0051,
            WM_SETCURSOR = 0x0020,
            WM_DEVICECHANGE = 0x0219,
            WM_MOUSEWHEEL = 0x020A,
            WM_MOUSEHWHEEL = 0x020E
        }
        #endregion
    }

}