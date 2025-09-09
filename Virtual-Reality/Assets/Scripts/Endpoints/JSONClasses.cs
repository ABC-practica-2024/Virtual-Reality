using System;
using System.Collections.Generic;
namespace FuckingEndpoints
{
    [Serializable]
    public class LoginRequest
    {
        public string username;
        public string password;
    }
    [Serializable]
    public class Site
    {
        public int id;
        public string name;
    }
    [Serializable]
    public class SiteSections
    {
        public List<Section> sections;
    }
    [Serializable]
    public class Section
    {
        public int id;
        public string name;
    }
    [Serializable]
    public class SectionData
    {
        public float length;
        public float width;
        public float depth;
        public float bearing;
    }
    [Serializable]
    public class SectionArtifacts
    {
        public List<Artifact> artifacts;
    }
    [Serializable]
    public class Artifact
    {
        public float ns_offset;
        public float ew_offset;
        public float depth_offset;
        public float rot_x;
        public float rot_y;
        public float rot_z;
    }
}