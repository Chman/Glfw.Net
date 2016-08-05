using Glfw3;
using OpenGL;
using System;

class TestExample
{
    static void Main(string[] args)
    {
        // If the library isn't in the environment path we need to set it
        Glfw.ConfigureNativesDirectory("../../External/");

        // Initialize the library
        if (!Glfw.Init())
            Environment.Exit(-1);

        // Create a windowed mode window and its OpenGL context
        var window = Glfw.CreateWindow(640, 480, "Hello World");
        if (!window)
        {
            Glfw.Terminate();
            Environment.Exit(-1);
        }

        // Make the window's context current
        Glfw.MakeContextCurrent(window);

        // Loop until the user closes the window
        while (!Glfw.WindowShouldClose(window))
        {
            // Render here
            Gl.Clear(ClearBufferMask.ColorBufferBit);

            // Swap front and back buffers
            Glfw.SwapBuffers(window);

            // Poll for and process events
            Glfw.PollEvents();
        }

        Glfw.Terminate();
    }
}
