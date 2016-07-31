using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ZoomHandler : MonoBehaviour {
    public float defaultZoom = 1.0f;
    public float maxZoom = 3.0f;

    public float OneToOneZoom = 30;

    public float defaultY = 0;
    public float zoomY = 0.5f;

    int currentZoom = 0;

    public GameObject mapCursor;

	// Use this for initialization
	void Start () {
        ZoomOut();

    }

    void setClips()
    {
        if (currentZoom == 0)
        {
            Camera.main.DOFarClipPlane(20, 0.5f);
            Camera.main.DONearClipPlane(0.1f, 0.5f);
        }

        if (currentZoom == 1)
        {
            Camera.main.DOFarClipPlane(100, 0.5f);
            Camera.main.DONearClipPlane(0.5f, 0.5f);
        }


        if (currentZoom == 2)
        {
            Camera.main.DOFarClipPlane(500, 0.5f);
            Camera.main.DONearClipPlane(0.5f, 0.5f);
        }



    }

    void ZoomIn()
    {
        
        currentZoom++;
        if (currentZoom >2) currentZoom = 2;

        if (currentZoom == 2)
        {
            this.transform.DOScale(OneToOneZoom, 0.5f);
        }
        else
        {
            this.transform.DOScale(maxZoom, 0.5f);
        }

        if (mapCursor.activeInHierarchy)
        {
            //OK so we zoom in onthis


        }



        //this.transform.localScale = new Vector3(maxZoom, maxZoom, maxZoom);
        Vector3 pos = this.transform.position;
        //pos.y = zoomY;
        this.transform.position = pos;
        setClips();
    }

    void ZoomOut()
    {
        currentZoom--;
        if (currentZoom < 0) currentZoom = 0;
        if (currentZoom == 1)
        {
            this.transform.DOScale(maxZoom, 0.5f);
        }
        else
        {
            this.transform.DOScale(defaultZoom, 0.5f);
        }
        //this.transform.localScale = new Vector3(defaultZoom, defaultZoom, defaultZoom);
        Vector3 pos = this.transform.position;
        //pos.y = defaultY;
        this.transform.position = pos;
        setClips();
    }

    // Update is called once per frame
    void Update () {
	
	}
}
