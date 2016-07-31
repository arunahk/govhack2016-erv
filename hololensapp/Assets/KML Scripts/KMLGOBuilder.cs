using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KMLGOBuilder : MonoBehaviour
{
    [Header(" [Material to use when no material found]")]
    public Material DefaultMeshMaterial = null;
    public Material DefaultLineMaterial = null;

    public float DefaultHeight = 30.0f;

    [Header(" [Default object to draw for a placemarker]")]
    public GameObject DefaultPlacemarker = null;

    [Header(" [Default object to draw for a sensor]")]
    public GameObject DefaultSensor = null;

    [Header(" [Default object to draw for a building]")]
    public GameObject DefaultBuilding = null;

    [Header(" [The parent object containing the map]")]
    public GameObject ParentModel = null;

    [Header(" [The object that will contain the mesh, (set in the editor)]")]
    public MyGravityMesh GravityMesh = null;

    private RectFlyweight _wgs84Region = new RectFlyweight();
    private Bounds _unityRegion = new Bounds();
    private static float maximumElevation = 1000.0f;
    private static float markerDelta = 0.03f; // fiddle factor to avoid z-fighting
    private static float ringDelta = 0.02f; // fiddle factor to avoid z-fighting
    private static float meshDelta = 0.01f; // fiddle factor to avoid z-fighting

    public List<HeatMapPoint> HeatMapPoints = new List<HeatMapPoint>();

    public KMLGOBuilder()
    {
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    private Vector3 PositionOnGround(Vector3 pos, float delta)
    {
        // Cast a ray from the parent object (at elevation 0) upwards until it hits the terrain
        RaycastHit hit;

        Vector3 result = new Vector3(pos.x, maximumElevation, pos.z);

        if (Physics.Raycast(result, -Vector3.up, out hit))
        {
            result.y = maximumElevation - hit.distance + delta;
        }
        else
        {
            result.y = DefaultHeight;
        }

        return result;
    }

    // Having problems with converting the world space vector to
    // a vector relative to the 'map' so have moved the map to 0,0,0
    // and disabled this code.
    private Vector3 UnityWorldSpaceToLocalSpace(GameObject obj, Vector3 vector)
    {
        // return obj.transform.InverseTransformPoint(vector); // this doesn't do what I want
        return vector;
    }

    // A hacky way to convert WGS84 to Unity units
    // We know that the unity bounds and the coordinate rect occupy the same space
    // (because the unity regions was generated from the WGS84 data and then transformed)
    // So we can just make a conversion from one relative to the other
    private Vector3 WGS84ToUnityWorldSpace(CoordinateFlyweight fly)
    {
        double rangeLon = _wgs84Region.MaxLon - _wgs84Region.MinLon;
        double rangeLat = _wgs84Region.MaxLat - _wgs84Region.MinLat;
        double relativeLon = (fly.Lon - _wgs84Region.MinLon);
        double relativeLat = (fly.Lat - _wgs84Region.MinLat);
        double ratioLon = relativeLon / rangeLon;
        double ratioLat = relativeLat / rangeLat;
        double unityX = (ratioLon * _unityRegion.size.x) + _unityRegion.min.x;
        double unityZ = (ratioLat * _unityRegion.size.z) + _unityRegion.min.z;
        return new Vector3() { x = (float)unityX, y = fly.Elev, z = (float)unityZ };
    }

    public Vector2[] GetVector2Array(GameObject go, PolygonFlyweight pf)
    {
        List<Vector2> result = new List<Vector2>();
        foreach (var p in pf.OuterRingFly.LinearRingFly)
        {
            // Vector3 vector3 = go.transform.InverseTransformPoint(WGS84ToUnityWorldSpace(p));
            Vector3 vector3 = WGS84ToUnityWorldSpace(p);
            result.Add(new Vector2() { x = vector3.x, y = vector3.z });
        }

        return result.ToArray();
    }

    private void BuildMarker(GameObject go, PointFlyweight pf, float area, bool onGround)
    {
        GameObject pointObject = new GameObject();
        pointObject.name = "Placemarker";
        if (pf.Id != string.Empty)
        {
            pointObject.name = pf.Id;
        }
        pointObject.transform.SetParent(go.transform, false);
        pointObject.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, .0f);
        pointObject.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        pointObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        ProcessMetaData(pointObject, pf);

        GameObject placemarker = null;

        GroundSensor gs = pointObject.GetComponent<GroundSensor>();
        if (gs != null)
        {
            placemarker = GameObject.Instantiate(DefaultSensor);
            placemarker.name = "Sensor";

            //print("ADD SENSOR "+pf.Name);
        }
        else
        {
            Building building = pointObject.GetComponent<Building>();
            if (building != null)
            {
                placemarker = GameObject.Instantiate(DefaultBuilding);
                placemarker.name = "Building";
                
            }
            else
            {
                placemarker = GameObject.Instantiate(DefaultPlacemarker);
                placemarker.name = "Default";

                if (placemarker.GetComponent<aBuilding>() != null)
                {
                    placemarker.GetComponent<aBuilding>().BuildingName = pf.Name;
                    placemarker.GetComponent<aBuilding>().BuildingDescription = pf.Description;
                    //print(pf.Name);
                }

                //print("ADD BUILDING");
                //Check if it contains 


            }
        }

        if (placemarker != null)
        {
            // The placemarker will have its transformation replaced. So make sure that
            // any modificiation to the placemarker transformation is done in child elements, not the root.
            placemarker.transform.SetParent(pointObject.transform, false);
            placemarker.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, .0f);
            placemarker.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            Vector3 ws = WGS84ToUnityWorldSpace(pf.CoordinateFly);
            Vector3 ls = UnityWorldSpaceToLocalSpace(placemarker, ws);
            if (onGround)
            {
                ls = PositionOnGround(ls, markerDelta);
            }
            placemarker.transform.localPosition = ls;

            if(gs != null)
            {
                //collect data for heat map
                HeatMapPoints.Add(new HeatMapPoint() { Position = ls, SensorData = gs });
            }

        }
    }

    private void BuildMarker(GameObject go, PolygonFlyweight pg, bool onGround)
    {
        List<Vector3> vertexList = new List<Vector3>();

        float minLat = 0.0f;
        float maxLat = 0.0f;
        float minLon = 0.0f;
        float maxLon = 0.0f;
        bool first = true;

        if (pg.OuterRingFly.LinearRingFly.Count > 0)
        {
            foreach (var point in pg.OuterRingFly.LinearRingFly)
            {
                if(first)
                {
                    minLat = point.Lat;
                    maxLat = point.Lat;
                    minLon = point.Lon;
                    maxLon = point.Lon;
                    first = false;
                }

                // linq would be more efficient
                if (point.Lat < minLat) minLat = point.Lat;
                if (point.Lat > maxLat) maxLat = point.Lat;
                if (point.Lon < minLon) minLon = point.Lon;
                if (point.Lon > maxLon) maxLon = point.Lon;
            }
        }
        
        if(!first)
        {
            float centreLon = minLon + ((maxLon - minLon) / 2.0f);
            float centreLat = minLat + ((maxLat - minLat) / 2.0f);
            float area = (maxLon - minLon) * (maxLat - minLat);
            PointFlyweight pf = new PointFlyweight() { CoordinateFly = new CoordinateFlyweight() { Lat = centreLat, Lon = centreLon }, Description = "Marker", Name = "Center" };
            BuildMarker(go, pf, area, true);
        }
    }

    private void BuildRing(GameObject go, PolygonFlyweight pg, bool onGround)
    {
        // Add a new game object for each set of triangles
        GameObject obj = new GameObject();
        obj.name = "Polygon - Line";
        obj.transform.SetParent(go.transform, false);
        obj.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, .0f);
        obj.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        obj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        LineRenderer renderer = obj.AddComponent(typeof(LineRenderer)) as LineRenderer;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.useWorldSpace = false;
        renderer.useLightProbes = false;

        List<Vector3> vertexList = new List<Vector3>();

        if (pg.OuterRingFly.LinearRingFly.Count > 0)
        {
            foreach (var point in pg.OuterRingFly.LinearRingFly)
            {
                Vector3 ws = WGS84ToUnityWorldSpace(point);
                Vector3 ls = UnityWorldSpaceToLocalSpace(obj, ws);
                if (onGround)
                {
                    ls = PositionOnGround(ls, ringDelta);
                }
                vertexList.Add(ls);
            }

            // add last element again to complete ring
            vertexList.Add(vertexList[0]);
        }

        Vector3[] positions = vertexList.ToArray();

        // may need to sanity check the positions, as they can be the same
        renderer.SetVertexCount(positions.Length);
        renderer.SetPositions(positions);
        renderer.SetWidth(0.1f, 0.1f);
        renderer.sharedMaterial = DefaultLineMaterial;
    }


    private void ProcessMetaData(GameObject go, PointFlyweight pf)
    {
        bool hasSensorData = false;
        bool hasBuildingData = false;
        string Date = string.Empty;
        string Time = string.Empty;
        float Magnitude = 0.0f;
        float PGAVertical = 0.0f;
        float PGAHorizontal1 = 0.0f;
        float PGAHorizontal2 = 0.0f;

        if(pf.Meta.ContainsKey("Data Type"))
        {
            string dataType = pf.Meta["Data Type"];
            if(string.Compare(dataType, "Sensor",true) == 0)
            {
                hasSensorData = true;
            }
        }
        else
        {
            hasBuildingData = true;
        }

        foreach(var item in pf.Meta)
        {
            switch(item.Key)
            {
                case "Earthquake Date (UT)":
                    {
                        Date = item.Value;
                    }
                    break;
                case "Time (UT)":
                    {
                        Time = item.Value;
                    }
                    break;
                case "Magnitude":
                    {
                        Magnitude = float.Parse(item.Value);
                    }
                    break;
                case "PGA Vertical (%g)":
                    {
                        PGAVertical = float.Parse(item.Value);
                    }
                    break;
                case "PGA Horiz_1 (%g)":
                    {
                        PGAHorizontal1 = float.Parse(item.Value);
                    }
                    break;
                case "PGA Horiz_2 (%g)":
                    {
                        PGAHorizontal2 = float.Parse(item.Value);
                    }
                    break;
                default:
                    break;
            }
        }

        if(hasSensorData)
        {
            GroundSensor sensor = go.AddComponent<GroundSensor>();
            sensor.Date = Date;
            sensor.Time = Time;
            sensor.Magnitude = Magnitude;
            sensor.PGAVertical = PGAVertical;
            sensor.PGAHorizontal1 = PGAHorizontal1;
            sensor.PGAHorizontal2 = PGAHorizontal2;

        }
        else if(hasBuildingData)
        {
            Building building = go.AddComponent<Building>();
        }
    }

    private void ParseKmlFly(GameObject go, KmlFlyweight fly)
    {
        foreach(var point in fly.Points)
        {
            BuildMarker(go, point, 1.0f, true);
        }

        foreach(var poly in fly.Polygons)
        {
            BuildMarker(go, poly, true);
            //BuildRing(go, poly, true);
            //BuildMesh(go, poly, true);
        }
    }

    public void PopulateGOKML(KmlFlyweight KmlFly, RectFlyweight wgs84Region, Bounds unityRegion)
    {
        if(KmlFly != null)
        {
            _wgs84Region = wgs84Region;
            _unityRegion = unityRegion;

            if (ParentModel != null)
            {
                ParseKmlFly(ParentModel, KmlFly);
                if(GravityMesh != null)
                {
                    GravityMesh.PopulateMesh(HeatMapPoints);
                }
                SetActive();
            }
        }
    }

    public void SetActive()
    {
        if(ParentModel != null)
        {
            ParentModel.SetActive(true);
        }
    }
}

public class HeatMapPoint
{
    public Vector3 Position;
    public GroundSensor SensorData;
}
