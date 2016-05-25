using UnityEngine;
using System.Collections;

public class Dialog : MonoBehaviour {

    TextBoxManager textBoxManager;
    GameObject parent;
    bool playerInZone;
    int layerMask;

    void Start()
    {
        textBoxManager = FindObjectOfType<TextBoxManager>();
        parent = transform.parent.gameObject;
        layerMask = 9;
    }

    void Update()
    {
        if (playerInZone)
        {
            textBoxManager.EnableTextBox(parent.GetComponent<NPC>().myName, parent.GetComponent<NPC>().myId);
            /*
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 10000.0f, layerMask))
                {
                    if (hit.transform.gameObject.tag == "NPC")
                    {
                        

                   }
                }
            }
            */
        }
    }

    void OnTriggerEnter(Collider other)
    {
        textBoxManager.playerArrivedToDialogZone();
        GameObject target;
        if (other.tag == "Player")
        {
            target = other.GetComponent<PlayerControl>().getTarget();
            if (target == transform.parent.gameObject)
            {
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

    }

    void OnTriggerExit(Collider other)
    {
        textBoxManager.playerLeftDialogZone();
        GameObject target;
        if (other.tag == "Player")
        {
            target = other.GetComponent<PlayerControl>().getTarget();
            if (target == transform.parent.gameObject)
            {
                playerInZone = false;
            }
        }
        /*
        if (other.name == "Player")
        {
            playerInZone = false;
            //textBoxManager.DisableTextBox();
        }
        */
    }
}
