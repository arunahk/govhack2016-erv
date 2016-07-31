using UnityEngine;
using System.Collections;

public class KMLModel : MonoBehaviour
{
    [Header(" [Connnect this in the editor]")]
    public KMLGOBuilder GoBuilder = null;

    [Header(" [Set to the kml filename]")]
    public string Filename = null;

    [Header(" [Contains the extents of the map area in Unity space]")]
    public GameObject BX23 = null;
    public GameObject BX24 = null;

    private MyKmlReader KmlReader = null;
    private KmlFlyweight KmlFly = null;

    // The region is limited to the maps tiles BX23 and BX24 
    // The structure specifies the extents of the map are in WGS-84 space
    RectFlyweight WGS84MapRegion = new RectFlyweight() { MinLon = 172.25456342f, MinLat = -43.76461329f , MaxLon = 172.85169953f, MaxLat = -43.44279919f};

    // Use this for initialization
    void Start()
    {
        // A more complete solution would have this done in a background thread       
        KmlReader = new MyKmlReader();

        // get the map region in unity world units
        //Vector3 bx23Size = BX23.GetComponent<Terrain>().terrainData.size;
        //Vector3 bx24Size = BX24.GetComponent<Terrain>().terrainData.size;
        //Vector3 pos = BX23.transform.position;
        //Bounds bounds = new Bounds() { min = pos, size = new Vector3(bx23Size.x + bx24Size.x, bx24Size.y, bx24Size.z) }; // just combine the width of the two tiles, retain elevation and height form tile bx24

        // Having trouble with the conversion, so moved map to 0,0,0 and hardcoded the transformation
        // it is a hackathon after all :-(
        Bounds bounds = new Bounds() { min = new Vector3(0.0f, 0.0f, 0.0f), max = new Vector3(48.0f, 0.0f, 36.0f) };

        KmlFly = KmlReader.Load(Filename, WGS84MapRegion);
        if(GoBuilder != null)
        {
            GoBuilder.PopulateGOKML(KmlFly, WGS84MapRegion, bounds);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
