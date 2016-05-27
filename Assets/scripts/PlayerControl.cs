using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour {
    NavMeshAgent agent;
    GameObject target;
    public GameObject moveindicator;
    GameObject indicator;
    bool isWalking = false;
    List<GameObject> children = new List<GameObject>();
    // Use this for initialization
    void Start () {
        agent = GetComponent<NavMeshAgent>();
    }
	
	// Update is called once per frame
	void Update () {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    disableMoveIndicator();
                }
            }
        }

        

        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || (Input.GetMouseButtonDown(0)))
        {

            RaycastHit hit;
            //Create a Ray on the tapped / clicked position

            //Layer mask
            int layerMask = 1 << 8 | 9;

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
                if (hit.transform.tag != "NPC" || target == hit.transform.gameObject)
                {
                    Vector3 pos = new Vector3(hit.point.x, 0, hit.point.z);
                    enableMoveIndicator(pos);
                    agent.SetDestination(pos);
                }

                if (hit.transform.gameObject.tag == "NPC")
                {

                    if(target != null)
                    {
                        outlineGameObject(target.transform, Shader.Find("Diffuse"));
                    }
                    target = hit.transform.gameObject;
                    outlineGameObject(target.transform, Shader.Find("Outlined/Silhouette Only"));
                    //Debug.Log(target.GetComponent<NPC>().myName + " " + target.GetComponent<NPC>().myId);
                    //target.GetComponent<Renderer>().material.shader = Shader.Find("Self-Illumin/Outlined Diffuse");
                }

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

    void disableMoveIndicator()
    {
        
        if(indicator != null)
        {
            Destroy(indicator);
        }
        
    }

    public GameObject getTarget()
    {
        return target; 
    }

    void enableMoveIndicator(Vector3 pos)
    {
        if(indicator != null)
           Destroy(indicator);
        pos = new Vector3(pos.x, pos.y + 8.5f, pos.z);
        indicator = (GameObject)Instantiate(moveindicator, pos, new Quaternion(0, 0, 0, 0));
    }

    void outlineGameObject(Transform gameobject, Shader shader)
    {
        foreach(Transform child in gameobject)
        {
            outlineGameObject(child, shader);
            if(child.GetComponent<Renderer>() != null)
                child.GetComponent<Renderer>().material.shader = shader;
        }
    }
}
