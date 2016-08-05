# Glfw.Net

## Introduction

[GLFW](http://www.glfw.org/) is a free, Open Source, multi-platform library for OpenGL, OpenGL ES and Vulkan application development. It provides a simple, platform-independent API for creating windows, contexts and surfaces, reading input, handling events, etc.

Glfw&#46;Net is a set of C# bindings for GLFW. It's fully-documented and comes with all the original functions except for Vulkan-related features. Some functions and delegates have been slightly changed to take advantage of C# features and make it easier to use.

## Using Glfw.Net

You'll find most of the original tests in [Glfw.Tests](Glfw.Tests), they're considered a good starting place. Below is a port of the [example code](http://www.glfw.org/documentation.html) from the GLFW documentation.

```csharp
using Glfw3;
using OpenGL;
using System;

class Example
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
```

## Requirements
Glfw.Net doesn't have any library dependency but I recommend you use the excellent [OpenGL.Net](https://github.com/luca-piccioni/OpenGL.Net) bindings. As for platform dependencies, Glfw.Net makes use of C# 6.0 features and has been tested with .Net Framework 4.5.2.

To build tests you'll need [OpenGL.Net](https://github.com/luca-piccioni/OpenGL.Net) and [CommandLineParser](https://commandline.codeplex.com/) (both can be installed via NuGet).

## Acknowledgements
This binding is heavily inspired by the work of others, mainly [Chevy Ray Johnston](https://github.com/ChevyRay/GLFW-CS) and [Robbie Lodico](https://github.com/lodico/glfw-net).

## License
Like the original library, this work is released under the [zlib/libpng](LICENSE.txt) license.