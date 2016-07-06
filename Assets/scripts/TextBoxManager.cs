using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TextBoxManager : MonoBehaviour {

    ClockTime clock;

    public Text myNameText = null;
    public Text myIdText = null;
    public Text myProblemText = null;

    public GameObject medCardPanel = null;
    public Text patientInfo = null;
    public Text med1 = null;
    public Text med2 = null;
    public Text med3 = null;
    public Text med4 = null;
    
    public Text morning1 = null;
    public Text night1 = null;
    public Text evening1 = null;
    public Text afternoon1 = null;

    public Text morning2 = null;
    public Text night2 = null;
    public Text evening2 = null;
    public Text afternoon2 = null;

    public Text morning3 = null;
    public Text night3 = null;
    public Text evening3 = null;
    public Text afternoon3 = null;

    public Text morning4 = null;
    public Text night4 = null;
    public Text evening4 = null;
    public Text afternoon4 = null;

    GameObject targetPanel;
    GameObject healthBar;
    GameObject happyBar;
    GameObject MyImage;

    // Use this for initialization
    void Start()
    {
        healthBar = GameObject.FindGameObjectWithTag("HpBar");
        happyBar = GameObject.FindGameObjectWithTag("HappyBar");
        targetPanel = GameObject.FindGameObjectWithTag("TargetPanel");
        MyImage = GameObject.FindGameObjectWithTag("MyImage");
        clock = GameObject.FindGameObjectWithTag("Clock").GetComponent<ClockTime>();
        DisableTextBox();
    }

    public void EnableTextBoxNotDiagnozed(NPCV2 npc)
    {
        myNameText.text = npc.myName;
        myIdText.text = npc.myId;
        myProblemText.text = "Waiting for diagnosis!";
        SetHealthBar(npc.myHp);
        SetHappyBar(npc.myHappiness);
        targetPanel.SetActive(true);
        healthBar.SetActive(true);
        happyBar.SetActive(true);
        MyImage.SetActive(true);
        MyImage.GetComponent<Image>().sprite = npc.myHead2d;
    }

    public void EnableTextBox(NPCV2 npc)
    {
        myNameText.text = npc.myName;
        myIdText.text = npc.myId;
        myProblemText.text = null;
        foreach (string s in npc.myProblems)
        {
            myProblemText.text += s + "\n";
        }
        SetHealthBar(npc.myHp);
        SetHappyBar(npc.myHappiness);
        targetPanel.SetActive(true);
        healthBar.SetActive(true);
        happyBar.SetActive(true);
        medCardPanel.SetActive(true);
        MyImage.SetActive(true);
        MyImage.GetComponent<Image>().sprite = npc.myHead2d;
        patientInfo.text = npc.myName + " (" + npc.myId + ")";
        for (int i = 0; i < npc.myMedication.Length; i++)
        {
            switch (i)
            {
                case 0:
                    med1.text = npc.myMedication[i].Title;
                    break;
                case 1:
                    med2.text = npc.myMedication[i].Title;
                    break;
                case 2:
                    med3.text = npc.myMedication[i].Title;
                    break;
                case 3:
                    med4.text = npc.myMedication[i].Title;
                    break;
            }
        }
        for (int i = 0; i < npc.morningMed.Length; i++)
        {
            if (string.IsNullOrEmpty(npc.morningMed[i].title))
            {
                switch (i)
                {
                    case 0:
                        morning1.text = null;
                        break;
                    case 1:
                        morning2.text = null;
                        break;
                    case 2:
                        morning3.text = null;
                        break;
                    case 3:
                        morning4.text = null;
                        break;
                }
            }
            else
            {
                switch (i)
                {
                    case 0:

                        morning1.text = npc.morningMed[i].dosage.ToString();
                        if (clock.currentDayTime == ClockTime.DayTime.MORNING)
                        {
                            if (npc.morningMed[i].isActive)
                            {
                                morning1.GetComponent<Text>().color = Color.green;
                            }
                            else
                            {
                                morning1.GetComponent<Text>().color = Color.red;
                            }
                        }
                        break;
                    case 1:
                        morning2.text = npc.morningMed[i].dosage.ToString();
                        if (clock.currentDayTime == ClockTime.DayTime.MORNING)
                        {
                            if (npc.morningMed[i].isActive)
                            {
                                morning2.GetComponent<Text>().color = Color.green;
                            }
                            else
                            {
                                morning2.GetComponent<Text>().color = Color.red;
                            }
                        }
                        break;
                    case 2:
                        morning3.text = npc.morningMed[i].dosage.ToString();
                        if (clock.currentDayTime == ClockTime.DayTime.MORNING)
                        {
                            if (npc.morningMed[i].isActive)
                            {
                                morning3.GetComponent<Text>().color = Color.green;
                            }
                            else
                            {
                                morning3.GetComponent<Text>().color = Color.red;
                            }
                        }
                        break;
                    case 3:
                        morning4.text = npc.morningMed[i].dosage.ToString();
                        if (clock.currentDayTime == ClockTime.DayTime.MORNING)
                        {
                            if (npc.morningMed[i].isActive)
                            {
                                morning4.GetComponent<Text>().color = Color.green;
                            }
                            else
                            {
                                morning4.GetComponent<Text>().color = Color.red;
                            }
                        }
                        break;
                }

            }

        }
        for (int i = 0; i < npc.afternoonMed.Length; i++)
        {
            if (string.IsNullOrEmpty(npc.afternoonMed[i].title))
            {
                switch (i)
                {
                    case 0:
                        afternoon1.text = null;
                        break;
                    case 1:
                        afternoon2.text = null;
                        break;
                    case 2:
                        afternoon3.text = null;
                        break;
                    case 3:
                        afternoon4.text = null;
                        break;
                }
            }
            else
            {
                switch (i)
                {
                    case 0:
                        afternoon1.text = npc.afternoonMed[i].dosage.ToString();
                        if (clock.currentDayTime == ClockTime.DayTime.AFTERNOON)
                        {
                            if (npc.afternoonMed[i].isActive)
                            {
                                afternoon1.GetComponent<Text>().color = Color.green;
                            }
                            else
                            {
                                afternoon1.GetComponent<Text>().color = Color.red;
                            }
                        }
                        break;
                    case 1:
                        afternoon2.text = npc.afternoonMed[i].dosage.ToString();
                        if (clock.currentDayTime == ClockTime.DayTime.AFTERNOON)
                        {
                            if (npc.afternoonMed[i].isActive)
                            {
                                afternoon2.GetComponent<Text>().color = Color.green;
                            }
                            else
                            {
                                afternoon2.GetComponent<Text>().color = Color.red;
                            }
                        }
                        break;
                    case 2:
                        afternoon3.text = npc.afternoonMed[i].dosage.ToString();
                        if (clock.currentDayTime == ClockTime.DayTime.AFTERNOON)
                        {
                            if (npc.afternoonMed[i].isActive)
                            {
                                afternoon3.GetComponent<Text>().color = Color.green;
                            }
                            else
                            {
                                afternoon3.GetComponent<Text>().color = Color.red;
                            }
                        }
                        break;
                    case 3:
                        afternoon4.text = npc.afternoonMed[i].dosage.ToString();
                        if (clock.currentDayTime == ClockTime.DayTime.AFTERNOON)
                        {
                            if (npc.afternoonMed[i].isActive)
                            {
                                afternoon4.GetComponent<Text>().color = Color.green;
                            }
                            else
                            {
                                afternoon4.GetComponent<Text>().color = Color.red;
                            }
                        }
                        break;
                }
            }
        }
        for (int i = 0; i < npc.eveningMed.Length; i++)
        {
            if (string.IsNullOrEmpty(npc.eveningMed[i].title))
            {
                switch (i)
                {
                    case 0:
                        evening1.text = null;
                        break;
                    case 1:
                        evening2.text = null;
                        break;
                    case 2:
                        evening3.text = null;
                        break;
                    case 3:
                        evening4.text = null;
                        break;
                }
            }
            else
            {
                switch (i)
                {
                    case 0:
                        evening1.text = npc.eveningMed[i].dosage.ToString();
                        if (clock.currentDayTime == ClockTime.DayTime.EVENING)
                        {
                            if (npc.eveningMed[i].isActive)
                            {
                                evening1.GetComponent<Text>().color = Color.green;
                            }
                            else
                            {
                                evening1.GetComponent<Text>().color = Color.red;
                            }
                        }
                        break;
                    case 1:
                        evening2.text = npc.eveningMed[i].dosage.ToString();
                        if (clock.currentDayTime == ClockTime.DayTime.EVENING)
                        {
                            if (npc.eveningMed[i].isActive)
                            {
                                evening2.GetComponent<Text>().color = Color.green;
                            }
                            else
                            {
                                evening2.GetComponent<Text>().color = Color.red;
                            }
                        }
                        break;
                    case 2:
                        evening3.text = npc.eveningMed[i].dosage.ToString();
                        if (clock.currentDayTime == ClockTime.DayTime.EVENING)
                        {
                            if (npc.eveningMed[i].isActive)
                            {
                                evening3.GetComponent<Text>().color = Color.green;
                            }
                            else
                            {
                                evening3.GetComponent<Text>().color = Color.red;
                            }
                        }
                        break;
                    case 3:
                        evening4.text = npc.eveningMed[i].dosage.ToString();
                        if (clock.currentDayTime == ClockTime.DayTime.EVENING)
                        {
                            if (npc.eveningMed[i].isActive)
                            {
                                evening4.GetComponent<Text>().color = Color.green;
                            }
                            else
                            {
                                evening4.GetComponent<Text>().color = Color.red;
                            }
                        }
                        break;
                }
            }
        }
        for (int i = 0; i < npc.nightMed.Length; i++)
        {
            if (string.IsNullOrEmpty(npc.nightMed[i].title))
            {
                switch (i)
                {
                    case 0:
                        night1.text = null;
                        break;
                    case 1:
                        night2.text = null;
                        break;
                    case 2:
                        night3.text = null;
                        break;
                    case 3:
                        night4.text = null;
                        break;
                }
            }
            else
            {
                switch (i)
                {
                    case 0:
                        night1.text = npc.nightMed[i].dosage.ToString();
                        if (clock.currentDayTime == ClockTime.DayTime.NIGHT)
                        {
                            if (npc.nightMed[i].isActive)
                            {
                                night1.GetComponent<Text>().color = Color.green;
                            }
                            else
                            {
                                night1.GetComponent<Text>().color = Color.red;
                            }
                        }
                        break;
                    case 1:
                        night2.text = npc.nightMed[i].dosage.ToString();
                        if (clock.currentDayTime == ClockTime.DayTime.NIGHT)
                        {
                            if (npc.nightMed[i].isActive)
                            {
                                night2.GetComponent<Text>().color = Color.green;
                            }
                            else
                            {
                                night2.GetComponent<Text>().color = Color.red;
                            }
                        }
                        break;
                    case 2:
                        night3.text = npc.nightMed[i].dosage.ToString();
                        if (clock.currentDayTime == ClockTime.DayTime.NIGHT)
                        {
                            if (npc.nightMed[i].isActive)
                            {
                                night3.GetComponent<Text>().color = Color.green;
                            }
                            else
                            {
                                night3.GetComponent<Text>().color = Color.red;
                            }
                        }
                        break;
                    case 3:
                        night4.text = npc.nightMed[i].dosage.ToString();
                        if (clock.currentDayTime == ClockTime.DayTime.NIGHT)
                        {
                            if (npc.nightMed[i].isActive)
                            {
                                night4.GetComponent<Text>().color = Color.green;
                            }
                            else
                            {
                                night4.GetComponent<Text>().color = Color.red;
                            }
                        }
                        break;
                }
            }

        }

    }

    public void DisableTextBoxNotDiagnozed()
    {

        myNameText.text = null;
        myIdText.text = null;
        myProblemText.text = null;

        targetPanel.SetActive(false);
        healthBar.SetActive(false);
        happyBar.SetActive(false);
        MyImage.SetActive(false);
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
        med1.text = null;
        med2.text = null;
        med3.text = null;
        med4.text = null;

        morning1.text = null;
        night1.text = null;
        evening1.text = null;
        afternoon1.text = null;

        morning2.text = null;
        night2.text = null;
        evening2.text = null;
        afternoon2.text = null;

        morning3.text = null;
        night3.text = null;
        evening3.text = null;
        afternoon3.text = null;

        morning4.text = null;
        night4.text = null;
        evening4.text = null;
        afternoon4.text = null;

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
