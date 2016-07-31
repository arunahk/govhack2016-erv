using UnityEngine;
using System.Collections;
using System.Xml.Linq;
using System;
using System.Collections.Generic;

public class MyGpxReader
{
    private string _ns;
    private string _filename;

    enum coordState
    {
        Lon,
        Lat,
        Elev
    };

    public MyGpxReader()
    {
        // The region is limited to the maps tiles BX23 and BX24 
        _ns = string.Empty;
        _filename = string.Empty;
    }

    private GpxFlyweight ParseDocument(XElement current)
    {
        GpxFlyweight GpxFly = new GpxFlyweight();
        GpxFly.Name = _filename;

        var meta = current.Element(_ns + "metadata");
        if(meta != null)
        {
            {
                var name = meta.Element(_ns + "name");
                if(name != null)
                {
                    GpxFly.Meta.Name = name.Value;
                }
            }

            var author = meta.Element(_ns + "author");
            if(author != null)
            {
                var name = author.Element(_ns + "name");
                if(name != null)
                {
                    GpxFly.Meta.Author = name.Value; 
                }
                var link = author.Element(_ns + "link");
                if(link != null)
                {
                    GpxFly.Meta.AuthorLink = link.Attribute("href").Value ?? "";
                }
            }
            {
                var link = meta.Element(_ns + "link");
                if (link != null)
                {
                    GpxFly.Meta.Link = link.Attribute("href").Value ?? "";
                }
            }
        }

        var trk = current.Element(_ns + "trk");
        if (trk != null)
        {
            var activity = trk.Element(_ns + "type");
            if (activity != null)
            {
                GpxFly.Meta.Activity = activity.Value;
            }

            var trkseg = trk.Element(_ns + "trkseg");
            if (trkseg != null)
            {
                var points = trkseg.Elements(_ns + "trkpt");

                int i = 0;
                foreach(var point in points)
                {
                    CoordinateFlyweight fly = new CoordinateFlyweight();
                    string lon = point.Attribute("lon").Value ?? "0.0";
                    string lat = point.Attribute("lat").Value ?? "0.0";
                    fly.Lon = float.Parse(lon);
                    fly.Lat = float.Parse(lat);
                    var ele = point.Element(_ns + "ele");
                    if(ele != null)
                    {
                        fly.Elev = float.Parse(ele.Value);
                        fly.HasElevation = true;
                    }
                    GpxFly.Points.Add(new PointFlyweight() { Name = i.ToString(), Description = "trkpnt", CoordinateFly = fly, Id = i.ToString() });
                    i++;
                }
            }
        }
        return GpxFly;
    }

    public GpxFlyweight Load(string filename)
    {
        _filename = filename;

        XElement doc = null;

        try
        {
            doc = XElement.Load(_filename);
            _ns = "{" + doc.GetDefaultNamespace().ToString() + "}";
        }
        catch (Exception e)
        {
            string error = String.Format("Coudn't load document: {0} error {1}", _filename, e.ToString());
            Debug.Log(error);
            doc = null;
        }

        if (doc != null)
        {
            var docTag = doc.Element(_ns + "Document");
            if (docTag != null)
            {
                return ParseDocument(docTag);
            }
            else
            {
                return ParseDocument(doc);
            }
        }

        return new GpxFlyweight(); // just an empty placeholder
    }
}
