using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogV2 : MonoBehaviour
{

    TextBoxManager textBoxManager;
    NPCV2 parent;
    public bool playerInZone = false;
    public bool npcInZone = false;
    Dictionary<GameObject, float> timers = new Dictionary<GameObject, float>();
    GameObject WhoIsTargetingMe;


    void Start()
    {
        textBoxManager = FindObjectOfType<TextBoxManager>();
        parent = transform.parent.GetComponent<NPCV2>();
        parent.initChild();
    }

    void Update()
    {

    }

    public GameObject getWhoIsTargetingMe()
    {
        return WhoIsTargetingMe;
    }

    void OnTriggerEnter(Collider other)
    {

        GameObject target;
        if (other.tag == "Player")
        {
            target = other.GetComponent<PlayerControl>().getTarget();
            if (target == transform.parent.gameObject)
            {
                WhoIsTargetingMe = other.gameObject;
                playerInZone = true;
                if (parent.diagnosed)
                {
                    textBoxManager.EnableTextBox(parent);
                }  
                else
                {
                    textBoxManager.EnableTextBoxNotDiagnozed(parent);
                }
            }
        }
        if (other.tag == "NPC")
        {
            target = other.GetComponent<NPCV2>().getTarget();
            if (target == transform.parent.gameObject)
            {
                WhoIsTargetingMe = other.gameObject;
                npcInZone = true;
            }
        }

    }

    void OnTriggerStay(Collider other)
    {

        if (other.tag == "Player")
        {
            GameObject target;
            target = other.GetComponent<PlayerControl>().getTarget();
            if (target == transform.parent.gameObject)
            {
                WhoIsTargetingMe = other.gameObject;
                playerInZone = true;
                if (parent != null)
                {
                    if (parent.diagnosed)
                    {
                        textBoxManager.EnableTextBox(parent);
                    } 
                    else
                    {
                        textBoxManager.EnableTextBoxNotDiagnozed(parent);
                    } 
                }
                else
                    textBoxManager.DisableTextBox();
            }
            else
            {
                playerInZone = false;
            }
        }
        if(other.tag == "NPC" && other.gameObject == WhoIsTargetingMe)
        {
            NPCV2 npc = other.GetComponent<NPCV2>();
            if(npc.myState == NPCV2.NPCState.STATE_DEAD)
            {
                WhoIsTargetingMe = null;
            } 
        }
    }

    void OnTriggerExit(Collider other)
    {

        GameObject target;
        if (other.tag == "Player")
        {
            if(other.gameObject == WhoIsTargetingMe)
                WhoIsTargetingMe = null;
            playerInZone = false;
            if (parent.diagnosed)
            {
                textBoxManager.DisableTextBox();
            }  
            else
            {
                textBoxManager.DisableTextBoxNotDiagnozed();
            }
        }
        if (other.tag == "NPC" && other.gameObject == WhoIsTargetingMe)
        {
            target = other.GetComponent<NPCV2>().getTarget();
            if (target == transform.parent.gameObject)
            {
                WhoIsTargetingMe = null;
                npcInZone = false;
            }
        }

    }
}
