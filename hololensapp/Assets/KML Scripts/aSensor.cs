using UnityEngine;
using System.Collections;

public class aSensor : MonoBehaviour {
    public Texture2D Gradiant;
	// Use this for initialization
	void Start () {
	
	}

    public Color getColor(float Mag)
    {
        Color myColor = new Color();
        int iMag = (int)((Mag / 100f) * 256);
        myColor = Gradiant.GetPixel(1, iMag);

        return myColor;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
