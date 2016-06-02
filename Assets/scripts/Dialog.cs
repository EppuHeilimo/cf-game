using UnityEngine;
using System.Collections;

public class Dialog : MonoBehaviour {

    TextBoxManager textBoxManager;
    GameObject parent;
    public bool playerInZone = false;
    public bool npcInZone = false;

    void Start()
    {
        textBoxManager = FindObjectOfType<TextBoxManager>();
        parent = transform.parent.gameObject;
        parent.GetComponent<NPC>().initChild();
    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        GameObject target;
        if (other.tag == "Player")
        {
            target = other.GetComponent<PlayerControl>().getTarget();
            if (target == transform.parent.gameObject)
            {
                textBoxManager.EnableTextBox(parent.GetComponent<NPC>().myName, parent.GetComponent<NPC>().myId, parent.GetComponent<NPC>().myHp, parent.GetComponent<NPC>().myHappiness, parent.GetComponent<NPC>().gotMed, parent.GetComponent<NPC>().myProblem);
                playerInZone = true;
            }
        }
        if(other.tag == "NPC")
        {
            target = other.GetComponent<NPC>().getTarget();
            if(target == transform.parent.gameObject)
            {
                if(target.GetComponent<NPC>().isIdle())
                {
                    parent.GetComponent<NPC>().setTarget(other.gameObject);
                    npcInZone = true;
                }
                    
            }
        }

    }

    void OnTriggerStay(Collider other)
    {
        
        if(other.tag == "Player")
        {
            GameObject target;

            target = other.GetComponent<PlayerControl>().getTarget();
            if (target == transform.parent.gameObject)
            {
                textBoxManager.EnableTextBox(parent.GetComponent<NPC>().myName, parent.GetComponent<NPC>().myId, parent.GetComponent<NPC>().myHp, parent.GetComponent<NPC>().myHappiness, parent.GetComponent<NPC>().gotMed, parent.GetComponent<NPC>().myProblem);
                playerInZone = true;
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
            target = other.GetComponent<PlayerControl>().getTarget();
            if (target == transform.parent.gameObject)
            {
                textBoxManager.DisableTextBox();
                playerInZone = false;
            }
        }

        if (other.tag == "NPC")
        {
            target = other.GetComponent<NPC>().getTarget();
            if (target == transform.parent.gameObject)
            {
                npcInZone = false;
            }
        }

    }
}
