using System;
namespace FuckingEndpoints
{
    [Serializable]
    public class Site
    {
        public int Id;
        public string Name;
    }
    [Serializable]
    public class Section
    {
        public int Id;
        public string Name;
        public float NSLength;
        public float EWWidth;
        public float Depth;
        public float Bearing;
    }
    [Serializable]
    public class Artifact
    {
        public int Id;
        public float NSOffset;
        public float EWOffset;
        public float DepthOffset;
        public float RotX;
        public float RotY;
        public float RotZ;
    }
}