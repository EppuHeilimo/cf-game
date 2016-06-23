using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

    Vector3 defaultPosition;
    GameObject minigameCamera;
    GameObject canvas;
    GameObject minigame1Canvas;

	// Use this for initialization
	void Start () {
        defaultPosition = transform.position;
        gameObject.SetActive(true);
        minigameCamera = GameObject.Find("Minigame Camera");
        minigameCamera.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        Vector3 playerPos = playerTransform.position;
        Vector3 camPos = transform.position;
        if (playerPos.x != camPos.x || playerPos.z != camPos.z)
        {
            transform.position = new Vector3(playerPos.x + defaultPosition.x, camPos.y, playerPos.z + defaultPosition.z);
        }
    }

    public void SwitchToMinigame1Camera()
    {
        gameObject.SetActive(false);
        minigameCamera.SetActive(true);
    }

    public void SwitchToMainCamera()
    {
        gameObject.SetActive(true);
        minigameCamera.SetActive(false);
    }
}
