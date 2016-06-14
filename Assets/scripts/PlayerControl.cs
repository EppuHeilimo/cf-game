﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour {
    NavMeshAgent agent;
    GameObject target;
    public GameObject moveindicator;
    GameObject indicator;
    ObjectInteraction interaction;
    ObjectManager objManager;
    IiroAnimBehavior anim;
    bool sitting = false;
    bool sleeping = false;
    bool followNpc = false;
    // Use this for initialization
    void Start () {
        interaction = GetComponent<ObjectInteraction>();
        anim = GetComponent<IiroAnimBehavior>();
        objManager = GameObject.FindGameObjectWithTag("ObjectManager").GetComponent<ObjectManager>();
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
        if(sitting)
        {
            if(arrivedToDestination(10.0f))
            {
                if(target != null)
                {
                    if (interaction.RotateAwayFrom(target.transform))
                    {
                        if (anim.sit != true)
                        {
                            anim.sit = true;
                        }
                    }
                }
                else
                {
                    sitting = false;
                    objManager.unbookObject(target);
                }

            }
        }
        else
        {
            if(anim.sit == true)
            {
                anim.sit = false;
            }
        }

        if (sleeping)
        {
            if (arrivedToDestination(10.0f))
            {
                if(target != null)
                {
                    if (interaction.RotateAwayFrom(target.transform))
                    {
                        if (anim.goToSleep != true)
                        {
                            anim.goToSleep = true;
                        }
                    }
                }
                else
                {
                    sleeping = false;
                    objManager.unbookObject(target);
                }

            }
        }
        else
        {
            if (anim.goToSleep == true)
            {
                anim.goToSleep = false;
            }
        }
        if(followNpc)
        {
            walkToTarget();
        }

        handleInput();
    }

    private bool arrivedToDestination(float accuracy)
    {
        float dist = Vector3.Distance(agent.destination, transform.position);
        if (dist < accuracy)
            return true;
        else
            return false;
    }

    void handleInput()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            disableTarget();
        }
        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || (Input.GetMouseButtonDown(0)))
        {
            RaycastHit hit;
            //Create a Ray on the tapped / clicked position

            //Layer mask
            LayerMask layerMask = (1 << 8);
            LayerMask layerMaskNpc = (1 << 9);
            LayerMask layerMaskTargetableLayer = (1 << 10);
            Ray ray = new Ray();
            //for unity editor
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //for touch device
#elif (UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8)
            ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);     
#endif
            if (Physics.Raycast(ray, out hit, 10000.0f, layerMaskNpc))
            {
                if (!isMouseOverUI())
                {
                    if (target == hit.transform.gameObject)
                    {
                        agent.SetDestination(new Vector3(target.transform.position.x - 16, target.transform.position.y, target.transform.position.z));
                        followNpc = true;
                    }
                    else
                    {
                        disableTarget();
                        target = hit.transform.gameObject;
                        interaction.setTarget(target);
                        outlineGameObjectRecursive(target.transform, Shader.Find("Outlined/Silhouetted Diffuse"));
                    }
                    if(sitting)
                    {
                        sitting = false;
                    }
                    if (sleeping)
                    {
                        sleeping = false;
                    }
                }

            }
            else if (Physics.Raycast(ray, out hit, 10000.0f, layerMaskTargetableLayer))
            {
                if (!isMouseOverUI())
                {
                    //if object was already targeted act depending on the targetable object
                    if (target == hit.transform.gameObject)
                    {
                        if (target.tag == "Chair" || target.tag == "QueueChair")
                        {
                            if (objManager.bookTargetObject(target, gameObject))
                            {
                                interaction.setCurrentChair(interaction.getTarget());
                                agent.SetDestination(interaction.getDestToTargetObjectSide(0, 16.0f));
                                sitting = true;
                                disableMoveIndicator();
                            }

                        }
                        else if (target.tag == "Bed")
                        {
                            objManager.bookTargetObject(target, gameObject);
                            interaction.setBookedBed(interaction.getTarget());
                            agent.SetDestination(interaction.getDestToTargetObjectSide(1, 16.0f));
                            sleeping = true;
                            disableMoveIndicator();
                        }
                    }
                    else
                    {

                        if (sitting)
                        {
                            sitting = false;
                            objManager.unbookObject(target);
                        }
                        /*Disable target*/
                        disableTarget();
                        //set new target
                        target = hit.transform.gameObject;
                        interaction.setTarget(target);
                        //outline the object
                        outlineOnlyParent(target.transform, Shader.Find("Outlined/Silhouetted Diffuse"));
                    }
                }
            }
            //check if the ray hits floor collider
            else if (Physics.Raycast(ray, out hit, 10000.0f, layerMask))
            {
                if(!isMouseOverUI())
                {
                    //get position of hit and move there
                    Vector3 pos = new Vector3(hit.point.x, 0, hit.point.z);
                    enableMoveIndicator(pos);
                    agent.SetDestination(pos);
                    if (sitting == true)
                    {
                        sitting = false;
                        objManager.unbookObject(target);
                    }
                    if(followNpc)
                    {
                        followNpc = false;
                    }
                }
            }
            //check if the ray hits targetableobjects collider
            
        }

        /* Scrollwheel zooming */
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

    bool walkToTarget()
    {
        if (Vector3.Distance(transform.position, target.transform.position) < 50.0f)
        {
            return true;
        }
        else
        {
            agent.SetDestination(new Vector3(target.transform.position.x - 20, target.transform.position.y, target.transform.position.z));
            return false;
        }
    }
    void disableTarget()
    {
        if (target != null)
        {
            if (target.tag == "NPC")
            {
                outlineGameObjectRecursive(target.transform, Shader.Find("Standard"));
                followNpc = false;
            }
            else
            {
                outlineOnlyParent(target.transform, Shader.Find("Standard"));
                followNpc = false;
            }
        }
        target = null;
    }

    bool isMouseOverUI()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE_WIN
        /* Check if click was over UI on PC*/
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        #elif (UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8)
        /* Check if touch was over UI on mobile*/
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(0))
        #endif
        {
            return false;
        }
        return true;
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

    void outlineOnlyParent(Transform gameobject, Shader shader)
    {
        Renderer renderer = gameobject.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.shader = shader;
            if (shader.name == "Outlined/Silhouetted Diffuse")
            {
                gameobject.GetComponent<Renderer>().material.SetFloat("_Outline", 0.3f);
                gameobject.GetComponent<Renderer>().material.SetColor("_OutlineColor", new Color(1.0f, 0.0f, 0.0f));
            }
        }
    }

    void outlineGameObjectRecursive(Transform gameobject, Shader shader)
    {

        foreach (Transform child in gameobject)
        {
            outlineGameObjectRecursive(child, shader);
            if(child.GetComponent<Renderer>() != null)
            {
                child.GetComponent<Renderer>().material.shader = shader;
                if(shader.name == "Outlined/Silhouetted Diffuse")
                {
                    child.GetComponent<Renderer>().material.SetFloat("_Outline", 0.023f);
                    child.GetComponent<Renderer>().material.SetColor("_OutlineColor", new Color(1.0f, 0.0f, 0.0f));
                }

            }
                

        }
    }
}
