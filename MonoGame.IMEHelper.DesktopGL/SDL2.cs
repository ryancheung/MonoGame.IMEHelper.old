using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MonoGame.IMEHelper
{
    internal static class Sdl
    {
        public static IntPtr NativeLibrary = GetNativeLibrary();

        private static IntPtr GetNativeLibrary()
        {
            var ret = IntPtr.Zero;

            // Load bundled library
            var assemblyLocation = Path.GetDirectoryName(typeof(Sdl).Assembly.Location);
            if (CurrentPlatform.OS == OS.Windows && Environment.Is64BitProcess)
                ret = FuncLoader.LoadLibrary(Path.Combine(assemblyLocation, "x64/SDL2.dll"));
            else if (CurrentPlatform.OS == OS.Windows && !Environment.Is64BitProcess)
                ret = FuncLoader.LoadLibrary(Path.Combine(assemblyLocation, "x86/SDL2.dll"));
            else if (CurrentPlatform.OS == OS.Linux && Environment.Is64BitProcess)
                ret = FuncLoader.LoadLibrary(Path.Combine(assemblyLocation, "x64/libSDL2-2.0.so.0"));
            else if (CurrentPlatform.OS == OS.Linux && !Environment.Is64BitProcess)
                ret = FuncLoader.LoadLibrary(Path.Combine(assemblyLocation, "x86/libSDL2-2.0.so.0"));
            else if (CurrentPlatform.OS == OS.MacOSX)
                ret = FuncLoader.LoadLibrary(Path.Combine(assemblyLocation, "libSDL2-2.0.0.dylib"));

            // Load system library
            if (ret == IntPtr.Zero)
            {
                if (CurrentPlatform.OS == OS.Windows)
                    ret = FuncLoader.LoadLibrary("SDL2.dll");
                else if (CurrentPlatform.OS == OS.Linux)
                    ret = FuncLoader.LoadLibrary("libSDL2-2.0.so.0");
                else
                    ret = FuncLoader.LoadLibrary("libSDL2-2.0.0.dylib");
            }

            // Welp, all failed, PANIC!!!
            if (ret == IntPtr.Zero)
                throw new Exception("Failed to load SDL library.");

            return ret;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Rectangle
        {
            public int X;
            public int Y;
            public int Width;
            public int Height;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_sdl_starttextinput();
        public static d_sdl_starttextinput StartTextInput = FuncLoader.LoadFunction<d_sdl_starttextinput>(NativeLibrary, "SDL_StartTextInput");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_sdl_stoptextinput();
        public static d_sdl_stoptextinput StopTextInput = FuncLoader.LoadFunction<d_sdl_stoptextinput>(NativeLibrary, "SDL_StopTextInput");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_sdl_settextinputrect(ref Rectangle rect);
        public static d_sdl_settextinputrect SetTextInputRect = FuncLoader.LoadFunction<d_sdl_settextinputrect>(NativeLibrary, "SDL_SetTextInputRect");
    }
}
