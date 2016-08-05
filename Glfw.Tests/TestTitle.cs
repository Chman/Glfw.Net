namespace Glfw3.Tests
{
    using OpenGL;
    using System;

    /// <summary>
    /// This test sets a UTF-8 window title.
    /// </summary>
    /// <remarks>
    /// Ported from <c>title.c</c>.
    /// </remarks>
    class TestTitle : TestBase
    {
        static void FramebufferSizeCallback(Glfw.Window window, int width, int height)
        {
            Gl.Viewport(0, 0, width, height);
        }

        static void Main(string[] args)
        {
            Init();

            Glfw.Window window;

            if (!Glfw.Init())
                Environment.Exit(1);
            
            window = Glfw.CreateWindow(400, 400, "English 日本語 русский язык 官話");

            if (!window)
            {
                Glfw.Terminate();
                Environment.Exit(1);
            }

            Glfw.MakeContextCurrent(window);
            Glfw.SwapInterval(1);

            Glfw.SetFramebufferSizeCallback(window, FramebufferSizeCallback);

            while (!Glfw.WindowShouldClose(window))
            {
                Gl.Clear(ClearBufferMask.ColorBufferBit);
                Glfw.SwapBuffers(window);
                Glfw.WaitEvents();
            }

            Glfw.Terminate();
        }
    }
}
