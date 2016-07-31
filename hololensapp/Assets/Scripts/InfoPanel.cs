using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InfoPanel : MonoBehaviour {
    public GameObject Panel;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Panel.SetActive(false);
    }

    public void SetText(string What)
    {
        Panel.GetComponentInChildren<Text>().text = What;
        Panel.SetActive(true);
    }
}
