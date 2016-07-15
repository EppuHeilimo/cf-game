using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class ComputerButtons : MonoBehaviour {

    NPCV2 npc;

    public Text patientInfo = null;
    public Text med1 = null;
    public Text med2 = null;
    public Text med3 = null;
    public Text med4 = null;
    public Text morning1 = null;
    public Text morning2 = null;
    public Text morning3 = null;
    public Text morning4 = null;

    public Text evening1 = null;
    public Text evening2 = null;
    public Text evening3 = null;
    public Text evening4 = null;

    public Text afternoon1 = null;
    public Text afternoon2 = null;
    public Text afternoon3 = null;
    public Text afternoon4 = null;

    public Text night1 = null;
    public Text night2 = null;
    public Text night3 = null;
    public Text night4 = null;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void closeComputer()
    {
        GameObject.FindGameObjectWithTag("Computer").GetComponent<Computer>().ShutdownComputer();
    }

    public void OpenTaskWindow()
    {
        GameObject.FindGameObjectWithTag("Computer").GetComponent<Computer>().openTaskWindow();
    }
    public void OpenDBWindow()
    {
        npc = GameObject.FindGameObjectWithTag("Computer").GetComponent<Computer>().openDBWindow();
        showPatientMedCard();
    }
    public void CloseTaskWindow()
    {
        GameObject.FindGameObjectWithTag("Computer").GetComponent<Computer>().closeTasks();
    }
    public void CloseDBWindow()
    {
        GameObject.FindGameObjectWithTag("Computer").GetComponent<Computer>().closeDB();
    }

    public void UpButton()
    {
        npc = GameObject.FindGameObjectWithTag("Computer").GetComponent<Computer>().PrevPatient();
        showPatientMedCard();
    }

    public void DownButton()
    {
        npc = GameObject.FindGameObjectWithTag("Computer").GetComponent<Computer>().NextPatient();
        showPatientMedCard();
    }


    private void showPatientMedCard()
    {
        if (npc == null)
        {
            return;
        }

        patientInfo.text = "";
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
                        break;
                    case 1:
                        morning2.text = npc.morningMed[i].dosage.ToString();
                        break;
                    case 2:
                        morning3.text = npc.morningMed[i].dosage.ToString();
                        break;
                    case 3:
                        morning4.text = npc.morningMed[i].dosage.ToString();
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
                        break;
                    case 1:
                        afternoon2.text = npc.afternoonMed[i].dosage.ToString();
                        break;
                    case 2:
                        afternoon3.text = npc.afternoonMed[i].dosage.ToString();
                        break;
                    case 3:
                        afternoon4.text = npc.afternoonMed[i].dosage.ToString();
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
                        break;
                    case 1:
                        evening2.text = npc.eveningMed[i].dosage.ToString();
                        break;
                    case 2:
                        evening3.text = npc.eveningMed[i].dosage.ToString();
                        break;
                    case 3:
                        evening4.text = npc.eveningMed[i].dosage.ToString();
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
                        break;
                    case 1:
                        night2.text = npc.nightMed[i].dosage.ToString();
                        break;
                    case 2:
                        night3.text = npc.nightMed[i].dosage.ToString();
                        break;
                    case 3:
                        night4.text = npc.nightMed[i].dosage.ToString();
                        break;
                }
            }

        }
    }
    
}
