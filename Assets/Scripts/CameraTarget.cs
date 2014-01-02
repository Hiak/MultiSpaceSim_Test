using UnityEngine;
using System.Collections;

public class CameraTarget : MonoBehaviour {

    public Vector3 fixUp;
    public Transform target;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (fixUp == null)
            transform.LookAt(target);
        else
            transform.LookAt(target, fixUp);
        
	}
}
