using UnityEngine;
using System.Collections;

public class Sit : MonoBehaviour {
    public bool sitting = false;
    ObjectInteraction intcomponent;
	// Use this for initialization
	void Start () {
        intcomponent = transform.parent.GetComponent<ObjectInteraction>();
	}
	
	// Update is called once per frame
	void Update () {
	    
	}
}
