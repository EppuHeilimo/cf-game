using UnityEngine;
using System.Collections;

public class DoorOpen : MonoBehaviour {
    Quaternion defaultRot;
    Vector3 defaultPos;
	// Use this for initialization
	void Start () {
        defaultRot = transform.parent.FindChild("default").transform.rotation;
        defaultPos = transform.parent.position;
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("door open");
        Vector3 pivot = new Vector3(defaultPos.x + 16, defaultPos.y, defaultPos.z + 16);
        transform.parent.FindChild("default").transform.RotateAround(pivot, new Vector3(0, 1, 0), 90);
    }

    void onTriggerStay(Collider collider)
    {

    }

    void OnTriggerExit(Collider collider)
    {
        Debug.Log("door close");
        Vector3 pivot = new Vector3(defaultPos.x + 16, defaultPos.y, defaultPos.z + 16);
        transform.parent.FindChild("default").transform.RotateAround(pivot, new Vector3(0, 1, 0), -90);
    }
}
