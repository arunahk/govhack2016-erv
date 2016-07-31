using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GpxGOBuilder : MonoBehaviour
{
    [Header(" [Material to use when no material found]")]
    public Material DefaultRunMaterial = null;
    public Material DefaultRideMaterial = null;

    public float DefaultHeight = 30.0f;

    [Header(" [The parent object containing the map]")]
    public GameObject ParentModel = null;

    private RectFlyweight _wgs84Region = new RectFlyweight();
    private Bounds _unityRegion = new Bounds();
    private static float maximumElevation = 1000.0f;
    private static float lineDelta = 0.02f; // fiddle factor to avoid z-fighting

    public GpxGOBuilder()
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

    private void ParseGpxFly(GameObject go, GpxFlyweight fly, bool onGround)
    {
        // Add a new game object for each set of triangles
        GameObject obj = new GameObject();
        obj.name = "Track";
        obj.transform.SetParent(go.transform, false);
        obj.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, .0f);
        obj.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        obj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        LineRenderer renderer = obj.AddComponent(typeof(LineRenderer)) as LineRenderer;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.useWorldSpace = false;
        renderer.useLightProbes = false;

        List<Vector3> vertexList = new List<Vector3>();

        foreach (var point in fly.Points)
        {
            Vector3 ws = WGS84ToUnityWorldSpace(point.CoordinateFly);
            Vector3 ls = UnityWorldSpaceToLocalSpace(obj, ws);
            if (onGround)
            {
                // use the delta to avoid z-fighting
                ls = PositionOnGround(ls, lineDelta);
            }
            vertexList.Add(ls);
        }

        Vector3[] positions = vertexList.ToArray();

        // may need to sanity check the positions, as they can be the same
        renderer.SetVertexCount(positions.Length);
        renderer.SetPositions(positions);
        renderer.SetWidth(0.1f, 0.1f);

        switch(fly.Meta.Activity)
        {
            case "Run":
                renderer.sharedMaterial = DefaultRunMaterial;
                break;
            case "Ride":
            default:
                renderer.sharedMaterial = DefaultRideMaterial;
                break;

        }
    }

    public void PopulateGOGpx(GpxFlyweight GpxFly, RectFlyweight wgs84Region, Bounds unityRegion)
    {
        if (GpxFly != null)
        {
            _wgs84Region = wgs84Region;
            _unityRegion = unityRegion;

            if (ParentModel != null)
            {
                ParseGpxFly(ParentModel, GpxFly, true);
                SetActive();
            }
        }
    }

    public void SetActive()
    {
        if (ParentModel != null)
        {
            ParentModel.SetActive(true);
        }
    }
}
