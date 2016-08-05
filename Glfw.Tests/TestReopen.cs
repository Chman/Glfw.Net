namespace Glfw3.Tests
{
    using OpenGL;
    using System;

    /// <summary>
    /// This test closes and re-opens the GLFW window every five seconds, alternating between
    /// windowed and full screen mode. It also times and logs opening and closing actions and
    /// attempts to separate user initiated window closing from its own.
    /// </summary>
    /// <remarks>
    /// Ported from <c>reopen.c</c>.
    /// </remarks>
    class TestReopen : TestBase
    {
        static void FramebufferSizeCallback(Glfw.Window window, int width, int height)
        {
            Gl.Viewport(0, 0, width, height);
        }

        static void WindowCloseCallback(Glfw.Window window)
        {
            Log("Close callback triggered");
        }

        static void KeyCallback(Glfw.Window window, Glfw.KeyCode key, int scancode, Glfw.InputState state, Glfw.KeyMods mods)
        {
            if (state != Glfw.InputState.Press)
                return;

            switch (key)
            {
                case Glfw.KeyCode.Q:
                case Glfw.KeyCode.Escape:
                    Glfw.SetWindowShouldClose(window, true);
                    break;
            }
        }

        static Glfw.Window OpenWindow(int width, int height, Glfw.Monitor monitor)
        {
            double b;
            Glfw.Window window;

            b = Glfw.GetTime();

            window = Glfw.CreateWindow(width, height, "Window Re-opener", monitor, Glfw.Window.None);
            if (!window)
                return Glfw.Window.None;

            Glfw.MakeContextCurrent(window);
            Glfw.SwapInterval(1);

            Glfw.SetFramebufferSizeCallback(window, FramebufferSizeCallback);
            Glfw.SetWindowCloseCallback(window, WindowCloseCallback);
            Glfw.SetKeyCallback(window, KeyCallback);

            if (monitor)
            {
                Log("Opening full screen window on monitor {0} took {1} seconds",
                       Glfw.GetMonitorName(monitor),
                       Glfw.GetTime() - b);
            }
            else
            {
                Log("Opening regular window took {0} seconds",
                       Glfw.GetTime() - b);
            }

            return window;
        }

        static void CloseWindow(Glfw.Window window)
        {
            double b = Glfw.GetTime();
            Glfw.DestroyWindow(window);
            Log("Closing window took {0} seconds", Glfw.GetTime() - b);
        }

        static void Main(string[] args)
        {
            Init();

            int count = 0;
            Glfw.Window window;
            var rand = new Random();

            if (!Glfw.Init())
                Environment.Exit(1);

            for (;;)
            {
                int width, height;
                var monitor = Glfw.Monitor.None;

                if (count % 2 == 0)
                {
                    var monitors = Glfw.GetMonitors();
                    monitor = monitors[rand.Next() % monitors.Length];
                }

                if (monitor)
                {
                    var mode = Glfw.GetVideoMode(monitor);
                    width = mode.Width;
                    height = mode.Height;
                }
                else
                {
                    width = 640;
                    height = 480;
                }

                window = OpenWindow(width, height, monitor);
                if (!window)
                {
                    Glfw.Terminate();
                    Environment.Exit(1);
                }

                Gl.MatrixMode(MatrixMode.Projection);
                Gl.Ortho(-1f, 1f, -1f, 1f, 1f, -1f);
                Gl.MatrixMode(MatrixMode.Modelview);

                Glfw.SetTime(0.0);

                while (Glfw.GetTime() < 5.0)
                {
                    Gl.Clear(ClearBufferMask.ColorBufferBit);

                    Gl.PushMatrix();
                    Gl.Rotate((float)Glfw.GetTime() * 100f, 0f, 0f, 1f);
                    Gl.Rect(-0.5f, -0.5f, 1f, 1f);
                    Gl.PopMatrix();

                    Glfw.SwapBuffers(window);
                    Glfw.PollEvents();

                    if (Glfw.WindowShouldClose(window))
                    {
                        CloseWindow(window);
                        Log("User closed window");

                        Glfw.Terminate();
                        return;
                    }
                }

                Log("Closing window");
                CloseWindow(window);

                count++;
            }
        }
    }
}
