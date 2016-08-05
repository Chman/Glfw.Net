namespace Glfw3.Tests
{
    using CommandLine;
    using CommandLine.Text;
    using OpenGL;
    using System;

    /// <summary>
    /// This test creates four windows and clears each in a different color.
    /// </summary>
    /// <remarks>
    /// Ported from <c>windows.c</c>.
    /// </remarks>
    class TestWindows : TestBase
    {
        struct Color
        {
            internal float r, g, b;

            internal Color(float r, float g, float b)
            {
                this.r = r;
                this.g = g;
                this.b = b;
            }
        }

        readonly static string[] m_Titles =
        {
            "Red",
            "Green",
            "Blue",
            "Yellow"
        };

        readonly static Color[] m_Colors =
        {
            new Color(0.95f, 0.32f, 0.11f),
            new Color(0.50f, 0.80f, 0.16f),
            new Color(0.00f, 0.68f, 0.94f),
            new Color(0.98f, 0.74f, 0.04f)
        };

        class Options
        {
            [Option('b', HelpText = "Create decorated windows")]
            public bool Decorated { get; set; }

            [HelpOption(HelpText = "Display this help screen.")]
            public string GetUsage()
            {
                return HelpText.AutoBuild(this,
                  (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
            }
        }

        static void KeyCallback(Glfw.Window window, Glfw.KeyCode key, int scancode, Glfw.InputState state, Glfw.KeyMods mods)
        {
            if (state != Glfw.InputState.Press)
                return;

            switch (key)
            {
                case Glfw.KeyCode.Space:
                    int xpos, ypos;
                    Glfw.GetWindowPos(window, out xpos, out ypos);
                    Glfw.SetWindowPos(window, xpos, ypos);
                    break;

                case Glfw.KeyCode.Escape:
                    Glfw.SetWindowShouldClose(window, true);
                    break;
            }
        }

        static void Main(string[] args)
        {
            Init();

            bool decorated = false;
            bool running = true;
            Glfw.Window[] windows = new Glfw.Window[4];

            var options = new Options();

            if (Parser.Default.ParseArguments(args, options))
                decorated = options.Decorated;

            if (!Glfw.Init())
                Environment.Exit(1);

            Glfw.WindowHint(Glfw.Hint.Decorated, decorated);
            Glfw.WindowHint(Glfw.Hint.Visible, false);

            for (int i = 0; i < 4; i++)
            {
                int left, top, right, bottom;

                windows[i] = Glfw.CreateWindow(200, 200, m_Titles[i]);
                if (!windows[i])
                {
                    Glfw.Terminate();
                    Environment.Exit(1);
                }

                Glfw.SetKeyCallback(windows[i], KeyCallback);

                Glfw.MakeContextCurrent(windows[i]);
                Gl.ClearColor(m_Colors[i].r, m_Colors[i].g, m_Colors[i].b, 1f);

                Glfw.GetWindowFrameSize(windows[i], out left, out top, out right, out bottom);
                Glfw.SetWindowPos(windows[i],
                                 100 + (i & 1) * (200 + left + right),
                                 100 + (i >> 1) * (200 + top + bottom));
            }

            for (int i = 0; i < 4; i++)
                Glfw.ShowWindow(windows[i]);

            while (running)
            {
                for (int i = 0; i < 4; i++)
                {
                    Glfw.MakeContextCurrent(windows[i]);
                    Gl.Clear(ClearBufferMask.ColorBufferBit);
                    Glfw.SwapBuffers(windows[i]);

                    if (Glfw.WindowShouldClose(windows[i]))
                        running = false;
                }

                Glfw.WaitEvents();
            }

            Glfw.Terminate();
        }
    }
}
