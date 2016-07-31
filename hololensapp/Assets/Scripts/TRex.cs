using UnityEngine;
using System.Collections;

public class TRex : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void PlayDead()
    {
        this.GetComponent<Animator>().SetInteger("animer", 200);
    }
}
