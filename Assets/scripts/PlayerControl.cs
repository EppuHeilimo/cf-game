using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour {

    public GameObject TimeSkipPrefab;

    NavMeshAgent agent;
    GameObject target;
    public GameObject moveindicator;
    GameObject indicator;
    ObjectInteraction interaction;
    ObjectManager objManager;
    IiroAnimBehavior anim;
    Tooltip tooltip;
    NavMeshPath dest;
    Vector3 defaultPosition;

    Profile profile;

    MouseOverIgnore[] mouseOverIgnoreGos;

    bool sitting = false;
    bool sleeping = false;
    bool followNpc = false;
    bool pickingup = false;
    bool movingToTarget = false;
    public bool atTrashCan = true;
    Vector3 nextDestination = Vector3.zero;

    bool agentPaused = false;
    Vector3 lastAgentVelocity = Vector3.zero;
    NavMeshPath lastAgentPath;

    // Use this for initialization
    void Start () {
        dest = new NavMeshPath();
        interaction = GetComponent<ObjectInteraction>();
        anim = GetComponent<IiroAnimBehavior>();
        objManager = GameObject.FindGameObjectWithTag("ObjectManager").GetComponent<ObjectManager>();
        agent = GetComponent<NavMeshAgent>();
        tooltip = GameObject.Find("Inventory").GetComponent<Tooltip>();
        defaultPosition = transform.position;
    }

    public void loadProfile()
    {
        profile = GameObject.Find("SavedData").GetComponent<SavedData>().getProfile();
        if(profile == null)
        {
            print("Null profile");
        }

        mouseOverIgnoreGos = FindObjectsOfType<MouseOverIgnore>();

        //change player's head
        Transform iiro = transform.Find("Iiro");
        Transform root = iiro.Find("Root");
        Transform torso = root.Find("Torso");
        Transform chest = torso.Find("Chest");
        Transform Head = chest.Find("Head");
        Transform head = Head.Find("iiro_head");
        MeshFilter mesh = head.GetComponent<MeshFilter>();
        MeshRenderer material = head.GetComponent<MeshRenderer>();

        mesh.sharedMesh = GameObject.Find("Heads").GetComponent<Heads>().getPlayersHead().GetComponent<MeshFilter>().sharedMesh;
        material.sharedMaterial = GameObject.Find("Heads").GetComponent<Heads>().getPlayersHead().GetComponent<MeshRenderer>().sharedMaterial;
    }

    void pause()
    {
        if (agent.enabled)
        {
            lastAgentVelocity = agent.velocity;
            lastAgentPath = agent.path;
            agent.velocity = Vector3.zero;
            agent.ResetPath();
        }

        agentPaused = true;
    }

    void resume()
    {
        if (agent.enabled)
        {
            agent.velocity = lastAgentVelocity;
            if(lastAgentPath == null)
            {
                moveTo(nextDestination);
            }
            else
            {
                agent.SetPath(lastAgentPath);
            }
            
        }
        agentPaused = false;
    }

    // Update is called once per frame
    void Update () {

        if (anim.waitforanim)
        {
            if(!agentPaused)
                pause();
            return;
        }
        if (agentPaused)
            resume();

        if (!agent.pathPending)
        {
            if (agent.enabled && agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    disableMoveIndicator();
                }
            }
        }


        if(pickingup)
        {
            if (target != null)
            {
                if (arrivedToDestination(50.0f))
                {
                    if(interaction.RotateTowards(target.transform))
                    {
                        anim.pickup();
                        disableTarget();
                    }
                }
            }
        }

        if(movingToTarget)
        {
            if (arrivedToDestination(10.0f) && target != null)
            {
                if (target.tag == "MedCabinet")
                {
                    GameObject.Find("Minigame1").GetComponent<Minigame1>().startMinigame();
                    disableTarget();
                    movingToTarget = false;
                }
                else if(target.tag == "Computer")
                {
                    target.GetComponent<Computer>().StartComputer();
                    disableTarget();
                    movingToTarget = false;
                }
                else if(target.tag == "TrashCan")
                {
                    movingToTarget = false;
                }
                else if (target.tag == "CoffeeMachine")
                {
                    Instantiate(TimeSkipPrefab);
                    movingToTarget = false;
                    disableTarget();
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
                        if(target.tag == "Chair2")
                        {
                            if (agent.enabled)
                            {
                                anim.sitwithrotation();
                            }
                        }
                        else
                        {
                            if (agent.enabled)
                            {
                                anim.sit();
                            }
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

        if (sleeping)
        {
            if (arrivedToDestination(10.0f))
            {
                if(target != null || target.tag == "Bed")
                {
                    if (interaction.RotateAwayFrom(target.transform))
                    {
                        if (!anim.sleeping)
                        {
                            anim.sleep();
                        }
                    }
                }
                else
                {
                    sleeping = false;
                    objManager.unbookObject(interaction.getBed());
                }

            }
        }
        if(followNpc)
        {
            if(target != null)
            {
                disableMoveIndicator();
                if (arrivedToDestination(20.0f))
                {
                    if (agent.hasPath)
                        agent.ResetPath();
                    interaction.RotateTowards(target.transform);
                }
                else
                {
                    walkToTarget();
                }
            }  
        }
        handleInput();
    }

    public void resetPlayerPosition()
    {
        anim.StopAll();
        sitting = false;
        sleeping = false;
        agent.Warp(defaultPosition);
        agent.ResetPath();
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
        if (Input.GetKeyDown(KeyCode.O))
        {
            Time.timeScale += 0.5f;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Time.timeScale -= 0.5f;
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            Time.timeScale = 1.0f;
        }

        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || (Input.GetMouseButtonDown(0)))
        {
            if(!isMouseOverUI())
            {
                if(tooltip.enabled)
                {
                    tooltip.Deactivate();
                }
            }
            
            RaycastHit hit2;
            //Layer mask
            LayerMask layerMask = (1 << 8) | (1 << 11);
            LayerMask layerMaskNpc = (1 << 9) | (1 << 10);
            Ray ray = new Ray();
            //for unity editor
#if UNITY_EDITOR || UNITY_STANDALONE
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //for touch device
#elif (UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8)
            ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);     
#endif
            /*
             * Prioritize raycast hit on npc, so that when npc is sitting 
             * or sleeping you hit the npc instead of the object they are on 
            */
            RaycastHit[] rays;
            rays = Physics.RaycastAll(ray, 10000.0f, layerMaskNpc);
            List<RaycastHit> hits = new List<RaycastHit>();
            bool npcwashit = false;
            movingToTarget = false;
            if (rays.Length > 0)
            {
                if (!isMouseOverUI())
                {
                    anim.StopAll();
                    pickingup = false;
                    foreach (RaycastHit hit in rays)
                    {
                        //object who booked the object. 
                        GameObject temp = objManager.isObjectBooked(hit.transform.gameObject);
                        if (hit.transform.tag != "NPC" && temp == null)
                        {
                            hits.Add(hit);
                            continue;
                        }
                        else
                        { 
                            npcwashit = true;
                            if ((target == hit.transform.gameObject) || (temp != null && target == temp))
                            {
                                nextDestination = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z);
                                moveTo(nextDestination);
                                followNpc = true;
                            }
                            else
                            {
                                disableTarget();
                                if(temp != null && temp != gameObject)
                                {
                                    target = temp;
                                }
                                else
                                {
                                    target = hit.transform.gameObject;
                                }
                                interaction.setTarget(target);
                                outlineGameObjectRecursive(target.transform, Shader.Find("Outlined/Silhouetted Diffuse"));
                            }
                            if (sitting)
                            {
                                sitting = false;
                                objManager.unbookObject(interaction.getCurrentChair());
                            }
                            if (sleeping)
                            {
                                sleeping = false;
                                objManager.unbookObject(interaction.getBed());
                            }
                        }
                    }
                    //if ther was now npc's hit, take the first hit object
                    if(!npcwashit)
                    {
                        hit2 = hits[0];
                        if (target == hit2.transform.gameObject)
                        {
                            if (sitting)
                            {
                                sitting = false;
                                objManager.unbookObject(interaction.getCurrentChair());
                            }
                            if (sleeping)
                            {
                                sleeping = false;
                                objManager.unbookObject(interaction.getBed());
                            }

                            if (target.tag == "Chair" || target.tag == "QueueChair" || target.tag == "Chair2")
                            {
                                if (objManager.bookTargetObject(target, gameObject))
                                {
                                    interaction.setCurrentChair(interaction.getTarget());
                                    if (target.tag == "Chair2")
                                    {
                                        nextDestination = interaction.getDestToTargetObjectSide(1, 16.0f);
                                       moveTo(nextDestination);
                                    }
                                    else
                                    {
                                        nextDestination = interaction.getDestToTargetObjectSide(0, 16.0f);
                                        moveTo(nextDestination);
                                    }
                                    sitting = true;
                                    disableMoveIndicator();
                                }

                            }
                            else if (target.tag == "Bed")
                            {
                                if(objManager.bookTargetObject(target, gameObject))
                                {
                                    interaction.setBookedBed(interaction.getTarget());
                                    nextDestination = interaction.getDestToTargetObjectSide(1, 16.0f);
                                    moveTo(nextDestination);
                                    sleeping = true;
                                    disableMoveIndicator();
                                }
                            }
                            else if (target.tag == "PickupItemFloor" || target.tag == "PickupItem")
                            {
                                interaction.setTarget(target);
                                pickingup = true;
                                nextDestination = target.transform.position;
                                moveTo(nextDestination);
                                disableMoveIndicator();
                            }
                            else if (target.tag == "MedCabinet" || target.tag == "Computer" || target.tag == "TrashCan" || target.tag == "CoffeeMachine")
                            {
                                interaction.setTarget(target);
                                nextDestination = target.transform.position;
                                moveTo(nextDestination);
                                disableMoveIndicator();
                                movingToTarget = true;
                            }
                        }
                        else
                        {
                            
                            if (sitting || interaction.getCurrentChair() != null)
                            {
                                sitting = false;
                                objManager.unbookObject(interaction.getCurrentChair());
                            }
                            if (sleeping || interaction.getBed() != null)
                            {
                                sleeping = false;
                                objManager.unbookObject(interaction.getBed());
                            }
                            /*Disable target*/
                            disableTarget();
                            GameObject temp = objManager.isObjectBooked(hit2.transform.gameObject);
                            if (temp != null)
                            {
                                target = temp;
                            }
                            else
                            {
                                //set new target
                                target = hit2.transform.gameObject;
                            }
                            interaction.setTarget(target);
                            //outline the object
                            outlineOnlyParent(target.transform, Shader.Find("Outlined/Silhouetted Diffuse"));
                        }
                    }
                }
            }
            //check if the ray hits floor collider
            else if (Physics.Raycast(ray, out hit2, 10000.0f, layerMask))
            {
                
                if (!isMouseOverUI())
                {
                    anim.StopAll();
                    pickingup = false;
                    if (interaction.getCurrentChair() != null)
                    {
                        objManager.unbookObject(interaction.getCurrentChair());
                    }
                    if(interaction.getBed() != null)
                    {
                        objManager.unbookObject(interaction.getBed());
                    }
                    //get position of hit and move there
                    Vector3 pos = new Vector3(hit2.point.x, 0, hit2.point.z);
                    enableMoveIndicator(pos);
                    nextDestination = pos;
                    moveTo(nextDestination);
                    if (sitting == true)
                    {
                        sitting = false;
                        objManager.unbookObject(target);
                    }
                    if (sleeping == true)
                    {
                        sleeping = false;
                        objManager.unbookObject(target);
                    }
                    if (followNpc)
                    {
                        followNpc = false;
                    }
                }
            }
            //check if the ray hits targetableobjects collider
            
        }
    }

    public void setTarget(GameObject go)
    {
        disableTarget();
        target = go;
        outlineOnlyParent(target.transform, Shader.Find("Outlined/Silhouetted Diffuse"));
    }

    bool walkToTarget()
    {
        if (Vector3.Distance(transform.position, target.transform.position) < 50.0f)
        {
            return true;
        }
        else
        {
            nextDestination = new Vector3(target.transform.position.x - 20, target.transform.position.y, target.transform.position.z);
            moveTo(nextDestination);
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
        interaction.setTarget(null);
        target = null;
    }

    bool isMouseOverUI()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE
        /* Check if click was over UI on PC*/
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        #elif (UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8)
        /* Check if touch was over UI on mobile*/
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(0))
        #endif
        {
            return false;
        }
        //if mouse is over UI, test if the UI is set to ignore mouse over
        bool ret = true;
        if(mouseOverIgnoreGos != null)
        {
            foreach (MouseOverIgnore ign in mouseOverIgnoreGos)
            {
                if (ign.ignore)
                    ret = false;
            }
        }
        return ret;
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
    /*
     * Tests if player can move to the point, tries to get better point. If can't try to go the destination anyway.
     * Also Calculates the path for navmeshagent synchonously (Only player, this is slow process) 
     */
    public void moveTo(Vector3 dest)
    {
        
        if (agent.enabled)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(dest, out hit, 10.0f, agent.areaMask))
            {
                NavMesh.CalculatePath(transform.position, hit.position, agent.areaMask, this.dest);
                agent.SetPath(this.dest);
            }
            else
            {
                if(NavMesh.CalculatePath(transform.position, dest, agent.areaMask, this.dest))
                {
                    agent.SetPath(this.dest);
                }
                else
                {
                    //if everything else fails, force it to move to dest
                    agent.SetDestination(dest);
                }
                
            }    
        }

    }
}
