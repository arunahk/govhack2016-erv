using UnityEngine;
using System.Collections;
using System.Xml.Linq;
using System;
using System.IO;
using System.Collections.Generic;

public class MyKmlReader
{
    private RectFlyweight _wgs84Region;
    private string _ns;
    private string _filename;

    enum coordState
    {
        Lon,
        Lat,
        Elev
    };

    public MyKmlReader()
    {
        // The region is limited to the maps tiles BX23 and BX24 
        _wgs84Region = new RectFlyweight();
        _ns = string.Empty;
        _filename = string.Empty;
    }

    private CoordinateFlyweight ParseSingleCoordinateString(string coordinateString)
    {
        string[] values = coordinateString.Split(',');
        if (values.Length >= 2)
        {
            try
            {
                CoordinateFlyweight result = new CoordinateFlyweight();
                result.Lon = float.Parse(values[0]);
                result.Lat = float.Parse(values[1]);
                if (values.Length >= 3)
                {
                    result.Elev = float.Parse(values[2]);
                    result.HasElevation = true;
                }
                return result;
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }
        return CoordinateFlyweight.Empty;
    }

    private CoordinateFlyweight ParseCoordinateElement(XElement current)
    {
        if (current != null)
        {
            string coordinateString = current.Value ?? "";
            if(coordinateString != string.Empty)
            {
                return ParseSingleCoordinateString(coordinateString);
            }
        }

        return CoordinateFlyweight.Empty;
    }

    private string ParseId(XElement current)
    {
        string result = string.Empty;

        var idAttr = current.Attribute("id");
        if (idAttr != null)
        {
            result = idAttr.ToString();
        }

        return result;
    }


    private List<CoordinateFlyweight> ParseCoordinates(XElement current)
    {
        List<CoordinateFlyweight> result = new List<CoordinateFlyweight>();

        if (current != null)
        {
            string coordinateString = current.Value ?? "";
            if (coordinateString != string.Empty)
            {
                string[] lines = coordinateString.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

                foreach(var line in lines)
                {
                    if (line.TrimStart() != string.Empty)
                    {                    
                        string[] coordinates = coordinateString.Split(' ');
                        foreach(var coord in coordinates)
                        {
                            CoordinateFlyweight c = ParseSingleCoordinateString(coord);
                            if(c != CoordinateFlyweight.Empty)
                            {
                                result.Add(c);
                            }
                        }
                    }
                }
            }

            // Unity works with open rings, so remove the last element if it is a duplicate
            if (result.Count > 1)
            {
                if (result[0].Lat == result[result.Count - 1].Lat && result[0].Lon == result[result.Count - 1].Lon && result[0].Elev == result[result.Count- 1].Elev)
                {
                    result.RemoveAt(result.Count - 1);
                }
            }
        }
        return result;
    }

    private CoordinateFlyweight ParsePoint(XElement current)
    {
        if(current != null)
        {
            return ParseCoordinateElement(current.Element(_ns + "coordinates"));
        }
        return CoordinateFlyweight.Empty;
    }

    private List<CoordinateFlyweight> ParsePoints(XElement current)
    {
        if (current != null)
        {
            return ParseCoordinates(current.Element(_ns + "coordinates"));
        }
        return new List<CoordinateFlyweight>();
    }

    private List<CoordinateFlyweight> ParseLinearRing(XElement current)
    {
        if(current != null)
        {
            return ParsePoints(current);
        }
        return new List<CoordinateFlyweight>();
    }

    private OuterRingFlyweight ParseOuterRing(XElement current)
    {
        OuterRingFlyweight fly = new OuterRingFlyweight();
        if(current != null)
        {
            fly.LinearRingFly = ParseLinearRing(current.Element(_ns + "LinearRing"));
        }
        return fly;
    }

    private KmlFlyweight ParseDocument(XElement current)
    {
        KmlFlyweight KmlFly = new KmlFlyweight();
        KmlFly.Name = _filename;

        var placemarks = current.Elements(_ns + "Placemark");
        foreach (var placemark in placemarks)
        {
            var pt = placemark.Element(_ns + "Point");
            if (pt != null)
            {
                PointFlyweight point = new PointFlyweight();
                point.Id = ParseId(placemark);
                point.Name = placemark.Element(_ns + "name").Value ?? "";
                point.Description = placemark.Element(_ns + "description").Value ?? "";
                point.CoordinateFly = ParsePoint(placemark.Element(_ns + "Point"));
                var metaItems = placemark.Elements(_ns + "MetaData");
                foreach(var item in metaItems)
                {
                    string name = item.Attribute("name").Value ?? "";
                    if (name != string.Empty)
                    {
                        point.Meta.Add(name, item.Value);
                    }
                }

                if(KMLFilter.IsEntirelyWithin(_wgs84Region, point.CoordinateFly))
                {
                    KmlFly.Points.Add(point);
                }

            }

            var ls = placemark.Element(_ns + "LineString");

            if (ls != null)
            {

            }

            var lr = placemark.Element(_ns + "LineRing");

            if (lr != null)
            {
            }

            var pg = placemark.Element(_ns + "Polygon");

            if (pg != null)
            {
                PolygonFlyweight poly = new PolygonFlyweight();
                poly.Id = ParseId(placemark);
                poly.OuterRingFly = ParseOuterRing(pg.Element(_ns + "outerBoundaryIs"));
                if (KMLFilter.IsEntirelyWithin(_wgs84Region, poly.OuterRingFly.LinearRingFly))
                {
                    KmlFly.Polygons.Add(poly);
                }
            }

            var mg = placemark.Element(_ns + "MultiGeometry");

            if (mg != null)
            {

            }

            // MultiGeometry
            // Model
            // gx:Track
            // gx:MultiTrack

        }
        return KmlFly;
    }

    public KmlFlyweight Load(string filename, RectFlyweight rect)
    {
        _filename = filename;
        _wgs84Region = rect;

        XElement doc = null;

        try
        {
            //doc = XElement.Load(_filename);
            
            TextAsset textAsset = (TextAsset)Resources.Load(_filename);

            TextReader tReader = new StringReader(textAsset.text);


            doc = XElement.Load(tReader);

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
            if(docTag != null)
            {
                return ParseDocument(docTag);
            }
            else
            {
                return ParseDocument(doc);
            }
        }

        return new KmlFlyweight(); // just an empty placeholder
    }
}
