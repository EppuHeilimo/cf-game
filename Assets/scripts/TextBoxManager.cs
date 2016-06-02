using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextBoxManager : MonoBehaviour {

    [SerializeField]
    Text dialog;

    GameObject healthBar;

    // Use this for initialization
    void Start()
    {
        healthBar = GameObject.FindGameObjectWithTag("HpBar");
        DisableTextBox();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void EnableTextBox(string myName, string myId, float myHp, bool medActive, string myProblem)
    {
        string medStatus = "Active";
        if (!medActive)
            medStatus = "Not active";
        dialog.text = "Name: " + myName + "\n" + "ID: " + myId + "\n" + "Hp: " + myHp.ToString() + "\n" + "Medicine: " + medStatus + "\n" + "Problem: " + myProblem;
        SetHealthBar(myHp);
        healthBar.SetActive(true);
    }

    public void DisableTextBox()
    {
        healthBar.SetActive(false);
        dialog.text = null;
    }

    public void SetHealthBar(float myHp)
    {
        float scaledHp = myHp / 100f;
        healthBar.transform.localScale = new Vector3(Mathf.Clamp(scaledHp, 0f, 1f), healthBar.transform.localScale.y, healthBar.transform.localScale.z);
    }
}
