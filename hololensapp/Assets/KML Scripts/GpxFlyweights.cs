using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MetaFlyweight
{
    public string Name { get; set; }
    public string Link { get; set; }
    public string Author { get; set; }
    public string AuthorLink { get; set; }
    public string Activity { get; set; }
}

public class GpxFlyweight
{
    public string Name;
    public MetaFlyweight Meta;
    public List<PointFlyweight> Points;

    public GpxFlyweight()
    {
        Name = string.Empty;
        Meta = new MetaFlyweight();
        Points = new List<PointFlyweight>();
    }
}
