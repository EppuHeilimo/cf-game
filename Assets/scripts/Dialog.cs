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
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 10000.0f, layerMask))
                {
                    if (hit.transform.gameObject.tag == "NPC")
                    { 
                        if (hit.transform.gameObject.GetComponent<NPC>().myId == parent.GetComponent<NPC>().myId)
                            textBoxManager.EnableTextBox(parent.GetComponent<NPC>().myName, parent.GetComponent<NPC>().myId);
                    }
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            playerInZone = true;      
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.name == "Player")
        {
            playerInZone = false;
            //textBoxManager.DisableTextBox();
        }
    }
}
