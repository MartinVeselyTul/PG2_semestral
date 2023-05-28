using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Compute.OpenCL;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;


namespace pg2_project
{
    public class Shaders
    {
        public int ID { get; private set; }
        private bool _disposedValue = false;
        private readonly Dictionary<string, int> _uniformLocations;
        public Shaders(string vsFile, string fsFile)
        {
            ID = GL.CreateProgram();
            int vertexShader = CompileShader(vsFile, ShaderType.VertexShader);
            int fragmentShader = CompileShader(fsFile, ShaderType.FragmentShader);
            Console.WriteLine(GL.GetShaderInfoLog(vertexShader));
            Console.WriteLine(GL.GetShaderInfoLog(fragmentShader));
            int program = LinkShader(new List<int> { vertexShader, fragmentShader });
            GL.DetachShader(ID, vertexShader);
            GL.DetachShader(ID, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
            
            Console.WriteLine("Shader was activated, shader ID: " + ID);
            
            _uniformLocations = new Dictionary<string, int>();

            GL.GetProgram(ID, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);
            // Loop over all the uniforms,
            for (var i = 0; i < numberOfUniforms; i++)
            {
                // get the name of this uniform,
                var key = GL.GetActiveUniform(ID, i, out _, out _);

                // get the location,
                var location = GL.GetUniformLocation(ID, key);

                // and then add it to the dictionary.
                _uniformLocations.Add(key, location);
            }
        }
        
        public void SetVector3(string name, Vector3 data)
        {
            GL.UseProgram(ID);
            GL.Uniform3(_uniformLocations[name], data);
        }

        public void Activate()
        {
            GL.UseProgram(ID);
        }

        public void Deactivate()
        {
            GL.UseProgram(0);
        }

        public void Clear()
        {
            GL.DeleteProgram(ID);
        }
    
        
        ~Shaders()
        {
            GL.DeleteProgram(ID);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                GL.DeleteProgram(ID);

                _disposedValue = true;
            }
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        public void SetMatrix4(string name, Matrix4 matrix)
        {
            int location = GL.GetUniformLocation(ID, name);
            GL.UniformMatrix4(location, true, ref matrix);
        }
        
        private int CompileShader(string sourceFile, ShaderType type)
        {
            int shader = GL.CreateShader(type);
            GL.ShaderSource(shader, File.ReadAllText(sourceFile));
            GL.CompileShader(shader);
            string infoLog = GetShaderInfoLog(shader);
            if (infoLog != System.String.Empty)
                System.Console.WriteLine("Shader info log for \"{0}\":\n{1}", sourceFile, infoLog);

            return shader;
        }
        
        private int LinkShader(List<int> shaderIds)
        {
            foreach (int shader in shaderIds)
            {
                GL.AttachShader(ID, shader);
            }
            GL.LinkProgram(ID);
            string infoLog = GetProgramInfoLog(ID);
            if (infoLog != System.String.Empty)
                System.Console.WriteLine("Program info log:\n{0}", infoLog);
            return 1;
        }

        private string GetShaderInfoLog(int obj)
        {
            return GL.GetShaderInfoLog(obj);
        }

        private string GetProgramInfoLog(int obj)
        {
            return GL.GetProgramInfoLog(obj);
        }
    }
}

