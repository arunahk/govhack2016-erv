using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour {
    public GameObject[] Scenes;
    public GameObject AlignmentScene;

    int LastScene;
    // Use this for initialization
    void Start () {
       // AlignmentScene.SetActive(true);
        ShowScene(0);
    }

    public void ShowAlignment(bool state)
    {
        AlignmentScene.SetActive(state);
        for (int t = 0; t < Scenes.Length; t++)
        {
            Scenes[t].SetActive(false);
        }
        if (!state) {
            Scenes[LastScene].SetActive(true);
        }
    }

    void ToggleAlignment()
    {
        AlignmentScene.SetActive(!AlignmentScene.activeInHierarchy);
    }

    public void ShowScene(int ID)
    {
        AlignmentScene.SetActive(false);
        LastScene = ID;
        for (int t=0; t<Scenes.Length; t++)
        {
            Scenes[t].SetActive(false);
        }
        Scenes[ID].SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
