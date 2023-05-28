using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Graphics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using OpenTK.Graphics.Wgl;
using System.Diagnostics;
using System.Windows.Input;
using LearnOpenTK.Common;


namespace pg2_project
{
    public class Game : GameWindow
    {
        //chceme dělat 3d pacmana
        
        //zjistit, jak vykreslit více objektů
        //udělat textury + světla
        private Stopwatch _gameTime = new Stopwatch();
        private Shaders _shader = default!; 
        
        private Shaders _lightshader = default!;
        
        private int _frameCount;
        private Camera _camera = default!;
        private Vector2 _lastPos;
        private bool _firstMove = true;
        private double _time;
        bool FScreen = false;
        private int _vaoLamp;
        private Vector3 _objPos = new Vector3(0.0f, 0.0f, 0.0f);

        private readonly Vector3 _lightPos = new Vector3(0f, 5f, 0f);
        //private readonly Vector4 _lightColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        
        private Mesh _lamp = default!;
        private Mesh _bunny = default!;
        private Mesh _woodcrate = default!;
        private Mesh _bench = default!;
        
        private Texture _texture = default!;
        private Texture _texture2 = default!;
        private Texture _texture3 = default!;
        private Texture _textureBench = default!;
        public Game() : base(new GameWindowSettings(), new NativeWindowSettings())
        {
            _gameTime.Start();
            VSync = VSyncMode.On; //nejspíš deklarace vsync, který se tímhle zapne a automaticky nastaví na false
        }
        
        
        private static void DebugCallback(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
        {
            var msg = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(message, length);
            Console.WriteLine($"GL Debug Output: {msg}");
        }
        
        protected override void OnLoad()
        {
            base.OnLoad();
            
            GL.Enable (EnableCap.DepthTest); //zapnutí depth testu - neprůhlednost textur
            
            //GL.Enable (EnableCap.Blend);
            //GL.BlendFunc (BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            
            GL.ClearColor(0.2f, 0.5f, 0.1f, 1.0f);
            
            GL.Enable(EnableCap.DebugOutput);
            
            _shader = new Shaders("../../../../pg2_project/shaders_dir/shader.vert", "../../../../pg2_project/shaders_dir/shader.frag");
            
            //SVĚTLO SHADER
            _lightshader = new Shaders("../../../../pg2_project/shaders_dir/shader.vert", "../../../../pg2_project/shaders_dir/light.frag");

            _texture = Texture.LoadFromFile("../../../../pg2_project/resources/park.jpg");
            _texture2 = Texture.LoadFromFile("../../../../pg2_project/resources/rock_texture.jpg");
            _texture3 = Texture.LoadFromFile("../../../../pg2_project/resources/RGBA_comp.png");
            _textureBench = Texture.LoadFromFile("../../../../pg2_project/resources/bench.jpg");
            
            //_texture.Use(TextureUnit.Texture0);
            //_texture2.Use(TextureUnit.Texture1);

            _lamp = Mesh.Load("../../../../pg2_project/obj_dir/lampPost.obj");
            _woodcrate = Mesh.Load("../../../../pg2_project/obj_dir/plane_tri_vnt.obj");
            _bunny = Mesh.Load("../../../../pg2_project/obj_dir/teapot_tri_vnt.obj");
            _bench = Mesh.Load("../../../../pg2_project/obj_dir/bunny_tri_vnt.obj");
            
            //VAO PRO SVĚTLO LAMPA
            _vaoLamp = GL.GenVertexArray();
            GL.BindVertexArray(_vaoLamp);

            _camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);
            CursorState = CursorState.Grabbed;
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            _time += 4.0 * args.Time;

            GL.ClearColor(Color4.Black);
            
            _shader.Activate();
            
            //GL.Uniform4(GL.GetUniformLocation(_lightshader.ID, "lightColor"), _lightColor[0], _lightColor[1], _lightColor[2], _lightColor[3]);
            //var model = Matrix4.Identity * Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(_time));
            
            _shader.SetMatrix4("model", Matrix4.Identity);
            _shader.SetMatrix4("view", _camera.GetViewMatrix());
            _shader.SetMatrix4("projection", _camera.GetProjectionMatrix());
            
            //_lightshader.SetVector3("lightColor", new Vector3(1.0f, 0.7f, 0.21f));
            _lightshader.SetVector3("lightColor", new Vector3(1.0f, 1f, 1f));
            _lightshader.SetVector3("lightPos", _lightPos);
            _lightshader.SetVector3("viewPos", _camera.Position);
            
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            
            GL.BindVertexArray(_vaoLamp);
            
            _lightshader.Activate();
            
            //var lamp = Matrix4.CreateScale(0.8f);
            //lamp *= Matrix4.CreateTranslation(_lightPos);
            _lightshader.SetMatrix4("model", Matrix4.Identity);
            _lightshader.SetMatrix4("view", _camera.GetViewMatrix());
            _lightshader.SetMatrix4("projection", _camera.GetProjectionMatrix());
            
            //_lamp.Draw();
            //atrix4 model = Matrix4.CreateTranslation(new Vector3(-3.5f, 0.0f, 0.0f)) * Matrix4.CreateScale(0.2f);
            
            _texture.Use(TextureUnit.Texture0);
            _woodcrate.Draw();
            _texture.Unbind();

            _texture2.Use(TextureUnit.Texture0);
            _shader.SetMatrix4("model", Matrix4.CreateTranslation(new Vector3(-5,0,-5)));
            _lamp.Draw();
            _shader.SetMatrix4("model", Matrix4.CreateTranslation(new Vector3(5,0,5)));
            _lamp.Draw();
            _shader.SetMatrix4("model", Matrix4.CreateTranslation(new Vector3(-5,0,5)));
            _lamp.Draw();
            _shader.SetMatrix4("model", Matrix4.CreateTranslation(new Vector3(5,0,-5)));
            _lamp.Draw();
            _texture2.Unbind();
            
            _shader.SetMatrix4("model", Matrix4.CreateTranslation(_objPos) * Matrix4.CreateScale(0.1f) * Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(_time) * 10.5f));
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            _texture3.Use(TextureUnit.Texture0);
            _bunny.Draw();
            _texture3.Unbind();
            GL.Disable(EnableCap.Blend);
            
