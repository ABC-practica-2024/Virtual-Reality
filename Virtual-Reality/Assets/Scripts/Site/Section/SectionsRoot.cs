using System;
using System.Collections.Generic;
public class SectionData
{
    public int id { get; set; }
    public string name { get; set; }
    public SouthWest southWest { get; set; }
    public NorthWest northWest { get; set; }
    public NorthEast northEast { get; set; }
    public SouthEast southEast { get; set; }
    public string status { get; set; }
    public DateTime createdAt { get; set; }
    public DateTime updatedAt { get; set; }
    public int siteId { get; set; }
    public List<int> artifactIds { get; set; }
}

public class NorthEast
{
    public int latitude { get; set; }
    public int longitude { get; set; }
}

public class NorthWest
{
    public int latitude { get; set; }
    public int longitude { get; set; }
}

public class SectionsRoot
{
    public bool flag { get; set; }
    public int code { get; set; }
    public string message { get; set; }
    public List<SectionData> data { get; set; }
}

public class SouthEast
{
    public int latitude { get; set; }
    public int longitude { get; set; }
}

public class SouthWest
{
    public int latitude { get; set; }
    public int longitude { get; set; }
}
