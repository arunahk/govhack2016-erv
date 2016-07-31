using UnityEngine;
using System.Collections;

public class GpxModel : MonoBehaviour
{
    [Header(" [Connnect this in the editor]")]
    public GpxGOBuilder GoBuilder = null;

    [Header(" [Set to the kml filename]")]
    public string Filename = null;

    [Header(" [Contains the extents of the map area in Unity space]")]
    public GameObject BX23 = null;
    public GameObject BX24 = null;

    private MyGpxReader GpxReader = null;
    private GpxFlyweight GpxFly = null;

    // The region is limited to the maps tiles BX23 and BX24 
    // The structure specifies the extents of the map are in WGS-84 space
    RectFlyweight WGS84MapRegion = new RectFlyweight() { MinLon = 172.25456342f, MinLat = -43.76461329f, MaxLon = 172.85169953f, MaxLat = -43.44279919f };

    // Use this for initialization
    void Start()
    {
        // A more complete solution would have this done in a background thread       
        GpxReader = new MyGpxReader();

        // Having trouble with the conversion, so moved map to 0,0,0 and hardcoded the transformation
        // it is a hackathon after all :-(
        Bounds bounds = new Bounds() { min = new Vector3(0.0f, 0.0f, 0.0f), max = new Vector3(48.0f, 0.0f, 36.0f) };

        GpxFly = GpxReader.Load(Filename);
        if (GoBuilder != null)
        {
            GoBuilder.PopulateGOGpx(GpxFly, WGS84MapRegion, bounds);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
