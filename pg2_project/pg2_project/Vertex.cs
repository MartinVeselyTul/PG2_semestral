using OpenTK.Mathematics;

namespace pg2_project;

public struct Vertex
{
    public Vector3 Position;
    public Vector3 Normal;
    public Vector2 TexCoord;

    public Vertex(Vector3 position, Vector3 normal, Vector2 texCoord)
    {
        Position = position;
        Normal = normal;
        TexCoord = texCoord;
    }
    
    public const int Size = (3 + 3 + 2) * 4; // 3 souřadnice pozice, 3 souřadnice normály, 2 souřadnice texturovacích souřadnic (každá 4 bajty)
    
    public void SetPosition(Vector3 position)
    {
        Position += position;
    }
}