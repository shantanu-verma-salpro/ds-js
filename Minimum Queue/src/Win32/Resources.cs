using System.Runtime.InteropServices;
using static ImGuiNET.WinConstants;
using static ImGuiNET.Pinvoke;
using static ImGuiNET.WinStructs;
using System.Runtime.CompilerServices;

namespace ImGuiNET
{
    public static class WinResources
    {
        public static readonly IntPtr ClassNamePtr = Marshal.StringToHGlobalUni("ImguiWin32");
        public static readonly IntPtr HandCursorPtr = LoadCursor(IntPtr.Zero, (int)SystemCursors.IDC_HAND);
        public static readonly IntPtr windowClass = Marshal.AllocHGlobal((int)WNDCLASSEX.Size);
        public static readonly IntPtr imguiData = Marshal.AllocHGlobal((int)Marshal.SizeOf<ImGuiData>());
        public static readonly IntPtr hInstance = System.Diagnostics.Process.GetCurrentProcess().Handle;
        public static IntPtr windowHandle = IntPtr.Zero;

        unsafe static WinResources()
        {
            Unsafe.InitBlockUnaligned((byte*)imguiData.ToPointer(), 0, (uint)Marshal.SizeOf<ImGuiData>());
            Unsafe.InitBlockUnaligned((byte*)windowClass.ToPointer(), 0, (uint)Marshal.SizeOf<WNDCLASSEX>());
        }
        public static void ReleaseAllocHGlobal(IntPtr resource)
        {
            Marshal.FreeHGlobal(resource);
        }
    }
}