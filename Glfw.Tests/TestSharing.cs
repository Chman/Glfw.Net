namespace Glfw3.Tests
{
    using OpenGL;
    using System;

    /// <summary>
    /// This program is used to test sharing of objects between contexts.
    /// </summary>
    /// <remarks>
    /// Ported from <c>sharing.c</c>.
    /// </remarks>
    class TestSharing : TestBase
    {
        static readonly int m_Width = 400;
        static readonly int m_Height = 400;
        static readonly int m_Offset = 50;

        static Random rand = new Random();

        static Glfw.Window[] windows = new Glfw.Window[2];

        static void KeyCallback(Glfw.Window window, Glfw.KeyCode key, int scancode, Glfw.InputState state, Glfw.KeyMods mods)
        {
            if (state == Glfw.InputState.Press && key == Glfw.KeyCode.Escape)
                Glfw.SetWindowShouldClose(window, true);
        }

        static Glfw.Window OpenWindow(string title, Glfw.Window share, int posX, int posY)
        {
            Glfw.Window window;

            Glfw.WindowHint(Glfw.Hint.Visible, false);
            window = Glfw.CreateWindow(m_Width, m_Height, title, Glfw.Monitor.None, share);

            if (!window)
                return Glfw.Window.None;

            Glfw.MakeContextCurrent(window);
            Glfw.SwapInterval(1);
            Glfw.SetWindowPos(window, posX, posY);
            Glfw.ShowWindow(window);

            Glfw.SetKeyCallback(window, KeyCallback);

            return window;
        }

        static uint CreateTexture()
        {
            var pixels = new byte[256 * 256];

            uint texture = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2d, texture);

            for (int y = 0; y < 256; y++)
            {
                for (int x = 0; x < 256; x++)
                    pixels[y * 256 + x] = (byte)(rand.Next() % 256);
            }

            Gl.TexImage2D(TextureTarget.Texture2d, 0, Gl.LUMINANCE, 256, 256, 0, PixelFormat.Luminance, PixelType.UnsignedByte, pixels);
            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, Gl.LINEAR);
            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, Gl.LINEAR);

            return texture;
        }

        static void DrawQuad(uint texture)
        {
            int width, height;
            Glfw.GetFramebufferSize(Glfw.GetCurrentContext(), out width, out height);

            Gl.Viewport(0, 0, width, height);

            Gl.MatrixMode(MatrixMode.Projection);
            Gl.LoadIdentity();
            Gl.Ortho(0f, 1f, 0f, 1f, 0f, 1f);

            Gl.Enable(EnableCap.Texture2d);
            Gl.BindTexture(TextureTarget.Texture2d, texture);
            Gl.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, Gl.MODULATE);

            Gl.Begin(PrimitiveType.Quads);

            Gl.TexCoord2(0f, 0f);
            Gl.Vertex2(0f, 0f);

            Gl.TexCoord2(1f, 0f);
            Gl.Vertex2(1f, 0f);

            Gl.TexCoord2(1f, 1f);
            Gl.Vertex2(1f, 1f);

            Gl.TexCoord2(0f, 1f);
            Gl.Vertex2(0f, 1f);

            Gl.End();
        }

        static void Main(string[] args)
        {
            Init();

            int x, y, width, height;
            uint texture;

            if (!Glfw.Init())
                Environment.Exit(1);

            windows[0] = OpenWindow("First", Glfw.Window.None, m_Offset, m_Offset);
            if (!windows[0])
            {
                Glfw.Terminate();
                Environment.Exit(1);
            }

            // This is the one and only time we create a texture
            // It is created inside the first context, created above
            // It will then be shared with the second context, created below
            texture = CreateTexture();

            Glfw.GetWindowPos(windows[0], out x, out y);
            Glfw.GetWindowSize(windows[0], out width, out height);

            // Put the second window to the right of the first one
            windows[1] = OpenWindow("Second", windows[0], x + width + m_Offset, y);
            if (!windows[1])
            {
                Glfw.Terminate();
                Environment.Exit(1);
            }

            // Set drawing color for both contexts
            Glfw.MakeContextCurrent(windows[0]);
            Gl.Color3(0.6f, 0f, 0.6f);
            Glfw.MakeContextCurrent(windows[1]);
            Gl.Color3(0.6f, 0.6f, 0f);

            Glfw.MakeContextCurrent(windows[0]);

            while (!Glfw.WindowShouldClose(windows[0]) &&
                   !Glfw.WindowShouldClose(windows[1]))
            {
                Glfw.MakeContextCurrent(windows[0]);
                DrawQuad(texture);

                Glfw.MakeContextCurrent(windows[1]);
                DrawQuad(texture);

                Glfw.SwapBuffers(windows[0]);
                Glfw.SwapBuffers(windows[1]);

                Glfw.WaitEvents();
            }

            Glfw.Terminate();
        }
    }
}
