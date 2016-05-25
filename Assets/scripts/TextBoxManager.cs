using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextBoxManager : MonoBehaviour {

    [SerializeField]
    Text dialog;
    int dialogZonesTriggered = 0;

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

    public void playerArrivedToDialogZone()
    {
        dialogZonesTriggered++;
    }
    public void playerLeftDialogZone()
    {
        if (dialogZonesTriggered > 0)
            dialogZonesTriggered--;
    }
}
