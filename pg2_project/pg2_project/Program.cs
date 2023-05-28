using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using pg2_project;

namespace pg2_project
{
    public static class Program
    {
        private static void Main()
        {
            using Game game = new Game();
            game.Title = "pg2_project";
            game.Size = new Vector2i(1000, 1000);
            game.Run();
        }
    }
}