namespace Glfw3.Tests
{
    using OpenGL;
    using System;
    using System.Threading;

    /// <summary>
    /// This test is intended to verify that posting of empty events works.
    /// </summary>
    /// <remarks>
    /// Ported from <c>empty.c</c>.
    /// </remarks>
    class TestEmpty : TestBase
    {
        static volatile bool running = true;

        static void ThreadMain()
        {
            while (running)
            {
                Thread.Sleep(1000);
                Glfw.PostEmptyEvent();
            }
        }

        static void KeyCallback(Glfw.Window window, Glfw.KeyCode key, int scancode, Glfw.InputState state, Glfw.KeyMods mods)
        {
            if (key == Glfw.KeyCode.Escape && state == Glfw.InputState.Press)
                Glfw.SetWindowShouldClose(window, true);
        }

        static void Main(string[] args)
        {
            Init();

            var rand = new Random();

            if (!Glfw.Init())
                Environment.Exit(1);

            var window = Glfw.CreateWindow(640, 480, "Empty Event Test");
            if (!window)
            {
                Glfw.Terminate();
                Environment.Exit(1);
            }

            Glfw.MakeContextCurrent(window);
            Glfw.SetKeyCallback(window, KeyCallback);

            var thread = new Thread(new ThreadStart(ThreadMain));

            if (thread == null)
            {
                Log("Failed to create secondary thread");

                Glfw.Terminate();
                Environment.Exit(1);
            }

            thread.Start();

            while (running)
            {
                int width, height;
                float r = rand.Next(), g = rand.Next(), b = rand.Next();
                float l = (float)Math.Sqrt(r * r + g * g + b * b);

                Glfw.GetFramebufferSize(window, out width, out height);

                Gl.Viewport(0, 0, width, height);
                Gl.ClearColor(r / l, g / l, b / l, 1f);
                Gl.Clear(ClearBufferMask.ColorBufferBit);
                Glfw.SwapBuffers(window);

                Glfw.WaitEvents();

                if (Glfw.WindowShouldClose(window))
                    running = false;
            }

            Glfw.HideWindow(window);
            thread.Join();
            Glfw.DestroyWindow(window);

            Glfw.Terminate();
        }
    }
}
