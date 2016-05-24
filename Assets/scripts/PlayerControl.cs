using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {

    // Use this for initialization
    void Start () {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
    }
	
	// Update is called once per frame
	void Update () {
        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || (Input.GetMouseButtonDown(0)))
        {
            RaycastHit hit;
            //Create a Ray on the tapped / clicked position

            //Layer mask
            int layerMask = 1 << 8;

            Ray ray = new Ray();
            //for unity editor
            #if UNITY_EDITOR
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Input.GetKeyDown(KeyCode.A))
            {
                Debug.DrawRay(ray.origin, ray.direction, Color.red, 10.0f);
            }
            
            //for touch device
            #elif (UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8)
            ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            #endif

            //check if the ray hits any collider
            if (Physics.Raycast(ray, out hit, 10000.0f, layerMask))
            {
                NavMeshAgent agent = GetComponent<NavMeshAgent>();

                agent.destination = new Vector3(hit.point.x, 0, hit.point.z);
            }
        }
        var d = Input.GetAxis("Mouse ScrollWheel");
        if (d > 0f)
        {
            GameObject.Find("Main Camera").GetComponent<Camera>().orthographicSize++;
        }
        else if (d < 0f)
        {
            GameObject.Find("Main Camera").GetComponent<Camera>().orthographicSize--;
        }
    }
}
