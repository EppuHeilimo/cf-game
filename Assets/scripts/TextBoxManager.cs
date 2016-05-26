using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextBoxManager : MonoBehaviour {

    [SerializeField]
    Text dialog;

    // Use this for initialization
    void Start()
    {
        DisableTextBox();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void EnableTextBox(string myName, string myId)
    {
        dialog.text = "Hello, my name is " + myName + " and ID: " + myId;
    }

    public void DisableTextBox()
    {
        dialog.text = null;
    }

}
