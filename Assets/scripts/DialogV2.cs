using UnityEngine;
using System.Collections;

public class DialogV2 : MonoBehaviour
{

    TextBoxManager textBoxManager;
    GameObject parent;
    public bool playerInZone = false;
    public bool npcInZone = false;

    void Start()
    {
        textBoxManager = FindObjectOfType<TextBoxManager>();
        parent = transform.parent.gameObject;
        parent.GetComponent<NPCV2>().initChild();
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
                textBoxManager.EnableTextBox(parent.GetComponent<NPCV2>().myName, parent.GetComponent<NPCV2>().myId, 
                                             parent.GetComponent<NPCV2>().myHp, parent.GetComponent<NPCV2>().myHappiness,
                                             parent.GetComponent<NPCV2>().morningMed.title, parent.GetComponent<NPCV2>().afternoonMed.title,
                                             parent.GetComponent<NPCV2>().eveningMed.title, parent.GetComponent<NPCV2>().nightMed.title,
                                             parent.GetComponent<NPCV2>().myProblems
                                             );
                playerInZone = true;
            }
        }
        if (other.tag == "NPC")
        {
            target = other.GetComponent<NPCV2>().getTarget();
            if (target == transform.parent.gameObject)
            {
                if (target.GetComponent<NPCV2>().isIdle())
                {
                    parent.GetComponent<NPCV2>().setTarget(other.gameObject);
                    npcInZone = true;
                }

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
                if (parent != null)
                {
                    textBoxManager.EnableTextBox(parent.GetComponent<NPCV2>().myName, parent.GetComponent<NPCV2>().myId,
                             parent.GetComponent<NPCV2>().myHp, parent.GetComponent<NPCV2>().myHappiness,
                             parent.GetComponent<NPCV2>().morningMed.title, parent.GetComponent<NPCV2>().afternoonMed.title,
                             parent.GetComponent<NPCV2>().eveningMed.title, parent.GetComponent<NPCV2>().nightMed.title,
                             parent.GetComponent<NPCV2>().myProblems
                             );
                    playerInZone = true;
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
            target = other.GetComponent<PlayerControl>().getTarget();
            if (target == transform.parent.gameObject)
            {
                textBoxManager.DisableTextBox();
                playerInZone = false;
            }
        }

        if (other.tag == "NPC")
        {
            target = other.GetComponent<NPCV2>().getTarget();
            if (target == transform.parent.gameObject)
            {
                npcInZone = false;
            }
        }

    }
}
