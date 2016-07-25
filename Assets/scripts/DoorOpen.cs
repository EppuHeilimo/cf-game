using UnityEngine;
using System.Collections;

public class DoorOpen : MonoBehaviour {
    Vector3 defaultPos;
    bool isOpen = false;
    bool isOpening = false;
    bool isClosing = false;
    Transform mesh;
    Transform parent;
    Vector3 pivot;
    float timer = 0;
    float openspeed = 300f;
    //how many objects are in the trigger area
    int triggerObjectsInArea = 0;
    float rotation = 0;
    float currRotationY = 0;
    float openRotationTarget = 90;
    float closeRotationTarget = 0;

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

        /* Checks for door's rotation and sets the pivot point */
        if (approx(parent.rotation.eulerAngles.y, 0f, 0.1f))
        {
            pivot = new Vector3(defaultPos.x + 16, defaultPos.y, defaultPos.z + 16);
            rotation = 0;
        }
        if (approx(parent.rotation.eulerAngles.y, 90f, 0.1f))
        {
            pivot = new Vector3(defaultPos.x + 16, defaultPos.y, defaultPos.z - 16);
            rotation = 90;
        }
        if (approx(parent.rotation.eulerAngles.y, 180f, 0.1f))
        {
            pivot = new Vector3(defaultPos.x - 16, defaultPos.y, defaultPos.z - 16);
            rotation = 180;
        }
        if (approx(parent.rotation.eulerAngles.y, 270f, 0.1f))
        {
            pivot = new Vector3(defaultPos.x - 16, defaultPos.y, defaultPos.z + 16);
            rotation = 270;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {

        //Animate opening and closing
        if(isOpening && !isOpen)
        {
            currRotationY += Time.deltaTime * openspeed;
            mesh.transform.RotateAround(pivot, new Vector3(0, 1, 0), Time.deltaTime * openspeed);
            if(currRotationY >= openRotationTarget)
            {
                isOpen = true;
                isOpening = false;
            }
        }
        if(isClosing)
        {
            currRotationY -= Time.deltaTime * openspeed;
            mesh.transform.RotateAround(pivot, new Vector3(0, 1, 0), -(Time.deltaTime * openspeed));
            if (currRotationY <= closeRotationTarget)
            {
                isOpen = false;
                isOpening = false;
                isClosing = false;
            }
        }
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
        if (!isOpen && !isOpening)
        {
            isOpening = true;          
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
        if(isOpen || isOpening && triggerObjectsInArea == 0)
        {
            isOpen = false;
            isOpening = false;
            isClosing = true;
        }

    }
}