            _textureBench.Use(TextureUnit.Texture0);
            _shader.SetMatrix4("model",
                Matrix4.CreateTranslation(new Vector3(15f, 7f, 15f)) * Matrix4.CreateScale(0.1f) *
                Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(_time) * 5.5f));
            _bench.Draw();
            _textureBench.Unbind();

            //_bunny.Move(_objPos); --zkoušení pohybu objektu
            
            SwapBuffers();
            _frameCount++;

            double elapsed = _gameTime.Elapsed.TotalSeconds;
            if (elapsed >= 1.0)
            {
                int fps = (int)(_frameCount / elapsed);
                Console.WriteLine("FPS: " + fps);
                _frameCount = 0;
                _gameTime.Restart();
            }
        }
        
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            //mrknout na antialiasing, jak se přidává
            //mrknout jak se dělají komprimované textury
            //knihovna ktx - ktx2 formát - přímo od tvůrců OpenGL - komprimované textury
            base.OnUpdateFrame(e);
            
            if (!IsFocused) // Check to see if the window is focused
            {
                return;
            }
            
            //_objPos = vector3(f,f,f)
            
            
            if (_objPos[0] > 5.0f)
            {
                _objPos[0] -= 0.5f;
            }
            
            if (_objPos[0] < -5f)
            {
                _objPos[0] += 0.5f;
            }
            

            //sem jde přidat něco ve smyslu 
            //var pohyb += 1 a udělá se to po každym framu
            
            var input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            const float cameraSpeed = 0.8f;
            const float sensitivity = 0.1f;
            
            if (input.IsKeyDown(Keys.W))
            {
                _camera.Position += _camera.Front * cameraSpeed * (float)e.Time; // Forward
            }

            if (input.IsKeyDown(Keys.S))
            {
                _camera.Position -= _camera.Front * cameraSpeed * (float)e.Time; // Backwards
            }
            if (input.IsKeyDown(Keys.A))
            {
                _camera.Position -= _camera.Right * cameraSpeed * (float)e.Time; // Left
            }
            if (input.IsKeyDown(Keys.D))
            {
                _camera.Position += _camera.Right * cameraSpeed * (float)e.Time; // Right
            }
            if (input.IsKeyDown(Keys.Space))
            {
                _camera.Position += _camera.Up * cameraSpeed * (float)e.Time; // Up
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                _camera.Position -= _camera.Up * cameraSpeed * (float)e.Time; // Down
            }

            // Get the mouse state
            var mouse = MouseState;

            if (_firstMove) // This bool variable is initially set to true.
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                // Calculate the offset of the mouse position
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);

                // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
                _camera.Yaw += deltaX * sensitivity;
                _camera.Pitch -= deltaY * sensitivity; // Reversed since y-coordinates range from bottom to top
            }
        }
        
        
        
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            //nastaveni pocatku souradnic do stredu zobrazeneho okna (nastavujem v mainu)
            // Get the size of the client area
            int width = ClientSize.X;
            int height = ClientSize.Y;

            // Calculate the center point
            int centerX = width / 2;
            int centerY = height / 2;

            // Set the viewport to center on the center point
            GL.Viewport(centerX - width / 2, centerY - height / 2, width, height);
        }
        
        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
            
            _objPos[0] += 0.1f;
            _objPos[2] += 0.1f;

            switch (e.Key)
            {
                case Keys.Escape:
                    _shader.Clear();
                    this.Close();
                    return;
                case Keys.V:
                    VSync = VSync == VSyncMode.On ? VSyncMode.Off : VSyncMode.On;
                    break;
                case Keys.F:
                    FScreen = FScreen == true ? false : true;
                    if (FScreen == true)
                    {
                        WindowState = WindowState.Fullscreen;
                    }
                    else
                    {
                        WindowState = WindowState.Normal;
                    }
                    break;
            }
        }
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            _camera.Fov -= e.OffsetY;
        }
    }
}

