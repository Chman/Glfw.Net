namespace Glfw3.Tests
{
    using OpenGL;
    using System;

    /// <summary>
    /// This program is used to test the icon feature.
    /// </summary>
    /// <remarks>
    /// Ported from <c>icon.c</c>.
    /// </remarks>
    class TestIcon : TestBase
    {
        // a simple glfw logo
        static string[] m_Logo =
        {
            "................",
            "................",
            "...0000..0......",
            "...0.....0......",
            "...0.00..0......",
            "...0..0..0......",
            "...0000..0000...",
            "................",
            "................",
            "...000..0...0...",
            "...0....0...0...",
            "...000..0.0.0...",
            "...0....0.0.0...",
            "...0....00000...",
            "................",
            "................"
        };

        static byte[][] m_IconColors =
        {
            new byte[] {   0,   0,   0, 255 }, // black
            new byte[] { 255,   0,   0, 255 }, // red
            new byte[] {   0, 255,   0, 255 }, // green
            new byte[] {   0,   0, 255, 255 }, // blue
            new byte[] { 255, 255, 255, 255 }  // white
        };

        static int m_CurIconColor = 0;

        static void SetIcon(Glfw.Window window, int iconColor)
        {
            var pixels = new byte[16 * 16 * 4];
            var img = new Glfw.Image
            {
                Width = 16,
                Height = 16,
                Pixels = pixels
            };

            int index = 0;
            for (int y = 0; y < img.Width; y++)
            {
                for (int x = 0; x < img.Height; x++)
                {
                    if (m_Logo[y][x] == '0')
                    {
                        pixels[index] = m_IconColors[iconColor][0];
                        pixels[index + 1] = m_IconColors[iconColor][1];
                        pixels[index + 2] = m_IconColors[iconColor][2];
                        pixels[index + 3] = m_IconColors[iconColor][3];
                    }
                    else
                    {
                        pixels[index] = 0;
                        pixels[index + 1] = 0;
                        pixels[index + 2] = 0;
                        pixels[index + 3] = 0;
                    }

                    index += 4;
                }
            }

            Glfw.SetWindowIcon(window, img);
        }

        static void KeyCallback(Glfw.Window window, Glfw.KeyCode key, int scancode, Glfw.InputState state, Glfw.KeyMods mods)
        {
            if (state != Glfw.InputState.Press)
                return;

            switch (key)
            {
                case Glfw.KeyCode.Escape:
                    Glfw.SetWindowShouldClose(window, true);
                    break;
                case Glfw.KeyCode.Space:
                    m_CurIconColor = (m_CurIconColor + 1) % 5;
                    SetIcon(window, m_CurIconColor);
                    break;
                case Glfw.KeyCode.X:
                    Glfw.SetWindowIcon(window, null);
                    break;
            }
        }

        static void Main(string[] args)
        {
            Init();

            Glfw.Window window;

            if (!Glfw.Init())
                Environment.Exit(1);

            window = Glfw.CreateWindow(200, 200, "Window Icon");
            if (!window)
            {
                Glfw.Terminate();
                Environment.Exit(1);
            }

            Glfw.MakeContextCurrent(window);

            Glfw.SetKeyCallback(window, KeyCallback);
            SetIcon(window, m_CurIconColor);

            while (!Glfw.WindowShouldClose(window))
            {
                Gl.Clear(ClearBufferMask.ColorBufferBit);
                Glfw.SwapBuffers(window);
                Glfw.WaitEvents();
            }

            Glfw.DestroyWindow(window);
            Glfw.Terminate();
        }
    }
}
