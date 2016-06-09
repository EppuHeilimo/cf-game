using UnityEngine;
using System.Collections;

public class DoorOpen : MonoBehaviour {
    Vector3 defaultPos;
    bool isOpen = false;
    //how many objects are in the trigger area
    int triggerObjectsInArea = 0;

	// Use this for initialization
	void Start () {


	}
	
	// Update is called once per frame
	void Update () {

	}

    void OnTriggerEnter(Collider other)
    {
        
        triggerObjectsInArea++;
        if (!isOpen)
        {
            //Debug.Log("door open");
            Vector3 pivot = new Vector3(defaultPos.x + 16, defaultPos.y, defaultPos.z + 16);
            transform.parent.FindChild("default").transform.RotateAround(pivot, new Vector3(0, 1, 0), 90);
            isOpen = true;
            
        }

    }


    void OnTriggerExit(Collider collider)
    {
        triggerObjectsInArea--;

        //fail safe, lets not let the objects go to negative
        if(triggerObjectsInArea < 0)
        {
            triggerObjectsInArea = 0;
        }
        if(isOpen && triggerObjectsInArea == 0)
        {
            //Debug.Log("door close");
            Vector3 pivot = new Vector3(defaultPos.x + 16, defaultPos.y, defaultPos.z + 16);
            transform.parent.FindChild("default").transform.RotateAround(pivot, new Vector3(0, 1, 0), -90);
            isOpen = false;
        }

    }
}
