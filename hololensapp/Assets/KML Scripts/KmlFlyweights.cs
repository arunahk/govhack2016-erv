using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoordinateFlyweight
{
    // to try to avoid stuff ups, remember in unity y is up
    public float Lon { get; set; } // x
    public float Elev { get; set; } // y
    public float Lat { get; set; } // z
    public bool HasElevation { get; set; }

    // maybe this will help
    //public float x { get { return Lon; } }
    //public float y { get { return Elev; } }
    //public float z { get { return Lat; } }

    public CoordinateFlyweight()
    {
        Lon = 0.0f;
        Elev = 0.0f;
        Lat = 0.0f;
        HasElevation = false;
    }

    public static CoordinateFlyweight Empty = new CoordinateFlyweight();

}

public class RectFlyweight
{
    public double MinLon { get; set; }
    public double MaxLon { get; set; }
    public double MaxLat { get; set; }
    public double MinLat { get; set; }
}

public class OuterRingFlyweight
{
    public List<CoordinateFlyweight> LinearRingFly { get; set; }
}

public class PolygonFlyweight
{
    public string Id { get; set; }
    public OuterRingFlyweight OuterRingFly { get; set; }
}

public class PointFlyweight
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description  { get; set; }
    public CoordinateFlyweight CoordinateFly { get; set; }

    public Dictionary<string, string> Meta { get; set; }

    public PointFlyweight()
    {
        Meta = new Dictionary<string, string>();
    }

}

public class KmlFlyweight
{
    public string Name;
    public List<PointFlyweight> Points;
    public List<PolygonFlyweight> Polygons;

    public KmlFlyweight()
    {
        Name = string.Empty;
        Points = new List<PointFlyweight>();
        Polygons = new List<PolygonFlyweight>();
    }
}

public static class KMLFilter
{
    public static bool IsEntirelyWithin(RectFlyweight rect, List<CoordinateFlyweight> list)
    {
        // Very simple version to test if anypoint is outside the rectangle defined by the bounds
        foreach (var coordinate in list)
        {
            if (coordinate.Lon < rect.MinLon) return false;
            if (coordinate.Lon > rect.MaxLon) return false;
            if (coordinate.Lat < rect.MinLat) return false;
            if (coordinate.Lat > rect.MaxLat) return false;
        }
        return true;
    }

    public static bool IsEntirelyWithin(RectFlyweight rect, CoordinateFlyweight coordinate)
    {
        if (coordinate.Lon < rect.MinLon) return false;
        if (coordinate.Lon > rect.MaxLon) return false;
        if (coordinate.Lat < rect.MinLat) return false;
        if (coordinate.Lat > rect.MaxLat) return false;

        return true;
    }
}