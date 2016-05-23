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

    public void EnableTextBox(string nimi, string hetu)
    {
        dialog.text = "Hello, my name is " + nimi + " and ID: " + hetu;
    }

    public void DisableTextBox()
    {
        dialog.text = null;
    }
}
