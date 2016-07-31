using UnityEngine;
using System.Collections;


public class DebugManager : MonoBehaviour {
    public GameObject FPSDisplay;
    public bool DebugMode = false;
	// Use this for initialization
	void Start () {
	    
	}

    void showFPS(bool state)
    {

        FPSDisplay.SetActive(state);
    }

    void ToggleFPS()
    {
        DebugMode = !DebugMode;
        FPSDisplay.SetActive(DebugMode);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
