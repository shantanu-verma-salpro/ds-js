using System.Runtime.CompilerServices;

namespace ImGuiNET
{
    public static class WinMacros
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GET_X_LPARAM(IntPtr lp) => (short)(lp.ToInt32());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GET_Y_LPARAM(IntPtr lp) => (short)((lp.ToInt32() >> 16));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte HIBYTE(ushort wValue) => (byte)(wValue >> 8);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort HIWORD(uint dwValue) => (ushort)(dwValue >> 16);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort HIWORD(IntPtr dwValue) => (ushort)(dwValue.ToInt64() >> 16);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort HIWORD(UIntPtr dwValue) => (ushort)(dwValue.ToUInt64() >> 16);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IS_INTRESOURCE(IntPtr ptr) => (ptr.ToInt64() >> 16) == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte LOBYTE(ushort wValue) => (byte)wValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort LOWORD(uint dwValue) => (ushort)dwValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort LOWORD(IntPtr dwValue) => (ushort)dwValue.ToInt64();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort LOWORD(UIntPtr dwValue) => (ushort)dwValue.ToUInt64();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GET_WHEEL_DELTA_WPARAM(IntPtr wParam) => (short)HIWORD(wParam);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GET_WHEEL_DELTA_WPARAM(uint wParam) => (short)HIWORD(wParam);
    }


}