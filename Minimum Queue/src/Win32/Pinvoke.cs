using System.Runtime.InteropServices;
using static ImGuiNET.WinStructs;

namespace ImGuiNET
{
    public static partial class Pinvoke
    {
        public delegate IntPtr WndProc(IntPtr hWnd, uint msg, UIntPtr wParam, IntPtr lParam);

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

        [LibraryImport("user32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool UnregisterClassW(IntPtr lpClassName, IntPtr hInstance);


        [LibraryImport("user32.dll")]
        public static partial void PostQuitMessage(int nExitCode);


        [LibraryImport("user32.dll", EntryPoint = "LoadCursorW")]
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
        public static partial IntPtr GetModuleHandleW(ReadOnlySpan<char> lpModuleName);

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

        [LibraryImport("user32.dll",EntryPoint = "RegisterClassW")]
        public static partial short RegisterClassEx(IntPtr lpwcx);

        [LibraryImport("user32.dll", EntryPoint = "CreateWindowExW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        public static partial IntPtr CreateWindowEx(
            uint dwExStyle,
            IntPtr lpClassName,
            IntPtr lpWindowName,
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

        [LibraryImport("user32.dll", EntryPoint = "DefWindowProcW")]
        public static partial IntPtr DefWindowProc(
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

}