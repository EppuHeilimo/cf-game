using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

    Vector3 defaultPosition;
	// Use this for initialization
	void Start () {
        defaultPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        Transform playerTransform = GameObject.Find("Player 1").transform;
        Vector3 playerPos = playerTransform.position;
        Vector3 camPos = transform.position;
        if (playerPos.x != camPos.x || playerPos.z != camPos.z)
        {
            transform.position = new Vector3(playerPos.x + defaultPosition.x, camPos.y, playerPos.z + defaultPosition.z);
        }
    }
}
