
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;



namespace ImGuiNET;
partial class Win32ImGui
{

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT(int x, int y)
    {
        public int X = x;
        public int Y = y;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct RECT(int left, int top, int right, int bottom)
    {
        public int Left = left;
        public int Top = top;
        public int Right = right;
        public int Bottom = bottom;

        public readonly int Width => Right - Left;
        public readonly int Height => Bottom - Top;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ImGui_ImplWin32_Data
    {
        public nint hWnd;
        public nint MouseHwnd;
        public int MouseTrackedArea;
        public int MouseButtonsDown;
        public long Time;
        public long TicksPerSecond;
        public int LastMouseCursor;
        public uint KeyboardCodePage;

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TRACKMOUSEEVENT
    {
        public uint cbSize;
        public uint dwFlags;
        public IntPtr hwndTrack;
        public uint dwHoverTime;
    }


    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct WindowMessage
    {
        public IntPtr hwnd;
        public UInt32 message;
        public UIntPtr wParam;
        public UIntPtr lParam;
        public UInt32 time;
        public POINT pt;
    }

    private delegate bool WndProc(IntPtr hWnd, uint msg, UIntPtr wParam, IntPtr lParam);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct WNDCLASSEX
    {
        [MarshalAs(UnmanagedType.U4)]
        public int cbSize;
        [MarshalAs(UnmanagedType.U4)]
        public int style;
        public IntPtr lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;
        public string lpszMenuName;
        public string lpszClassName;
        public IntPtr hIconSm;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PAINTSTRUCT
    {
        public IntPtr hdc;
        public bool fErase;
        public RECT rcPaint;
        public bool fRestore;
        public bool fIncUpdate;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] rgbReserved;
    }
    public static partial class NativeMethods
    {
        [LibraryImport("user32.dll")]
        public static partial IntPtr GetKeyboardLayout(uint idThread);

        [LibraryImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool QueryPerformanceCounter(out long lpPerformanceCount);

        [LibraryImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool QueryPerformanceFrequency(out long lpFrequency);

        [LibraryImport("user32.dll")]
        public static partial short GetKeyState(int vKey);

     

        [LibraryImport("user32.dll", EntryPoint = "LoadCursorW", StringMarshalling = StringMarshalling.Utf16)]
        public static partial IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

        [LibraryImport("user32.dll")]
        public static partial IntPtr SetCursor(IntPtr hCursor);

        [LibraryImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool GetCursorPos(out POINT lpPoint);

        [LibraryImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool SetCursorPos(int X, int Y);

        [LibraryImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

        [LibraryImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

        [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Utf16)]
        public static partial IntPtr GetModuleHandleW(string lpModuleName);

        [LibraryImport("user32.dll", SetLastError = true)]
        public static partial IntPtr GetForegroundWindow();

        [LibraryImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [LibraryImport("user32.dll")]
        public static partial IntPtr GetMessageExtraInfo();

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool TrackMouseEvent(ref TRACKMOUSEEVENT lpEventTrack);

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool SetCapture(IntPtr hWnd);

        [LibraryImport("user32.dll")]
        public static partial IntPtr GetCapture();

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool ReleaseCapture();

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool IsWindowUnicode(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern short RegisterClassEx([In] ref WNDCLASSEX lpwcx);

        [LibraryImport("user32.dll", EntryPoint = "CreateWindowExW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        public static partial IntPtr CreateWindowEx(
            uint dwExStyle,
            string lpClassName,
            string lpWindowName,
            uint dwStyle,
            int x,
            int y,
            int nWidth,
            int nHeight,
            IntPtr hWndParent,
            IntPtr hMenu,
            IntPtr hInstance,
            IntPtr lpParam
        );

        [LibraryImport("user32.dll", EntryPoint = "DefWindowProcW", SetLastError = false, StringMarshalling = StringMarshalling.Utf16)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool DefWindowProc(
            IntPtr hWnd,
            uint uMsg,
            UIntPtr wParam,
            IntPtr lParam
        );

        [LibraryImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool DestroyWindow(IntPtr hWnd);


        [LibraryImport("user32.dll", EntryPoint = "GetMessageW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool GetMessage(
            out WindowMessage lpMsg,
            IntPtr hWnd,
            uint wMsgFilterMin,
            uint wMsgFilterMax
        );

        [LibraryImport("user32.dll", EntryPoint = "PeekMessageW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool PeekMessage(
            out WindowMessage lpMsg,
            IntPtr hWnd,
            uint wMsgFilterMin,
            uint wMsgFilterMax,
            uint wRemoveMsg);

        [LibraryImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool TranslateMessage(ref WindowMessage lpMsg);

        [LibraryImport("user32.dll", SetLastError = true)]
        public static partial IntPtr DispatchMessageW(ref WindowMessage lpMsg);

        [LibraryImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [LibraryImport("gdi32.dll", SetLastError = true)]
        public static partial IntPtr GetStockObject(int fnObject);

        [LibraryImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool UpdateWindow(IntPtr hWnd);

    }

    public enum SystemCursors
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
    public enum SW
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

    public static class BitwiseHelpers
    {

        public static int GET_X_LPARAM(IntPtr lp) => unchecked((short)(long)lp);
        public static int GET_Y_LPARAM(IntPtr lp) => unchecked((short)((long)lp >> 16));
        public static byte HIBYTE(ushort wValue) => (byte)((wValue >> 8) & 0xff);
        public static ushort HIWORD(uint dwValue) => (ushort)((dwValue >> 16) & 0xffff);
        public static ushort HIWORD(IntPtr dwValue) => unchecked((ushort)((long)dwValue >> 16));
        public static ushort HIWORD(UIntPtr dwValue) => unchecked((ushort)((ulong)dwValue >> 16));
        public static bool IS_INTRESOURCE(IntPtr ptr) => unchecked((ulong)ptr.ToInt64()) >> 16 == 0;
        public static byte LOBYTE(ushort wValue) => (byte)(wValue & 0xff);
        public static ushort LOWORD(uint dwValue) => (ushort)(dwValue & 0xffff);
        public static ushort LOWORD(IntPtr dwValue) => unchecked((ushort)(long)dwValue);
        public static ushort LOWORD(UIntPtr dwValue) => unchecked((ushort)(ulong)dwValue);
        public static int GET_WHEEL_DELTA_WPARAM(IntPtr wParam)
        {
            return (short)HIWORD(wParam);
        }

        public static int GET_WHEEL_DELTA_WPARAM(uint wParam)
        {
            return (short)HIWORD(wParam);
        }
    }


    private static readonly Dictionary<int, int> LangIdToCodePageMap = new()
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

    public static class Utils
    {
        public static int GetCurrentKeyboardCodePage() => LangIdToCodePageMap.TryGetValue((ushort)(NativeMethods.GetKeyboardLayout(0).ToInt64() & 0xFFFF), out var codePage) ? codePage : 0;

        public static bool IsKeyPressed(int vKey) => (NativeMethods.GetKeyState(vKey) & 0x8000) != 0;

        public static IntPtr LoadSystemCursor(SystemCursors cursorName) => NativeMethods.LoadCursor(IntPtr.Zero, (int)cursorName);

        public static void SetSystemCursor(SystemCursors cursorId)
        {
            IntPtr cursorHandle = LoadSystemCursor(cursorId);
            NativeMethods.SetCursor(cursorHandle);
        }
        public static bool GetCursorPosition(out POINT point) => NativeMethods.GetCursorPos(out point);

        public static bool SetCursorPosition(int x, int y) => NativeMethods.SetCursorPos(x, y);

        public static bool ClientToScreen(IntPtr hWnd, ref POINT point) => NativeMethods.ClientToScreen(hWnd, ref point);

        public static bool ScreenToClient(IntPtr hWnd, ref POINT point) => NativeMethods.ScreenToClient(hWnd, ref point);
        public static IntPtr GetForegroundWindowHandle() => NativeMethods.GetForegroundWindow();
        public static bool GetClientRect(IntPtr hWnd, out RECT rect) => NativeMethods.GetClientRect(hWnd, out rect);
        public static void SetMouseCapture(IntPtr hWnd)
        {
            NativeMethods.SetCapture(hWnd);
        }

        public static IntPtr GetMouseCapture()
        {
            return NativeMethods.GetCapture();
        }

        public static void ReleaseMouseCapture()
        {
            NativeMethods.ReleaseCapture();
        }

        public static string MultiByteToWC(string multiByteStr, int codePage)
        {
            var encoder = Encoding.GetEncoding(codePage);
            byte[] byteArray = encoder.GetBytes(multiByteStr);
            return Encoding.Unicode.GetString(byteArray);
        }

        public static IntPtr CreateWindow(in string className, IntPtr hInstance)
        {
            return NativeMethods.CreateWindowEx(
            WindowStylesEx.WS_EX_OVERLAPPEDWINDOW,
            className,
            "The Hello Program",
            (uint)WindowStyles.WS_OVERLAPPEDWINDOW,
            -1, -1, 800, 600,
            IntPtr.Zero, IntPtr.Zero, hInstance, IntPtr.Zero);
        }

        public static void CreateAndRegisterWindowClass(in string className, IntPtr hInstance)
        {
            WNDCLASSEX wndClass = new()
            {
                cbSize = Marshal.SizeOf(typeof(WNDCLASSEX)),
                style = 0x01 | 0x02,
                lpfnWndProc = Marshal.GetFunctionPointerForDelegate<WndProc>(CustomWndProc),
                cbClsExtra = 0,
                cbWndExtra = 0,
                hInstance = hInstance,
                hCursor = Utils.LoadSystemCursor(SystemCursors.IDC_HAND),
                hIcon = default,
                hbrBackground = NativeMethods.GetStockObject(WHITE_BRUSH),
                lpszMenuName = null,
                lpszClassName = className
            };

            ushort classAtom = (ushort)NativeMethods.RegisterClassEx(ref wndClass);
            if (classAtom == 0 && Marshal.GetLastWin32Error() != ERROR_CLASS_ALREADY_EXISTS)
            {
                Console.WriteLine("Failed to register window class.");
                return;
            }

        }


    }
    private const int ERROR_CLASS_ALREADY_EXISTS = 1410;
    private const int WHITE_BRUSH = 0;
    private static POINT point = new();
    private static RECT rect = new();
    private static TRACKMOUSEEVENT tmeCancel = new();
    private static TRACKMOUSEEVENT tmeTrack = new();
    public static class WindowStylesEx
    {
        public const uint WS_EX_OVERLAPPEDWINDOW = 0x00000300;
    }

    [Flags]
    public enum WindowStyles
    {
        WS_BORDER = 0x00800000,
        WS_CAPTION = 0x00C00000,
        WS_CHILD = 0x40000000,
        WS_CHILDWINDOW = 0x40000000,
        WS_CLIPCHILDREN = 0x02000000,
        WS_CLIPSIBLINGS = 0x04000000,
        WS_DISABLED = 0x08000000,
        WS_DLGFRAME = 0x00400000,
        WS_GROUP = 0x00020000,
        WS_HSCROLL = 0x00100000,
        WS_ICONIC = 0x20000000,
        WS_MAXIMIZE = 0x01000000,
        WS_MAXIMIZEBOX = 0x00010000,
        WS_MINIMIZE = 0x20000000,
        WS_MINIMIZEBOX = 0x00020000,
        WS_OVERLAPPED = 0x00000000,
        WS_OVERLAPPEDWINDOW =
            WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
        WS_POPUP = unchecked((int)0x80000000),
        WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
        WS_SIZEBOX = 0x00040000,
        WS_SYSMENU = 0x00080000,
        WS_TABSTOP = 0x00010000,
        WS_THICKFRAME = 0x00040000,
        WS_TILED = 0x00000000,
        WS_TILEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
        WS_VISIBLE = 0x10000000,
        WS_VSCROLL = 0x00200000
    }

    public static class ImGuiUtils
    {
        public static void SetImGuiCursor(ImGuiMouseCursor imguiCursor)
        {
            SystemCursors cursorId = imguiCursor switch
            {
                ImGuiMouseCursor.Arrow => SystemCursors.IDC_ARROW,
                ImGuiMouseCursor.TextInput => SystemCursors.IDC_IBEAM,
                ImGuiMouseCursor.ResizeAll => SystemCursors.IDC_SIZEALL,
                ImGuiMouseCursor.ResizeEW => SystemCursors.IDC_SIZEWE,
                ImGuiMouseCursor.ResizeNS => SystemCursors.IDC_SIZENS,
                ImGuiMouseCursor.ResizeNESW => SystemCursors.IDC_SIZENESW,
                ImGuiMouseCursor.ResizeNWSE => SystemCursors.IDC_SIZENWSE,
                ImGuiMouseCursor.Hand => SystemCursors.IDC_HAND,
                ImGuiMouseCursor.NotAllowed => SystemCursors.IDC_NO,
                _ => SystemCursors.IDC_ARROW,
            };

            SetSystemCursor(cursorId);
        }

        public static void SetSystemCursor(SystemCursors cursorId)
        {
            IntPtr cursorHandle = LoadSystemCursor(cursorId);
            NativeMethods.SetCursor(cursorHandle);
        }

        private static IntPtr LoadSystemCursor(SystemCursors cursorId)
        {
            return NativeMethods.LoadCursor(IntPtr.Zero, (int)cursorId);
        }
    }

    private static bool CustomWndProc(IntPtr hWnd, uint msg, UIntPtr wParam, IntPtr lParam)
    {
        if (ImGui_ImplWin32_WndProcHandler(hWnd, msg, wParam, lParam) != 0)
            return true;

        switch (msg)
        {
            case 0x0010:
                NativeMethods.DestroyWindow(hWnd);
                return false;
            case 0x0002:
                Environment.Exit(0);
                break;
        }
        return NativeMethods.DefWindowProc(hWnd, msg, wParam, lParam);
    }

    public unsafe static ImGui_ImplWin32_Data* ImGui_ImplWin32_GetBackendData()
    {
        if (ImGui.GetCurrentContext() == IntPtr.Zero) return null;
        return (ImGui_ImplWin32_Data*)ImGui.GetIO().NativePtr->BackendPlatformUserData;
    }

    public static unsafe void ImGui_ImplWin32_UpdateKeyboardCodePage(ImGuiIOPtr io)
    {
        ImGui_ImplWin32_Data* backendDataPtr = (ImGui_ImplWin32_Data*)io.BackendPlatformUserData.ToPointer();
        if (backendDataPtr == null) return;
        backendDataPtr->KeyboardCodePage = (uint)Utils.GetCurrentKeyboardCodePage();
    }

    public static unsafe bool ImGui_ImplWin32_Init(IntPtr hwnd, bool platform_has_own_dc = false)
    {
        ImGuiIOPtr io = ImGui.GetIO();
        System.Diagnostics.Debug.Assert(io.NativePtr->BackendPlatformUserData == null, "Already initialized a platform backend!");
        if (!NativeMethods.QueryPerformanceFrequency(out long perf_frequency) || !NativeMethods.QueryPerformanceCounter(out long perf_counter))
            return false;
        IntPtr bdPtr = Marshal.AllocHGlobal(Marshal.SizeOf<ImGui_ImplWin32_Data>());

        try
        {
            var bd = new ImGui_ImplWin32_Data
            {
                hWnd = hwnd,
                TicksPerSecond = perf_frequency,
                Time = perf_counter,
                LastMouseCursor = (int)ImGuiMouseCursor.COUNT
            };

            Marshal.StructureToPtr(bd, bdPtr, false);
            io.NativePtr->BackendPlatformUserData = bdPtr.ToPointer();
            io.BackendFlags |= ImGuiBackendFlags.HasMouseCursors | ImGuiBackendFlags.HasSetMousePos;
            var mainViewport = ImGui.GetMainViewport();
            mainViewport.PlatformHandle = mainViewport.PlatformHandleRaw = hwnd;
            ImGui_ImplWin32_UpdateKeyboardCodePage(io);

            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error during initialization: {ex.Message}");
            return false;
        }
    }

    public unsafe static void ImGui_ImplWin32_Shutdown()
    {
        ImGui_ImplWin32_Data* bd = ImGui_ImplWin32_GetBackendData();
        System.Diagnostics.Debug.Assert(bd != null, "No platform backend to shutdown, or already shutdown?");
        ImGuiIOPtr io = ImGui.GetIO();
        if (io.BackendPlatformUserData != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(io.BackendPlatformUserData);
            io.BackendPlatformUserData = IntPtr.Zero;
            io.BackendFlags &= ~(ImGuiBackendFlags.HasMouseCursors | ImGuiBackendFlags.HasSetMousePos | ImGuiBackendFlags.HasGamepad);
        }
    }
    public static bool ImGui_ImplWin32_UpdateMouseCursor(ImGuiIOPtr io, ImGuiMouseCursor imguiCursor)
    {
        if (io.ConfigFlags.HasFlag(ImGuiConfigFlags.NoMouseCursorChange))
            return false;

        if (imguiCursor == ImGuiMouseCursor.None || io.MouseDrawCursor)
            NativeMethods.SetCursor(IntPtr.Zero);

        else
        {
            ImGuiUtils.SetImGuiCursor(imguiCursor);
        }

        return true;
    }

    [Flags]
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

    [Flags]
    public enum WM : uint
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

    [Flags]
    public enum TME : uint
    {
        TME_LEAVE = 0x00000002,
        TME_NONCLIENT = 0x00000010,
        TME_CANCEL = 0x80000000
    }

    static bool IsVkDown(VK vk)
    {
        return Utils.IsKeyPressed((int)vk);
    }
    static void ImGui_ImplWin32_AddKeyEvent(ImGuiIOPtr io, ImGuiKey key, bool down, int native_keycode, int native_scancode = -1)
    {
        io.AddKeyEvent(key, down);
        io.SetKeyEventNativeData(key, native_keycode, native_scancode);
        _ = native_scancode;
    }
    static void ImGui_ImplWin32_ProcessKeyEventsWorkarounds(ImGuiIOPtr io)
    {
        if (ImGui.IsKeyDown(ImGuiKey.LeftShift) && !IsVkDown(VK.VK_LSHIFT))
            ImGui_ImplWin32_AddKeyEvent(io, ImGuiKey.LeftShift, false, (int)VK.VK_LSHIFT);
        if (ImGui.IsKeyDown(ImGuiKey.RightShift) && !IsVkDown(VK.VK_RSHIFT))
            ImGui_ImplWin32_AddKeyEvent(io, ImGuiKey.RightShift, false, (int)VK.VK_RSHIFT);

        if (ImGui.IsKeyDown(ImGuiKey.LeftSuper) && !IsVkDown(VK.VK_LWIN))
            ImGui_ImplWin32_AddKeyEvent(io, ImGuiKey.LeftSuper, false, (int)VK.VK_LWIN);
        if (ImGui.IsKeyDown(ImGuiKey.RightSuper) && !IsVkDown(VK.VK_RWIN))
            ImGui_ImplWin32_AddKeyEvent(io, ImGuiKey.RightSuper, false, (int)VK.VK_RWIN);
    }
    static void ImGui_ImplWin32_UpdateKeyModifiers(ImGuiIOPtr io)
    {
        io.AddKeyEvent(ImGuiKey.ModCtrl, IsVkDown(VK.VK_CONTROL));
        io.AddKeyEvent(ImGuiKey.ModShift, IsVkDown(VK.VK_SHIFT));
        io.AddKeyEvent(ImGuiKey.ModAlt, IsVkDown(VK.VK_MENU));
        io.AddKeyEvent(ImGuiKey.ModSuper, IsVkDown(VK.VK_LWIN) || IsVkDown(VK.VK_RWIN));
    }

    static unsafe void ImGui_ImplWin32_UpdateMouseData(ImGuiIOPtr io)
    {
        ImGui_ImplWin32_Data* bd = ImGui_ImplWin32_GetBackendData();
        System.Diagnostics.Debug.Assert(bd->hWnd != IntPtr.Zero, "No platform backend to shutdown, or already shutdown?");

        IntPtr focusedWindow = NativeMethods.GetForegroundWindow();
        bool isAppFocused = focusedWindow == bd->hWnd;

        if (isAppFocused)
        {
            if (io.WantSetMousePos)
            {
                point.X = (int)io.MousePos.X;
                point.Y = (int)io.MousePos.Y;
                if (NativeMethods.ClientToScreen(bd->hWnd, ref point))
                {
                    NativeMethods.SetCursorPos(point.X, point.Y);
                }
            }

            if (!io.WantSetMousePos && bd->MouseTrackedArea == 0)
            {
                if (NativeMethods.GetCursorPos(out point) && NativeMethods.ScreenToClient(bd->hWnd, ref point))
                {
                    io.AddMousePosEvent((float)point.X, (float)point.Y);
                }
            }
        }
    }

    public unsafe static void ImGui_ImplWin32_NewFrame()
    {
        ImGui_ImplWin32_Data* bd = ImGui_ImplWin32_GetBackendData();
        System.Diagnostics.Debug.Assert(bd->hWnd != IntPtr.Zero, "Context or backend not initialized? Did you call ImGui_ImplWin32_Init()?");

        ImGuiIOPtr io = ImGui.GetIO();
        NativeMethods.GetClientRect(bd->hWnd, out rect);
        io.DisplaySize.X = rect.Width;
        io.DisplaySize.Y = rect.Height;

        NativeMethods.QueryPerformanceCounter(out long current_time);
        io.DeltaTime = (float)(current_time - bd->Time) / bd->TicksPerSecond;
        bd->Time = current_time;

        ImGui_ImplWin32_UpdateMouseData(io);
        ImGui_ImplWin32_ProcessKeyEventsWorkarounds(io);

        ImGuiMouseCursor mouse_cursor = io.MouseDrawCursor ? ImGuiMouseCursor.None : ImGui.GetMouseCursor();
        if (bd->LastMouseCursor != (int)mouse_cursor)
        {
            bd->LastMouseCursor = (int)mouse_cursor;
            ImGui_ImplWin32_UpdateMouseCursor(io, mouse_cursor);
        }
    }
    public unsafe static ImGuiKey ImGui_ImplWin32_KeyEventToImGuiKey(UIntPtr wParam, IntPtr lParam)
    {
        VK wp = (VK)wParam.ToUInt32();
        if (wp == VK.VK_RETURN && (BitwiseHelpers.HIWORD(lParam) & 0x0100) != 0) return ImGuiKey.KeypadEnter;

        return wp switch

        {
            VK.VK_TAB => ImGuiKey.Tab,
            VK.VK_LEFT => ImGuiKey.LeftArrow,
            VK.VK_RIGHT => ImGuiKey.RightArrow,
            VK.VK_UP => ImGuiKey.UpArrow,
            VK.VK_DOWN => ImGuiKey.DownArrow,
            VK.VK_HOME => ImGuiKey.Home,
            VK.VK_END => ImGuiKey.End,
            VK.VK_INSERT => ImGuiKey.Insert,
            VK.VK_DELETE => ImGuiKey.Delete,
            VK.VK_BACK => ImGuiKey.Backspace,
            VK.VK_SPACE => ImGuiKey.Space,
            VK.VK_RETURN => ImGuiKey.Enter,
            VK.VK_ESCAPE => ImGuiKey.Escape,
            VK.VK_OEM_7 => ImGuiKey.Apostrophe,
            VK.VK_OEM_COMMA => ImGuiKey.Comma,
            VK.VK_OEM_MINUS => ImGuiKey.Minus,
            VK.VK_OEM_PERIOD => ImGuiKey.Period,
            VK.VK_OEM_2 => ImGuiKey.Slash,
            VK.VK_OEM_1 => ImGuiKey.Semicolon,
            VK.VK_OEM_PLUS => ImGuiKey.Equal,
            VK.VK_OEM_4 => ImGuiKey.LeftBracket,
            VK.VK_OEM_5 => ImGuiKey.Backslash,
            VK.VK_OEM_6 => ImGuiKey.RightBracket,
            VK.VK_OEM_3 => ImGuiKey.GraveAccent,
            VK.VK_CAPITAL => ImGuiKey.CapsLock,
            VK.VK_SCROLL => ImGuiKey.ScrollLock,
            VK.VK_PAUSE => ImGuiKey.Pause,
            VK.VK_0 => ImGuiKey._0,
            VK.VK_1 => ImGuiKey._1,
            VK.VK_2 => ImGuiKey._2,
            VK.VK_3 => ImGuiKey._3,
            VK.VK_4 => ImGuiKey._4,
            VK.VK_5 => ImGuiKey._5,
            VK.VK_6 => ImGuiKey._6,
            VK.VK_7 => ImGuiKey._7,
            VK.VK_8 => ImGuiKey._8,
            VK.VK_9 => ImGuiKey._9,
            VK.VK_F1 => ImGuiKey.F1,
            VK.VK_F2 => ImGuiKey.F2,
            VK.VK_F3 => ImGuiKey.F3,
            VK.VK_F4 => ImGuiKey.F4,
            VK.VK_F5 => ImGuiKey.F5,
            VK.VK_F6 => ImGuiKey.F6,
            VK.VK_F7 => ImGuiKey.F7,
            VK.VK_F8 => ImGuiKey.F8,
            VK.VK_F9 => ImGuiKey.F9,
            VK.VK_F10 => ImGuiKey.F10,
            VK.VK_F11 => ImGuiKey.F11,
            VK.VK_F12 => ImGuiKey.F12,
            VK.VK_A => ImGuiKey.A,
            VK.VK_B => ImGuiKey.B,
            VK.VK_C => ImGuiKey.C,
            VK.VK_D => ImGuiKey.D,
            VK.VK_E => ImGuiKey.E,
            VK.VK_F => ImGuiKey.F,
            VK.VK_G => ImGuiKey.G,
            VK.VK_H => ImGuiKey.H,
            VK.VK_I => ImGuiKey.I,
            VK.VK_J => ImGuiKey.J,
            VK.VK_K => ImGuiKey.K,
            VK.VK_L => ImGuiKey.L,
            VK.VK_M => ImGuiKey.M,
            VK.VK_N => ImGuiKey.N,
            VK.VK_O => ImGuiKey.O,
            VK.VK_P => ImGuiKey.P,
            VK.VK_Q => ImGuiKey.Q,
            VK.VK_R => ImGuiKey.R,
            VK.VK_S => ImGuiKey.S,
            VK.VK_T => ImGuiKey.T,
            VK.VK_U => ImGuiKey.U,
            VK.VK_V => ImGuiKey.V,
            VK.VK_W => ImGuiKey.W,
            VK.VK_X => ImGuiKey.X,
            VK.VK_Y => ImGuiKey.Y,
            VK.VK_Z => ImGuiKey.Z,

            _ => ImGuiKey.None,
        };
    }

    static public ImGuiMouseSource ImGui_ImplWin32_GetMouseSourceFromMessageExtraInfo()
    {
        IntPtr extra_info = NativeMethods.GetMessageExtraInfo();
        long extraInfoValue = extra_info.ToInt64();

        if ((extraInfoValue & 0xFFFFFF80) == 0xFF515700)
            return ImGuiMouseSource.Pen;

        if ((extraInfoValue & 0xFFFFFF80) == 0xFF515780)
            return ImGuiMouseSource.TouchScreen;

        return ImGuiMouseSource.Mouse;
    }
    public static int ImGui_ImplWin32_WndProcHandler(IntPtr hwnd, uint msg, UIntPtr wParam, IntPtr lParam)
    {
        if (ImGui.GetCurrentContext() == IntPtr.Zero)
            return 0;
        return ImGui_ImplWin32_WndProcHandlerEx(hwnd, msg, wParam, lParam, ImGui.GetIO());
    }
    
    public unsafe static int ImGui_ImplWin32_WndProcHandlerEx(IntPtr hwnd, uint _msg, UIntPtr wParam, IntPtr lParam, ImGuiIOPtr io)
    {
        WM msg = (WM)_msg;
        if (io.BackendPlatformUserData == IntPtr.Zero)
            return 0;
        ImGui_ImplWin32_Data* bd = ImGui_ImplWin32_GetBackendData();

        switch (msg)
        {
            case WM.WM_MOUSEMOVE:
            case WM.WM_NCMOUSEMOVE:
                {
                    ImGuiMouseSource mouseSource = ImGui_ImplWin32_GetMouseSourceFromMessageExtraInfo();
                    int area = (msg == WM.WM_MOUSEMOVE) ? 1 : 2;
                    bd->MouseHwnd = hwnd;
                    if (bd->MouseTrackedArea != area)
                    {
                        tmeCancel.cbSize = (uint)Marshal.SizeOf<TRACKMOUSEEVENT>();
                        tmeCancel.dwFlags = (uint)TME.TME_CANCEL;
                        tmeCancel.hwndTrack = hwnd;
                        tmeCancel.dwHoverTime = default;

                        tmeTrack.cbSize = (uint)Marshal.SizeOf<TRACKMOUSEEVENT>();
                        tmeTrack.dwFlags = (uint)((area == 2) ? (TME.TME_LEAVE | TME.TME_NONCLIENT) : TME.TME_LEAVE);
                        tmeTrack.hwndTrack = hwnd;
                        tmeTrack.dwHoverTime = default;

                        if (bd->MouseTrackedArea != 0)
                        {
                            NativeMethods.TrackMouseEvent(ref tmeCancel);
                        }

                        NativeMethods.TrackMouseEvent(ref tmeTrack);
                        bd->MouseTrackedArea = area;
                    }


                    point.X = BitwiseHelpers.GET_X_LPARAM(lParam);
                    point.Y = BitwiseHelpers.GET_Y_LPARAM(lParam);

                    if (msg == WM.WM_NCMOUSEMOVE && !NativeMethods.ScreenToClient(hwnd, ref point))
                    {
                        return 0;
                    }

                    io.AddMouseSourceEvent(mouseSource);
                    io.AddMousePosEvent(point.X, point.Y);

                    return 0;
                }

            case WM.WM_MOUSELEAVE:
            case WM.WM_NCMOUSELEAVE:
                {
                    int area = (msg == WM.WM_MOUSELEAVE) ? 1 : 2;
                    if (bd->MouseTrackedArea == area)
                    {
                        if (bd->MouseHwnd == hwnd)
                            bd->MouseHwnd = IntPtr.Zero;
                        bd->MouseTrackedArea = 0;
                        io.AddMousePosEvent(-float.MaxValue, -float.MaxValue);
                    }
                    return 0;
                }
            case WM.WM_DESTROY:
                if (bd->MouseHwnd == hwnd && bd->MouseTrackedArea != 0)
                {

                    tmeCancel.cbSize = (uint)Marshal.SizeOf<TRACKMOUSEEVENT>();
                    tmeCancel.dwFlags = (uint)TME.TME_CANCEL;
                    tmeCancel.hwndTrack = hwnd;
                    tmeCancel.dwHoverTime = default;

                    NativeMethods.TrackMouseEvent(ref tmeCancel);
                    bd->MouseHwnd = IntPtr.Zero;
                    bd->MouseTrackedArea = 0;
                    io.AddMousePosEvent(-float.MaxValue, -float.MaxValue);
                }
                return 0;
            case WM.WM_LBUTTONDOWN:
            case WM.WM_LBUTTONDBLCLK:
            case WM.WM_RBUTTONDOWN:
            case WM.WM_RBUTTONDBLCLK:
            case WM.WM_MBUTTONDOWN:
            case WM.WM_MBUTTONDBLCLK:
            case WM.WM_XBUTTONDOWN:
            case WM.WM_XBUTTONDBLCLK:
                {
                    ImGuiMouseSource mouse_source = ImGui_ImplWin32_GetMouseSourceFromMessageExtraInfo();
                    int button = 0;
                    if (msg == WM.WM_LBUTTONDOWN || msg == WM.WM_LBUTTONDBLCLK) { button = 0; }
                    if (msg == WM.WM_RBUTTONDOWN || msg == WM.WM_RBUTTONDBLCLK) { button = 1; }
                    if (msg == WM.WM_MBUTTONDOWN || msg == WM.WM_MBUTTONDBLCLK) { button = 2; }
                    if (bd->MouseButtonsDown == 0 && NativeMethods.GetCapture() == IntPtr.Zero)
                        NativeMethods.SetCapture(hwnd);
                    bd->MouseButtonsDown |= 1 << button;
                    io.AddMouseSourceEvent(mouse_source);
                    io.AddMouseButtonEvent(button, true);
                    return 0;
                }
            case WM.WM_LBUTTONUP:
            case WM.WM_RBUTTONUP:
            case WM.WM_MBUTTONUP:
            case WM.WM_XBUTTONUP:
                {
                    ImGuiMouseSource mouse_source = ImGui_ImplWin32_GetMouseSourceFromMessageExtraInfo();
                    int button = 0;
                    if (msg == WM.WM_LBUTTONUP) { button = 0; }
                    if (msg == WM.WM_RBUTTONUP) { button = 1; }
                    if (msg == WM.WM_MBUTTONUP) { button = 2; }

                    bd->MouseButtonsDown &= ~(1 << button);
                    if (bd->MouseButtonsDown == 0 && NativeMethods.GetCapture() == hwnd)
                        NativeMethods.ReleaseCapture();

                    io.AddMouseSourceEvent(mouse_source);
                    io.AddMouseButtonEvent(button, false);
                    return 0;
                }
            case WM.WM_MOUSEWHEEL:
                io.AddMouseWheelEvent(0.0f, (float)BitwiseHelpers.GET_WHEEL_DELTA_WPARAM(wParam.ToUInt32()) / (float)120);
                return 0;
            case WM.WM_MOUSEHWHEEL:
                io.AddMouseWheelEvent(-(float)BitwiseHelpers.GET_WHEEL_DELTA_WPARAM(wParam.ToUInt32()) / (float)120, 0.0f);
                return 0;
            case WM.WM_KEYDOWN:
            case WM.WM_KEYUP:
            case WM.WM_SYSKEYDOWN:
            case WM.WM_SYSKEYUP:
                {
                    bool is_key_down = msg == WM.WM_KEYDOWN || msg == WM.WM_SYSKEYDOWN;
                    if (wParam < 256)
                    {
                        ImGui_ImplWin32_UpdateKeyModifiers(io);

                        ImGuiKey key = ImGui_ImplWin32_KeyEventToImGuiKey(wParam, lParam);
                        int vk = (int)wParam.ToUInt32();
                        int scancode = BitwiseHelpers.LOBYTE(BitwiseHelpers.HIWORD(lParam));

                        if (key == ImGuiKey.PrintScreen && !is_key_down)
                            ImGui_ImplWin32_AddKeyEvent(io, key, true, vk, scancode);

                        if (key != ImGuiKey.None)
                            ImGui_ImplWin32_AddKeyEvent(io, key, is_key_down, vk, scancode);

                        if (vk == (int)VK.VK_SHIFT)
                        {
                            if (IsVkDown(VK.VK_LSHIFT) == is_key_down) { ImGui_ImplWin32_AddKeyEvent(io, ImGuiKey.LeftShift, is_key_down, (int)VK.VK_LSHIFT, scancode); }
                            if (IsVkDown(VK.VK_RSHIFT) == is_key_down) { ImGui_ImplWin32_AddKeyEvent(io, ImGuiKey.RightShift, is_key_down, (int)VK.VK_RSHIFT, scancode); }
                        }
                        else if (vk == (int)VK.VK_CONTROL)
                        {
                            if (IsVkDown(VK.VK_LCONTROL) == is_key_down) { ImGui_ImplWin32_AddKeyEvent(io, ImGuiKey.LeftCtrl, is_key_down, (int)VK.VK_LCONTROL, scancode); }
                            if (IsVkDown(VK.VK_RCONTROL) == is_key_down) { ImGui_ImplWin32_AddKeyEvent(io, ImGuiKey.RightCtrl, is_key_down, (int)VK.VK_RCONTROL, scancode); }
                        }
                        else if (vk == (int)VK.VK_MENU)
                        {
                            if (IsVkDown(VK.VK_LMENU) == is_key_down) { ImGui_ImplWin32_AddKeyEvent(io, ImGuiKey.LeftAlt, is_key_down, (int)VK.VK_LMENU, scancode); }
                            if (IsVkDown(VK.VK_RMENU) == is_key_down) { ImGui_ImplWin32_AddKeyEvent(io, ImGuiKey.RightAlt, is_key_down, (int)VK.VK_RMENU, scancode); }
                        }
                    }
                    return 0;
                }
            case WM.WM_SETFOCUS:
            case WM.WM_KILLFOCUS:
                io.AddFocusEvent(msg == WM.WM_SETFOCUS);
                return 0;
            case WM.WM_INPUTLANGCHANGE:
                ImGui_ImplWin32_UpdateKeyboardCodePage(io);
                return 0;
            case WM.WM_CHAR:
                if (NativeMethods.IsWindowUnicode(hwnd))
                {

                    if (wParam.ToUInt32() > 0 && wParam.ToUInt32() < 0x10000)
                        io.AddInputCharacterUTF16((ushort)wParam.ToUInt32());
                }
                else
                {
                    uint codePage = bd->KeyboardCodePage;
                    string multiByteStr = ((char)wParam.ToUInt32()).ToString();

                    string sh = Utils.MultiByteToWC(multiByteStr, (int)codePage);
                    ushort wch = sh[0];
                    io.AddInputCharacter(wch);

                }
                return 0;

            case WM.WM_SETCURSOR:
                if (BitwiseHelpers.LOWORD(lParam.ToInt32()) == 1 && ImGui_ImplWin32_UpdateMouseCursor(io, (ImGuiMouseCursor)bd->LastMouseCursor))
                    return 1;
                return 0;
            case WM.WM_DEVICECHANGE:
                return 0;
        }
        return 0;
    }
}
