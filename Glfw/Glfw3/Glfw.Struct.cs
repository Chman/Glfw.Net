namespace Glfw3
{
    using System;
    using System.Runtime.InteropServices;

    public static partial class Glfw
    {
        /// <summary>
        /// Opaque cursor object.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct Cursor : IEquatable<Cursor>
        {
            /// <summary>
            /// <para>Null cursor pointer.</para>
            /// </summary>
            public static readonly Cursor None = new Cursor(IntPtr.Zero);

            /// <summary>
            /// Pointer to an internal GLFWcursor.
            /// </summary>
            public IntPtr Ptr;

            internal Cursor(IntPtr ptr)
            {
                Ptr = ptr;
            }

            public override bool Equals(object obj)
            {
                if (obj is Cursor)
                    return Equals((Cursor)obj);
                return false;
            }

            public bool Equals(Cursor obj) => Ptr == obj.Ptr;

            public override string ToString() => Ptr.ToString();

            public override int GetHashCode() => Ptr.GetHashCode();

            public static bool operator ==(Cursor a, Cursor b) => a.Equals(b);

            public static bool operator !=(Cursor a, Cursor b) => !a.Equals(b);

            public static implicit operator bool(Cursor obj) => obj.Ptr != IntPtr.Zero;
        }

        /// <summary>
        /// This describes the gamma ramp for a monitor.
        /// </summary>
        /// <seealso cref="GetGammaRamp(Monitor)"/>
        /// <seealso cref="SetGammaRamp(Monitor, GammaRamp)"/>
        [StructLayout(LayoutKind.Sequential)]
        public struct GammaRamp
        {
            /// <summary>
            /// An array of value describing the response of the red channel.
            /// </summary>
            [MarshalAs(UnmanagedType.LPArray)]
            public ushort[] Red;

            /// <summary>
            /// An array of value describing the response of the green channel.
            /// </summary>
            [MarshalAs(UnmanagedType.LPArray)]
            public ushort[] Green;

            /// <summary>
            /// An array of value describing the response of the blue channel.
            /// </summary>
            [MarshalAs(UnmanagedType.LPArray)]
            public ushort[] Blue;

            /// <summary>
            /// The number of elements in each array.
            /// </summary>
            public uint Size => (uint)Math.Min(Red.Length, Math.Min(Green.Length, Blue.Length));
        }

        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct InternalGammaRamp
        {
            /// <summary>
            /// An array of value describing the response of the red channel.
            /// </summary>
            internal ushort* Red;
            
            /// <summary>
            /// An array of value describing the response of the green channel.
            /// </summary>
            internal ushort* Green;

            /// <summary>
            /// An array of value describing the response of the blue channel.
            /// </summary>
            internal ushort* Blue;

            /// <summary>
            /// The number of elements in each array.
            /// </summary>
            internal uint Size;
        }

        /// <summary>
        /// Image data.
        /// </summary>
        /// <seealso cref=""/>
        public struct Image
        {
            /// <summary>
            /// The width, in pixels, of this image.
            /// </summary>
            public int Width;

            /// <summary>
            /// The height, in pixels, of this image.
            /// </summary>
            public int Height;

            /// <summary>
            /// The pixel data of this image, arranged left-to-right, top-to-bottom.
            /// </summary>
            public byte[] Pixels;
        }

        /// <summary>
        /// Internal image data.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct InternalImage
        {
            /// <summary>
            /// The width, in pixels, of this image.
            /// </summary>
            internal int Width;

            /// <summary>
            /// The height, in pixels, of this image.
            /// </summary>
            internal int Height;

            /// <summary>
            /// The pixel data of this image, arranged left-to-right, top-to-bottom.
            /// </summary>
            internal IntPtr Pixels;
        }

        /// <summary>
        /// <para>Opaque monitor object.</para>
        /// </summary>
        /// <seealso cref="GetMonitors"/>
        /// <seealso cref="GetPrimaryMonitor"/>
        [StructLayout(LayoutKind.Sequential)]
        public struct Monitor : IEquatable<Monitor>
        {
            /// <summary>
            /// <para>Null monitor pointer.</para>
            /// </summary>
            public static readonly Monitor None = new Monitor(IntPtr.Zero);
            
            /// <summary>
            /// Pointer to an internal GLFWmonitor.
            /// </summary>
            public IntPtr Ptr;

            internal Monitor(IntPtr ptr)
            {
                Ptr = ptr;
            }

            public override bool Equals(object obj)
            {
                if (obj is Monitor)
                    return Equals((Monitor)obj);

                return false;
            }

            public bool Equals(Monitor obj) => Ptr == obj.Ptr;

            public override string ToString() => Ptr.ToString();

            public override int GetHashCode() => Ptr.GetHashCode();

            public static bool operator ==(Monitor a, Monitor b) => a.Equals(b);

            public static bool operator !=(Monitor a, Monitor b) => !a.Equals(b);

            public static implicit operator bool(Monitor obj) => obj.Ptr != IntPtr.Zero;
        }

        /// <summary>
        /// This describes a single video mode.
        /// </summary>
        /// <seealso cref="GetVideoMode(Monitor)"/>
        /// <seealso cref="GetVideoModes(Monitor)"/>
        [StructLayout(LayoutKind.Sequential)]
        public struct VideoMode : IEquatable<VideoMode>
        {
            /// <summary>
            /// The width, in screen coordinates, of the video mode.
            /// </summary>
            public int Width;

            /// <summary>
            /// The height, in screen coordinates, of the video mode.
            /// </summary>
            public int Height;

            /// <summary>
            /// The bit depth of the red channel of the video mode.
            /// </summary>
            public int RedBits;

            /// <summary>
            /// The bit depth of the green channel of the video mode.
            /// </summary>
            public int GreenBits;

            /// <summary>
            /// The bit depth of the blue channel of the video mode.
            /// </summary>
            public int BlueBits;

            /// <summary>
            /// The refresh rate, in Hz, of the video mode.
            /// </summary>
            public int RefreshRate;

            public override bool Equals(object obj)
            {
                if (obj is VideoMode)
                    return Equals((VideoMode)obj);

                return false;
            }

            public bool Equals(VideoMode obj)
            {
                return obj.Width == Width
                    && obj.Height == Height
                    && obj.RedBits == RedBits
                    && obj.GreenBits == GreenBits
                    && obj.BlueBits == BlueBits
                    && obj.RefreshRate == RefreshRate;
            }

            public override string ToString()
            {
                return string.Format("VideoMode(width: {0}, height: {1}, redBits: {2}, greenBits: {3}, blueBits: {4}, refreshRate: {5})",
                    Width.ToString(),
                    Height.ToString(),
                    RedBits.ToString(),
                    GreenBits.ToString(),
                    BlueBits.ToString(),
                    RefreshRate.ToString()
                );
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 23 + Width.GetHashCode();
                    hash = hash * 23 + Height.GetHashCode();
                    hash = hash * 23 + RedBits.GetHashCode();
                    hash = hash * 23 + GreenBits.GetHashCode();
                    hash = hash * 23 + BlueBits.GetHashCode();
                    hash = hash * 23 + RefreshRate.GetHashCode();
                    return hash;
                }
            }

            public static bool operator ==(VideoMode a, VideoMode b) => a.Equals(b);

            public static bool operator !=(VideoMode a, VideoMode b) => !a.Equals(b);
        }

        /// <summary>
        /// <para>Opaque window object.</para>
        /// </summary>
        /// <seealso cref="CreateWindow(int, int, string, Monitor, Window)"/>
        [StructLayout(LayoutKind.Sequential)]
        public struct Window : IEquatable<Window>
        {
            /// <summary>
            /// <para>Null window pointer.</para>
            /// </summary>
            public static readonly Window None = new Window(IntPtr.Zero);

            /// <summary>
            /// Pointer to an internal GLFWwindow.
            /// </summary>
            public IntPtr Ptr;

            internal Window(IntPtr ptr)
            {
                Ptr = ptr;
            }

            public override bool Equals(object obj)
            {
                if (obj is Window)
                    return Equals((Window)obj);

                return false;
            }

            public bool Equals(Window obj) => Ptr == obj.Ptr;

            public override string ToString() => Ptr.ToString();

            public override int GetHashCode() => Ptr.GetHashCode();

            public static bool operator ==(Window a, Window b) => a.Equals(b);

            public static bool operator !=(Window a, Window b) => !a.Equals(b);

            public static implicit operator bool(Window obj) => obj.Ptr != IntPtr.Zero;
        }
    }
}
