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

    public void EnableTextBox(string myName, string myId, float myHp, bool medActive)
    {
        string medStatus = "Active";
        if (!medActive)
            medStatus = "Not active";
        dialog.text = "Name: " + myName + "\n" + "ID: " + myId + "\n" + "Hp: " + myHp.ToString() + "\n" + "Medicine: " + medStatus;
    }

    public void DisableTextBox()
    {
        dialog.text = null;
    }

}
