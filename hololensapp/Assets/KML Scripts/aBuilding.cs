using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class aBuilding : MonoBehaviour {
    public string BuildingName;
    public string BuildingDescription;
    public string DataType;
    public string DataName;
    public float Velocity = 0;
    public Texture2D Gradiant;

    public GameObject infoPanel;

    // Use this for initialization
    void Start () {
        infoPanel = GameObject.Find("Canvas");

        //Look for color on textures


        RaycastHit hit;
        Vector3 pos = this.transform.position;
        pos.z += -4.55f;
        pos.y = 10.0f;
        //this.transform.position = pos;

        if (Physics.Raycast(pos, -Vector3.up, out hit))
        {
            //print("Found an object - distance: " + hit.distance);
            Velocity = (10f - hit.distance) * 10f;
            Color myColor = new Color();
            int iMag = (int)((Velocity / 100f) * 256);
            myColor = Gradiant.GetPixel(1, iMag);
            this.transform.GetChild(0).GetComponent<Renderer>().material.color = myColor;
        } 
        if (Velocity<-3)
        {
            this.transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(0,0,1);
        }
    }

    //Show if Cursor Over
    public void FocusMe()
    {
        try
        {
            
            infoPanel.GetComponent<InfoPanel>().SetText(BuildingName);
        }
        catch { }
    }


    // Update is called once per frame
    void Update () {

        //Ray myRay = new Ray(pos, Vector2.up);
        

    }
}
