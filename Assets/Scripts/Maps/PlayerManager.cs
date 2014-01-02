using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Ship;

public class PlayerManager : MonoBehaviour {

    public static List<Ship> ships=new List<Ship>();

	// Use this for initialization
	void Start () {
        var views=FindObjectsOfType<NetworkView>();

        foreach (var view in views)
        {
            if (view.isMine == false)
            {
                view.transform.parent = transform;
                ships.Add(view.GetComponent<Ship>());
            }
        }
	}
	
	// Update is called once per frame
	/*void Update () {
	
	}*/
}
