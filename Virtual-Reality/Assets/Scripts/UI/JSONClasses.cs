using System;
using System.Collections.Generic;

[Serializable]
public class LoginRequest
{
    public string username;
    public string password;
}

// Adjusted API response class for `getSites`
[Serializable]
public class SiteResponse
{
    public bool flag;
    public int code;
    public string message;
    public List<Site> data;  // Adjusted to hold a list of Site objects
}

[Serializable]
public class Site
{
    public int id;
    public string title;
    public string description;
    public Coordinate centralCoordinate;
    public List<PerimeterCoordinate> perimeterCoordinates;
    public string status;
    public int mainArchaeologistID;
    public List<int> sectionsIds;
    public List<int> archaeologistsIds;
}

[Serializable]
public class SectionArrayResponse
{
    public List<Section> sections; // Adjust to match your JSON structure
}

// Adjusted API response class for `getSections{ID}`
[Serializable]
public class SectionResponse
{
    public bool flag;
    public int code;
    public string message;
    public Section data;
}

[Serializable]
public class Section
{
    public int id;
    public string name;
    public Coordinate southWest;
    public Coordinate northWest;
    public Coordinate northEast;
    public Coordinate southEast;
    public string status;
    public DateTime createdAt;
    public DateTime updatedAt;
    public int siteId;
    public List<int> artifactIds;
}

[Serializable]
public class Coordinate
{
    public float latitude;
    public float longitude;
}

[Serializable]
public class PerimeterCoordinate : Coordinate
{
    public int orderIndex;
}