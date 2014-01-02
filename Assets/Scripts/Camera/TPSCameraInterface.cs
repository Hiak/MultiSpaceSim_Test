using UnityEngine;
using System.Collections;

public class TPSCameraInterface : MonoBehaviour {

    public Camera camera;

    public bool isActiveCamera { get { return active; }
        set
        {
            if (is_active == value) return;

            is_active = value;

            if (value)
            {
                //Debug.Log(camera);

                Camera mainC = Camera.main;
                mainC.tag = "";
                mainC.gameObject.SetActive(false);
                camera.tag = "MainCamera";
                camera.gameObject.SetActive(true);
            }
            else
            {
                camera.tag = "";
                camera.gameObject.SetActive(false);
            }

        }
    }

    bool is_active;


    public float Zoom
    {
        get { return zoom; }
        set
        {
            if ((zoom + value < MinZoom && value < 0) || (zoom + value > MaxZoom && value > 0)) return;

            zoom = value;

            transform.localScale = new Vector3(zoom, zoom, zoom);

        }
    }

    float zoom = 1;

    public float MinZoom = 1;

    public float MaxZoom = 15;

    
    void Start()
    {
        Zoom = 5;
    }
    


    /*
    // Use this for initialization
    void Start()
    {

    }

	// Update is called once per frame
	void Update () {
        
	}
     * */


}
