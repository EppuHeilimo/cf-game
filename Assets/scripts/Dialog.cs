using UnityEngine;
using System.Collections;

public class Dialog : MonoBehaviour {

    TextBoxManager textBoxManager;
    GameObject parent;
    public bool playerInZone = false;

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
                textBoxManager.EnableTextBox(parent.GetComponent<NPC>().myName, parent.GetComponent<NPC>().myId, parent.GetComponent<NPC>().myHp, parent.GetComponent<NPC>().myHappiness);
                playerInZone = true;
            }
        }

            
        /*
        if (other.name == "Player")
        {
            playerInZone = true;      
        }
        */
    }

    void OnTriggerStay(Collider other)
    {
        
        if(other.tag == "Player")
        {
            GameObject target;

            target = other.GetComponent<PlayerControl>().getTarget();
            if (target == transform.parent.gameObject)
            {
                textBoxManager.EnableTextBox(parent.GetComponent<NPC>().myName, parent.GetComponent<NPC>().myId, parent.GetComponent<NPC>().myHp, parent.GetComponent<NPC>().myHappiness);
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

    }
}
