using UnityEngine;
using System.Collections;

public class DoorOpen : MonoBehaviour {
    Vector3 defaultPos;
    bool isOpen = false;
    Transform mesh;
    Transform parent;
    //how many objects are in the trigger area
    int triggerObjectsInArea = 0;
    float rotation = 0;

	// Use this for initialization
	void Start () {
        foreach (Transform child in transform.parent)
        {
            if(child.name == "default")
            {
                mesh = child;
            }
        }
        parent = transform.parent;
        defaultPos = parent.position;
        if (approx(parent.rotation.eulerAngles.y, 0f, 0.1f))
        {
            rotation = 0;
        }
        if (approx(parent.rotation.eulerAngles.y, 90f, 0.1f))
        {
            rotation = 90;
        }
        if (approx(parent.rotation.eulerAngles.y, 180f, 0.1f))
        {
            rotation = 180;
        }
        if (approx(parent.rotation.eulerAngles.y, 270f, 0.1f))
        {
            rotation = 270;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {

	}

    private bool approx(float a, float b, float accuracy)
    {
        float sub = a - b;
        if (Mathf.Abs(sub) < accuracy)
        {
            return true;
        }
        return false;

    }

    void OnTriggerEnter(Collider other)
    {
        triggerObjectsInArea++;
        if (!isOpen)
        {
            if(rotation == 0)
            {
                Vector3 pivot = new Vector3(defaultPos.x + 16, defaultPos.y, defaultPos.z + 16);
                mesh.transform.RotateAround(pivot, new Vector3(0, 1, 0), 90);
            }
            else if (rotation == 90)
            {
                Vector3 pivot = new Vector3(defaultPos.x + 16, defaultPos.y, defaultPos.z - 16);
                mesh.transform.RotateAround(pivot, new Vector3(0, 1, 0), 90);
            }
            else if (rotation == 180)
            {
                Vector3 pivot = new Vector3(defaultPos.x - 16, defaultPos.y, defaultPos.z - 16);
                mesh.transform.RotateAround(pivot, new Vector3(0, 1, 0), 90);
            }
            else if (rotation == 270)
            {
                Vector3 pivot = new Vector3(defaultPos.x - 16, defaultPos.y, defaultPos.z + 16);
                mesh.transform.RotateAround(pivot, new Vector3(0, 1, 0), 90);
            }
            else
            {
                print("door rotation invalid!");
            }
            
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
            if (rotation == 0)
            {
                Vector3 pivot = new Vector3(defaultPos.x + 16, defaultPos.y, defaultPos.z + 16);
                mesh.transform.RotateAround(pivot, new Vector3(0, 1, 0), -90);
            }
            else if (rotation == 90)
            {
                Vector3 pivot = new Vector3(defaultPos.x + 16, defaultPos.y, defaultPos.z - 16);
                mesh.transform.RotateAround(pivot, new Vector3(0, 1, 0), -90);
            }
            else if (rotation == 180)
            {
                Vector3 pivot = new Vector3(defaultPos.x - 16, defaultPos.y, defaultPos.z - 16);
                mesh.transform.RotateAround(pivot, new Vector3(0, 1, 0), -90);
            }
            else if (rotation == 270)
            {
                Vector3 pivot = new Vector3(defaultPos.x - 16, defaultPos.y, defaultPos.z + 16);
                mesh.transform.RotateAround(pivot, new Vector3(0, 1, 0), -90);
            }
            else
            {
                print("door rotation invalid!");
            }

            isOpen = false;
        }

    }
}
