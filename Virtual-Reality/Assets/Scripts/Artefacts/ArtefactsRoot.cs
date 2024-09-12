using System;
using System.Collections.Generic;

public class ArtifactDimension
{
    public int length { get; set; }
    public int width { get; set; }
    public int height { get; set; }
}

public class ArtifactPosition
{
    public int latitude { get; set; }
    public int longitude { get; set; }
    public int depth { get; set; }
}

public class ArtifactRotation
{
    public int pitch { get; set; }
    public int yaw { get; set; }
    public int roll { get; set; }
}

public class ArtefactData
{
    public int id { get; set; }
    public ArtifactDimension artifactDimension { get; set; }
    public ArtifactPosition artifactPosition { get; set; }
    public ArtifactRotation artifactRotation { get; set; }
    public string label { get; set; }
    public string category { get; set; }
    public bool analysisComplete { get; set; }
    public string thumbnail { get; set; }
    public int sectionId { get; set; }
    public int archeologistId { get; set; }
    public DateTime createdAt { get; set; }
    public DateTime updatedAt { get; set; }
}

public class ArtefactsRoot
{
    public bool flag { get; set; }
    public int code { get; set; }
    public string message { get; set; }
    public List<ArtefactData> data { get; set; }
}