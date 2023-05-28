using System.Globalization;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace pg2_project;

public class Mesh
{
    private int vao;
    private int vbo;
    private int ebo;
    private int vertexCount;
    private int indexCount;
    private Vertex[] vertices;

    public Mesh(Vertex[] vertices, uint[] indices)
    {
        // Vytvoření vertex array objektu
        vao = GL.GenVertexArray();
        GL.BindVertexArray(vao);

        // Vytvoření bufferu pro vrcholy
        vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, Vertex.Size * vertices.Length, vertices, BufferUsageHint.StaticDraw);
        
        // Nastavení atributů vrcholu
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex.Size, 0);

        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Vertex.Size, Vector3.SizeInBytes);

        GL.EnableVertexAttribArray(2);
        GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, Vertex.Size, Vector3.SizeInBytes * 2);
        
        // Vytvoření bufferu pro indexy
        ebo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * indices.Length, indices, BufferUsageHint.StaticDraw);
        
        // Uložení počtu vrcholů a indexů
        vertexCount = vertices.Length;
        indexCount = indices.Length;
    }

    public void Draw()
    {
        GL.BindVertexArray(vao);
        GL.DrawElements(PrimitiveType.Triangles, indexCount, DrawElementsType.UnsignedInt, 0);
    }
    
    public void Dispose()
    {
        GL.DeleteBuffer(vbo);
        GL.DeleteBuffer(ebo);
        GL.DeleteVertexArray(vao);
    }
    public static bool LoadObj(string path, out List<Vector3> out_vertices, out List<Vector2> out_uvs, out List<Vector3> out_normals)
        {
            List<int> vertexIndices = new List<int>();
            List<int> uvIndices = new List<int>();
            List<int> normalIndices = new List<int>();
            List<Vector3> temp_vertices = new List<Vector3>();
            List<Vector2> temp_uvs = new List<Vector2>();
            List<Vector3> temp_normals = new List<Vector3>();

            out_vertices = new List<Vector3>();
            out_uvs = new List<Vector2>();
            out_normals = new List<Vector3>();

            StreamReader file = new StreamReader(path);
            if (file == null)
            {
                Console.WriteLine("Impossible to open the file !\n");
                return false;
            }

            while (true)
            {
                string line = file.ReadLine();
                if (line == null)
                {
                    break;
                }

                string[] tokens = line.Split(' ');
                if (tokens[0] == "v")
                {
                    Vector3 vertex = new Vector3(float.Parse(tokens[1], CultureInfo.InvariantCulture), float.Parse(tokens[2], CultureInfo.InvariantCulture), float.Parse(tokens[3], CultureInfo.InvariantCulture));
                    temp_vertices.Add(vertex);
                }
                else if (tokens[0] == "vt")
                {
                    Vector2 uv = new Vector2(float.Parse(tokens[1], CultureInfo.InvariantCulture), float.Parse(tokens[2], CultureInfo.InvariantCulture));
                    temp_uvs.Add(uv);
                }
                else if (tokens[0] == "vn")
                {
                    Vector3 normal = new Vector3(float.Parse(tokens[1], CultureInfo.InvariantCulture), float.Parse(tokens[2], CultureInfo.InvariantCulture), float.Parse(tokens[3], CultureInfo.InvariantCulture));
                    temp_normals.Add(normal);
                }
                else if (tokens[0] == "f")
                {
                    for (int i = 1; i < tokens.Length; i++)
                    {
                        string[] faceTokens = tokens[i].Split('/');
                        vertexIndices.Add(int.Parse(faceTokens[0]));
                        uvIndices.Add(int.Parse(faceTokens[1]));
                        normalIndices.Add(int.Parse(faceTokens[2]));
                    }
                }
            }

            for (int i = 0; i < vertexIndices.Count; i++)
            {
                int vertexIndex = vertexIndices[i];
                Vector3 vertex = temp_vertices[vertexIndex - 1];
                out_vertices.Add(vertex);
            }

            for (int i = 0; i < uvIndices.Count; i++)
            {
                int uvIndex = uvIndices[i];
                Vector2 uv = temp_uvs[uvIndex - 1];
                out_uvs.Add(uv);
            }

            int normalIndex = 0;
            for (int i = 0; i < normalIndices.Count; i++)
            {
                normalIndex = normalIndices[i];
                Vector3 normal = temp_normals[normalIndex - 1];
                out_normals.Add(normal);
            }

            return true;
        }

    public static Mesh Load(string path)
    {
        LoadObj(path, out List<Vector3> _vertices, out List<Vector2> _tex, out List<Vector3> _normals);
        Vertex[] vertexy = new Vertex[_vertices.Count];
        uint[] indexy = new uint[_vertices.Count];
        
        for (int i = 0; i < _vertices.Count; i++)
        {
            Vertex vertex = new Vertex(_vertices[i], _normals[i], _tex[i]);
            vertexy[i] = vertex;
            indexy[i] = (uint)i;
            //udělat pole vertexů 0-35, to naplnit a použít jako verteces
        }
    
        return new Mesh(vertexy, indexy);
    }

}