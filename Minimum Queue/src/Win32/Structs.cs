using System.Runtime.InteropServices;

namespace ImGuiNET
{
    public static class WinStructs
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TRACKMOUSEEVENT
        {
            public uint cbSize;
            public uint dwFlags;
            public IntPtr hwndTrack;
            public uint dwHoverTime;

            public static readonly uint Size = (uint)Marshal.SizeOf<TRACKMOUSEEVENT>();
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct WNDCLASSEX
        {
            public uint cbSize;
            public uint style;
            public IntPtr lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            public IntPtr lpszMenuName;
            public IntPtr lpszClassName;
            public IntPtr hIconSm;
            public static readonly uint Size = (uint)Marshal.SizeOf<WNDCLASSEX>();

        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct WindowMessage
        {
            public IntPtr hwnd;
            public uint message;
            public UIntPtr wParam;
            public UIntPtr lParam;
            public uint time;
            public POINT pt;
            public static readonly uint Size = (uint)Marshal.SizeOf<WindowMessage>();
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ImGuiData
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
    }

}