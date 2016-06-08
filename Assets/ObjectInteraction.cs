using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ObjectInteraction : MonoBehaviour {

    // Use this for initialization

    GameObject target;

    //reserved objects booked for this object
    GameObject currentChair;
    GameObject bookedBed;

    void Start () {
        currentChair = null;
        bookedBed = null;
        target = null;
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void setTarget(GameObject go)
    {
        target = go;
    }

    public void setCurrentChair(GameObject go)
    {
        currentChair = go;
    }

    public void setBookedBed(GameObject go)
    {
        bookedBed = go;
    }

    public GameObject getCurrentChair()
    {
        return currentChair;
    }

    public GameObject getTarget()
    {
        return target;
    }

    public GameObject getBed()
    {
        return bookedBed;
    }

    /* Gets Vector3 position next to the bed left side */
    /* Side: 0 - front, 1 - left, 2 - back, 3 - right */
    public Vector3 getDestToTargetObjectSide(int side, float offset)
    {
        Vector3 destination = Vector3.zero;
        float targetroty = target.transform.eulerAngles.y;
        switch (side)
        {
            case 0:
                if (approx(targetroty, 0.0f, 0.1f))
                    destination = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z - offset);
                else if (approx(targetroty, 90.0f, 0.1f))
                    destination = new Vector3(target.transform.position.x - offset, transform.position.y, target.transform.position.z);
                else if (approx(targetroty, 180.0f, 0.1f))
                    destination = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z + offset);
                else if (approx(targetroty, 270.0f, 0.1f))
                    destination = new Vector3(target.transform.position.x + offset, transform.position.y, target.transform.position.z);
                else
                {
                    print("Target unreachable! Target name: " + target.name);
                }
                break;
            case 1:
                if (approx(targetroty, 0.0f, 0.1f))
                    destination = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z + offset);
                else if (approx(targetroty, 90.0f, 0.1f))
                    destination = new Vector3(target.transform.position.x - offset, transform.position.y, target.transform.position.z);
                else if (approx(targetroty, 180.0f, 0.1f))
                    destination = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z - offset);
                else if (approx(targetroty, 270.0f, 0.1f))
                    destination = new Vector3(target.transform.position.x + offset, transform.position.y, target.transform.position.z);
                else
                {
                    print("Target unreachable! Target name: " + target.name);
                }
                break;
            case 2:
                if (approx(targetroty, 0.0f, 0.1f))
                    destination = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z + offset);
                else if (approx(targetroty, 90.0f, 0.1f))
                    destination = new Vector3(target.transform.position.x - offset, transform.position.y, target.transform.position.z);
                else if (approx(targetroty, 180.0f, 0.1f))
                    destination = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z - offset);
                else if (approx(targetroty, 270.0f, 0.1f))
                    destination = new Vector3(target.transform.position.x + offset, transform.position.y, target.transform.position.z);
                else
                {
                    print("Target unreachable! Target name: " + target.name);
                }
                break;
            case 3:
                if (approx(targetroty, 0.0f, 0.1f))
                    destination = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z + offset);
                else if (approx(targetroty, 90.0f, 0.1f))
                    destination = new Vector3(target.transform.position.x - offset, transform.position.y, target.transform.position.z);
                else if (approx(targetroty, 180.0f, 0.1f))
                    destination = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z - offset);
                else if (approx(targetroty, 270.0f, 0.1f))
                    destination = new Vector3(target.transform.position.x + offset, transform.position.y, target.transform.position.z);
                else
                {
                    print("Target unreachable! Target name: " + target.name);
                }
                break;

            
        }
        return destination;
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
}
