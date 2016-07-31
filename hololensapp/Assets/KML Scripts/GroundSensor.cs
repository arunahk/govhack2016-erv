using UnityEngine;
using System.Collections;

public class GroundSensor : MonoBehaviour
{
    public string Date;
    public string Time;
    public float Magnitude;
    public float PGAVertical;
    public float PGAHorizontal1;
    public float PGAHorizontal2;
    public float Velocity;


    public GameObject myLight;

    // Use this for initialization
    void Start ()
    {
        try
        {
            Velocity = Vector3.Magnitude(new Vector3(PGAVertical, PGAHorizontal1, PGAHorizontal2));
            myLight = this.transform.FindChild("Sensor").transform.FindChild("Light").gameObject;
            GroundSensor mySensor =  this.GetComponent<GroundSensor>();

           // print("MAG " + mySensor.Magnitude);

            aSensor thisSensor = this.GetComponentInChildren<aSensor>();

            //Color myColor = 

            myLight.GetComponent<Renderer>().material.color =  thisSensor.getColor(Velocity);
        }
        catch { }
        //Calculate color from magnitude




        return;
        /*
        if (obj != null)
        {

            obj.GetComponent<Renderer>().material.color = new Color(0, 0, 1);
        }
        */
    }
	
	// Update is called once per frame
	void Update ()
    {

    }
}
