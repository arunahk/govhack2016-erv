using UnityEngine;

public class ButtonCommands : MonoBehaviour
{
    public int SceneID = 0;
    // Use this for initialization
    void Start()
    {
    }

    // Called by GazeGestureManager when the user performs a Select gesture
    void OnSelect()
    {
        SceneManager sm = SceneManager.FindObjectOfType<SceneManager>();
        sm.ShowScene(SceneID);
        //this.BroadcastMessage("ShowScene", SceneID);

    }


}