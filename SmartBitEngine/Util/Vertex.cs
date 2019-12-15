using OpenTK;

namespace DumBitEngine.Core.Util
{
    public struct Vertex
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector2 texCoord;

        public Vertex(Vector3 position, Vector3 normal, Vector2 texCoord)
        {
            this.position = position;
            this.normal = normal;
            this.texCoord = texCoord;
        }
    }
}