using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextBoxManager : MonoBehaviour {

    [SerializeField]
    Text dialog;

    [SerializeField]
    GameObject btnGiveMed;

    // Use this for initialization
    void Start()
    {
        DisableTextBox();
        btnGiveMed.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void EnableTextBox(string myName, string myId, float myHp, float myHappiness)
    {
        dialog.text = "Name: " + myName + "\n" + "ID: " + myId + "\n" + "Hp: " + myHp.ToString() + "\n" + "Happiness:" + myHappiness.ToString();
        btnGiveMed.SetActive(true);
    }

    public void DisableTextBox()
    {
        dialog.text = null;
        btnGiveMed.SetActive(false);
    }

}
