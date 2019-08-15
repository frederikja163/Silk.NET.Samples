using System;
using System.Drawing;
using System.IO;
using Silk.NET.Input;
using Silk.NET.Input.Common;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Common;

namespace Silk.NET.Tutorial11
{
    public static class Program
    {
        private static uint _vertexArray;
        private static uint _vertexBuffer;
        private static uint _shaderProgram;
        private static GL _gl;
        private static IInputContext _input;
        private static IWindow _window;

        private static readonly float[] _vertexBufferData =
        {
            -1.0f, -1.0f, 0.0f,
             1.0f, -1.0f, 0.0f,
             0.0f,  1.0f, 0.0f
        };
        static void Main(string[] args)
        {
            //Setting up the window.
            _window = Window.Create(WindowOptions.Default);
            _window.Load += Load;
            _window.Render += RenderFrame;
            _window.Resize += Resize;
            _window.Run();

            //Clean up after the application is done.
            End();
        }

        private static unsafe void Load()
        {
            //Get the opengl api.
            _gl ??= GL.GetApi();

            //Load and compile temporary individual shaders.
            string vertexSource = File.ReadAllText("Shader.vert");
            uint vertexShader = _gl.CreateShader(GLEnum.VertexShader);
            _gl.CompileShader(vertexShader);
            _gl.ShaderSource(vertexShader, vertexSource);
            string vertexLog = _gl.GetShaderInfoLog(vertexShader);
            if (vertexLog.Length > 0)
            {
                throw new Exception("Vertex shader compiled with error: " + vertexLog);
            }

            string fragmentSource = File.ReadAllText("Shader.frag");
            uint fragmentShader = _gl.CreateShader(GLEnum.FragmentShader);
            _gl.ShaderSource(fragmentShader, fragmentSource);
            _gl.CompileShader(fragmentShader);
            string fragmentLog = _gl.GetShaderInfoLog(fragmentShader);
            if (fragmentLog.Length > 0)
            {
                throw new Exception("Fragment shader compiled with error: " + fragmentLog);
            }

            //Create shader program.
            _shaderProgram = _gl.CreateProgram();
            _gl.AttachShader(_shaderProgram, vertexShader);
            _gl.AttachShader(_shaderProgram, fragmentShader);
            _gl.LinkProgram(_shaderProgram);
            string programLog = _gl.GetProgramInfoLog(_shaderProgram);
            if (programLog.Length > 0)
            {
                throw new Exception("Program failed to link with error: " + programLog);
            }

            //Delete temporary shaders.
            _gl.DetachShader(_shaderProgram, vertexShader);
            _gl.DeleteShader(vertexShader);

            _gl.DetachShader(_shaderProgram, fragmentShader);
            _gl.DeleteShader(fragmentShader);


            //Create a vertex array object.
            _vertexArray = _gl.GenVertexArray();
            _gl.BindVertexArray(_vertexArray);

            //Create a vertex buffer object.
            _vertexBuffer = _gl.GenBuffer();
            _gl.BindBuffer(GLEnum.ArrayBuffer, _vertexBuffer);
            //Give the data to our vertex buffer object.
            fixed(void* vertices = _vertexBufferData)
            {
                _gl.BufferData(GLEnum.ArrayBuffer, (uint)_vertexBufferData.Length * sizeof(float), vertices, GLEnum.StaticDraw);
            }

            _gl.EnableVertexAttribArray(0);
            _gl.VertexAttribPointer(0, 3, GLEnum.Float, false, 0, 0);
        }

        private static void RenderFrame(double obj)
        {
            //Clear the window.
            _gl.Clear((uint)GLEnum.ColorBufferBit);

            //Use our shader.
            _gl.UseProgram(_shaderProgram);

            //Draw triangle.
            _gl.DrawArrays(GLEnum.Triangles, 0, 3);

            //Swap buffer of the window.
            _window.SwapBuffers();
        }
        private static void Resize(Size size)
        {
            //Set the viewport size every time we resize the window.
            _gl.Viewport(0, 0, (uint)size.Width, (uint)size.Height);
        }

        private static void End()
        {
            _gl.DeleteVertexArray(_vertexArray);
            _gl.DeleteBuffer(_vertexBuffer);
        }
    }
}