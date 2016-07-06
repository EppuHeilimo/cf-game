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
    GameObject npcTargetingThis;


    void Start()
    {
        textBoxManager = FindObjectOfType<TextBoxManager>();
        parent = transform.parent.GetComponent<NPCV2>();
        parent.initChild();
    }

    void Update()
    {

    }

    public GameObject getNpcTargetingMe()
    {
        return npcTargetingThis;
    }

    void OnTriggerEnter(Collider other)
    {

        GameObject target;
        if (other.tag == "Player")
        {
            target = other.GetComponent<PlayerControl>().getTarget();
            if (target == transform.parent.gameObject)
            {
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
                npcTargetingThis = other.gameObject;
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
    }

    void OnTriggerExit(Collider other)
    {

        GameObject target;
        if (other.tag == "Player")
        {
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
        if (other.tag == "NPC" && other == npcTargetingThis)
        {
            target = other.GetComponent<NPCV2>().getTarget();
            if (target == transform.parent.gameObject)
            {
                parent.talking = false;
                npcTargetingThis = null;
                npcInZone = false;
            }
        }

    }
}
