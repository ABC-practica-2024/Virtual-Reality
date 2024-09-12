using System.Collections.Generic;
// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class CentralCoordinate
{
    public int latitude { get; set; }
    public int longitude { get; set; }
}

public class SiteData
{
    public int id { get; set; }
    public string title { get; set; }
    public string description { get; set; }
    public CentralCoordinate centralCoordinate { get; set; }
    public List<PerimeterCoordinate> perimeterCoordinates { get; set; }
    public string status { get; set; }
    public int mainArchaeologistID { get; set; }
    public List<int> sectionsIds { get; set; }
    public List<int> archaeologistsIds { get; set; }
}

public class PerimeterCoordinate
{
    public int latitude { get; set; }
    public int longitude { get; set; }
    public int orderIndex { get; set; }
}

public class SitesRoot
{
    public bool flag { get; set; }
    public int code { get; set; }
    public string message { get; set; }
    public List<SiteData> data { get; set; }
}

