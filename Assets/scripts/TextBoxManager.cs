using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextBoxManager : MonoBehaviour {

    public Text myNameText = null;
    public Text myIdText = null;
    public Text myProblemText = null;

    GameObject targetPanel;
    GameObject healthBar;
    GameObject happyBar;

    // Use this for initialization
    void Start()
    {
        healthBar = GameObject.FindGameObjectWithTag("HpBar");
        happyBar = GameObject.FindGameObjectWithTag("HappyBar");
        targetPanel = GameObject.FindGameObjectWithTag("TargetPanel");
        DisableTextBox();
    }

    public void EnableTextBox(string myName, string myId, int myHp, int myHappiness)
    {
        myNameText.text = myName;
        myIdText.text = myId;
        SetHealthBar(myHp);
        SetHappyBar(myHappiness);
        targetPanel.SetActive(true);
        healthBar.SetActive(true);
        happyBar.SetActive(true);
    }

    public void DisableTextBox()
    {
        targetPanel.SetActive(false);
        healthBar.SetActive(false);
        happyBar.SetActive(false);
        myNameText.text = null;
        myIdText.text = null;
        myProblemText.text = null;
    }

    public void SetHealthBar(int myHp)
    {
        float scaledHp = myHp / 100f;
        healthBar.transform.localScale = new Vector3(Mathf.Clamp(scaledHp, 0f, 1f), healthBar.transform.localScale.y, healthBar.transform.localScale.z);
    }

    public void SetHappyBar(int myHappiness)
    {
        float scaledHappiness = myHappiness / 100f;
        happyBar.transform.localScale = new Vector3(Mathf.Clamp(scaledHappiness, 0f, 1f), happyBar.transform.localScale.y, happyBar.transform.localScale.z);
    }
}
