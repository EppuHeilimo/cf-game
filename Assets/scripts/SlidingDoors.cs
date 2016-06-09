using UnityEngine;
using System.Collections;

public class SlidingDoors : MonoBehaviour {

    int triggerObjectsInArea = 0;
    Vector3[] defaultPos = new Vector3[2];
    bool isOpen = false;
    float animSpeed = 40f;
    Transform[] doors = new Transform[2];
    Vector3[] targetPos = new Vector3[2];
    // Use this for initialization
    void Start() {
        if (Mathf.Approximately(transform.rotation.y, 0.0f))
        {
            doors[0] = transform.FindChild("door 1");
            doors[1] = transform.FindChild("door 2");
            defaultPos[0] = doors[0].position;
            defaultPos[1] = doors[1].position;
            targetPos[0] = new Vector3(defaultPos[0].x, defaultPos[0].y, defaultPos[0].z - 30);
            targetPos[1] = new Vector3(defaultPos[1].x, defaultPos[1].y, defaultPos[0].z + 30);
        }
        else if (Mathf.Approximately(transform.rotation.eulerAngles.y, 90.0f))
        {
            doors[0] = transform.FindChild("door 1");
            doors[1] = transform.FindChild("door 2");
            defaultPos[0] = doors[0].position;
            defaultPos[1] = doors[1].position;
            targetPos[0] = new Vector3(defaultPos[0].x - 30, defaultPos[0].y, defaultPos[0].z);
            targetPos[1] = new Vector3(defaultPos[1].x + 30, defaultPos[1].y, defaultPos[0].z);
        }

    }
	
	// Update is called once per frame
	void Update () {
        if (Mathf.Approximately(transform.rotation.y, 0.0f))
        {
            if (isOpen)
            {
                if (doors[0].position.z - targetPos[0].z > 0)
                    doors[0].position = new Vector3(defaultPos[0].x, defaultPos[0].y, doors[0].position.z - Time.deltaTime * animSpeed);
                if (doors[1].position.z - targetPos[1].z < 0)
                    doors[1].position = new Vector3(defaultPos[1].x, defaultPos[1].y, doors[1].position.z + Time.deltaTime * animSpeed);
            }
            else
            {
                if (doors[0].position.z < defaultPos[0].z)
                    doors[0].position = new Vector3(defaultPos[0].x, defaultPos[0].y, doors[0].position.z + Time.deltaTime * animSpeed);
                if (doors[1].position.z > defaultPos[1].z)
                    doors[1].position = new Vector3(defaultPos[1].x, defaultPos[1].y, doors[1].position.z - Time.deltaTime * animSpeed);

            }
        }
        else if(Mathf.Approximately(transform.rotation.eulerAngles.y, 90.0f))
        {
            if (isOpen)
            {
                if (doors[0].position.x - targetPos[0].x > 0)
                    doors[0].position = new Vector3(doors[0].position.x - Time.deltaTime * animSpeed, defaultPos[0].y, defaultPos[0].z );
                if (doors[1].position.x - targetPos[1].x < 0)
                    doors[1].position = new Vector3(doors[1].position.x + Time.deltaTime * animSpeed, defaultPos[1].y, defaultPos[1].z);
            }
            else
            {
                if (doors[0].position.x < defaultPos[0].x)
                    doors[0].position = new Vector3(doors[0].position.x + Time.deltaTime * animSpeed, defaultPos[0].y, doors[0].position.z );
                if (doors[1].position.x > defaultPos[1].x)
                    doors[1].position = new Vector3(doors[1].position.x - Time.deltaTime * animSpeed, defaultPos[1].y, doors[1].position.z );

            }
        }

	}

    void OnTriggerEnter(Collider other)
    {
        triggerObjectsInArea++;
        if(!isOpen)
        {
            isOpen = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        triggerObjectsInArea--;
        if(triggerObjectsInArea < 0)
        {
            triggerObjectsInArea = 0;
        }
        if(isOpen && triggerObjectsInArea == 0)
        {
            isOpen = false;
        }
    }
}
