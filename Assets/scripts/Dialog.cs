using UnityEngine;
using System.Collections;

public class Dialog : MonoBehaviour {

    TextBoxManager textBoxManager;
    GameObject parent;

    void Start()
    {
        textBoxManager = FindObjectOfType<TextBoxManager>();
        parent = transform.parent.gameObject;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            textBoxManager.EnableTextBox(parent.GetComponent<NPC>().nimi, parent.GetComponent<NPC>().hetu);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.name == "Player")
        {
            textBoxManager.DisableTextBox();
        }
    }
}
