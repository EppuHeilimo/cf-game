using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TextBoxManager : MonoBehaviour {

    public Text myNameText = null;
    public Text myIdText = null;
    public Text myProblemText = null;

    public GameObject medCardPanel = null;
    public Text patientInfo = null;
    public Text morningInfo = null;
    public Text morningX = null;
    public Text afternoonInfo = null;
    public Text afternoonX = null;
    public Text eveningInfo = null;
    public Text eveningX = null;
    public Text nightInfo = null;
    public Text nightX = null;

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

    public void EnableTextBox(string myName, string myId, int myHp, int myHappiness, string morningMed, string afternoonMed, string eveningMed, string nightMed, string[] myProblems, int morningDos, int afternoonDos, int eveningDos, int nightDos)
    {
        myNameText.text = myName;
        myIdText.text = myId;
        myProblemText.text = null;
        foreach (string s in myProblems)
        {
            myProblemText.text += s + "\n";
        }
        SetHealthBar(myHp);
        SetHappyBar(myHappiness);
        targetPanel.SetActive(true);
        healthBar.SetActive(true);
        happyBar.SetActive(true);
        medCardPanel.SetActive(true);

        patientInfo.text = myName + " (" + myId + ")";
        if (string.IsNullOrEmpty(morningMed))
        {
            morningInfo.text = null;
            morningX.text = null;
        }
        else
        {
            morningInfo.text = morningMed;
            morningX.text = morningDos.ToString();
        }

        if (string.IsNullOrEmpty(afternoonMed))
        {
            afternoonInfo.text = null;
            afternoonX.text = null;
        }
        else
        {
            afternoonInfo.text = afternoonMed;
            afternoonX.text = afternoonDos.ToString();
        }

        if (string.IsNullOrEmpty(eveningMed))
        {
            eveningInfo.text = null;
            eveningX.text = null;
        }
        else
        {
            eveningInfo.text = eveningMed;
            eveningX.text = eveningDos.ToString();
        }

        if (string.IsNullOrEmpty(nightMed))
        {
            nightInfo.text = null;
            nightX.text = null;
        }
        else
        {
            nightInfo.text = nightMed;
            nightX.text = nightDos.ToString();
        }
    }

    public void DisableTextBox()
    {
        targetPanel.SetActive(false);
        healthBar.SetActive(false);
        happyBar.SetActive(false);
        myNameText.text = null;
        myIdText.text = null;
        myProblemText.text = null;

        medCardPanel.SetActive(false);
        patientInfo.text = null;
        morningInfo.text = null;
        morningX.text = null;
        afternoonInfo.text = null;
        afternoonX.text = null;
        eveningInfo.text = null;
        eveningX.text = null;
        nightInfo.text = null;
        nightX.text = null;
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
