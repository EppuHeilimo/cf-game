using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

    Vector3 defaultPosition;
    GameObject minigameCamera;
    GameObject canvas;
    GameObject minigame1Canvas;
    GameObject player;
    Transform lockedToThis;
    float locktime = 0;
    float timer = 0;
    float cameraSpeed = 500.0f;

    bool movingCamera;



	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        lockedToThis = player.transform;
        defaultPosition = transform.position;
        gameObject.SetActive(true);
        minigameCamera = GameObject.Find("Minigame Camera");
        minigameCamera.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        if(!movingCamera)
            transform.position = lockedToThis.position;
        if(movingCamera)
        {
            transform.position = Vector3.MoveTowards(transform.position, lockedToThis.position, cameraSpeed * Time.deltaTime);
            if(Vector3.Distance(transform.position, lockedToThis.position) < 5.0f)
            {
                movingCamera = false;
            }
        }
        if(locktime > 0)
        {
            timer += Time.deltaTime;
            if(timer >= locktime)
            {
                timer = 0;
                locktime = 0;
                lockToPlayer();
            }
        }
    }

    public void lockToPlayer()
    {

        movingCamera = true;
        lockedToThis = player.transform;

    }

    public bool lockCameraToThisTransformForXTime(Transform t, float time)
    {
        if (!movingCamera)
        {
            locktime = time;
            lockedToThis = t;
            movingCamera = true;
            return true;
        }
        return false;

    }

    public bool lockCameraToThisTransform(Transform t)
    {
        if(!movingCamera)
        {
            lockedToThis = t;
            movingCamera = true;
            return true;
        }
        return false;

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
