﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Minigame1 : MonoBehaviour {


    ClockTime.DayTime time;
    bool active = false;
    GameObject invObj;
    Inventory playerInv;
    ItemDatabase database;
    GameObject uiManagerObj;
    UIManager uiManager;
    GameObject minigameCanvas;
    GameObject minigameCanvas2;
    GameObject uiCanvas;
    GameObject player;
    GameObject textBoxManager;
    GameObject npcManagerObj;
    NPCManagerV2 npcManager;
    List<GameObject> npcList;
    int currNpc;
    public bool kasiDesi;
   
    /* med card stuff */
    public GameObject medCardPanel = null;
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

    CameraMovement mCamera;

    /* animation stuff */
    public GameObject kasDesObj;
    Animation kasDesAnim;
    bool spawnDrops;
    public GameObject[] drops;

    void Start()
    {
        
        invObj = GameObject.Find("Inventory");
        playerInv = invObj.GetComponent<Inventory>();
        database = invObj.GetComponent<ItemDatabase>();
        minigameCanvas = GameObject.FindGameObjectWithTag("Minigame1Canvas");
        minigameCanvas2 = GameObject.FindGameObjectWithTag("Minigame1Canvas2");
        minigameCanvas.SetActive(false);
        minigameCanvas2.SetActive(false);
        uiManagerObj = GameObject.FindGameObjectWithTag("UIManager");
        uiManager = uiManagerObj.GetComponent<UIManager>();
        uiCanvas = GameObject.FindGameObjectWithTag("UICanvas");
        player = GameObject.FindGameObjectWithTag("Player");
        textBoxManager = GameObject.FindGameObjectWithTag("TextBoxManager");
        npcManagerObj = GameObject.FindGameObjectWithTag("NPCManager");
        npcManager = npcManagerObj.GetComponent<NPCManagerV2>();
        mCamera = GameObject.Find("Main Camera").GetComponent<CameraMovement>();
        npcList = new List<GameObject>();
        
    }

    void Update()
    {
        if (active)
        {
            if (!kasDesAnim.IsPlaying("Drop") && spawnDrops)
            {
                EnableDropsAnim(false);
                spawnDrops = false;
            }
        }
    }

    public void startMinigame()
    {
        kasiDesi = false;
        time = GameObject.FindGameObjectWithTag("Clock").GetComponent<ClockTime>().currentDayTime;
        uiManager.pause(true);
        minigameCanvas.SetActive(true);
        uiCanvas.SetActive(false);
        kasDesObj = GameObject.FindGameObjectWithTag("KasDes");
        kasDesAnim = kasDesObj.GetComponent<Animation>();
        drops = GameObject.FindGameObjectsWithTag("Drop");
        spawnDrops = true;
        active = true;

        player.GetComponent<PlayerControl>().enabled = false;
        foreach (GameObject n in npcManager.npcList)
        {
            if (n != null)
            {
                if(n.GetComponent<NPCV2>().diagnosed)
                {
                    npcList.Add(n);
                }
            }
        }
        currNpc = 0;
        GetComponent<MedCabInventory>().Init();
        if (npcList.Count == 0)
            return;
        GameObject npcObj = npcList[currNpc];
        NPCV2 npc = npcObj.GetComponent<NPCV2>();
        showMedCard(npc);   
    }

    public void quitMinigame()
    {
        AddCupsToInv();
        GetComponent<MedCabInventory>().Reset();
        active = false;
        uiCanvas.SetActive(true);
        minigameCanvas.SetActive(false);
        minigameCanvas2.SetActive(false);    
        uiManager.pause(false);
        player.GetComponent<PlayerControl>().enabled = true;
        EnableDropsAnim(true);
    }

    public void nextNPC()
    {
        if (npcList.Count == 0)
            return;
        GameObject npcObj;
        // if reached end of npc list, go to beginning
        if (currNpc == npcList.Count - 1)
        {
            currNpc = 0;
            npcObj = npcList[currNpc];
        }           
        else
        {
            currNpc++;
            npcObj = npcList[currNpc];
        }            
        NPCV2 npc = npcObj.GetComponent<NPCV2>();
        showMedCard(npc);
    }

    public void prevNPC()
    {
        if (npcList.Count == 0)
            return;
        GameObject npcObj;
        // if reached beginning of npc list, go to end
        if (currNpc == 0)
        {
            currNpc = npcList.Count - 1;
            npcObj = npcList[currNpc];
        }     
        else
        {
            currNpc--;
            npcObj = npcList[currNpc];
        }           
        NPCV2 npc = npcObj.GetComponent<NPCV2>();
        showMedCard(npc);
    }

    public void kasiVitunDesi()
    {
        kasDesObj.GetComponent<KasiDesi>().StopBlinking();
        kasDesObj.GetComponent<KasiDesi>().SetDefaultColor();
        kasiDesi = true;
        EnableDropsAnim(true);
        kasDesAnim.Play();
        spawnDrops = true;    
    }

    public void showMedCard(NPCV2 npc)
    {
        if(npcList.Count == 0)
        {
            return;
        }
        medCardPanel.SetActive(true);
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
                        if (time == ClockTime.DayTime.MORNING)
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
                        if (time == ClockTime.DayTime.MORNING)
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
                        if (time == ClockTime.DayTime.MORNING)
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
                        if (time == ClockTime.DayTime.MORNING)
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
                        if (time == ClockTime.DayTime.AFTERNOON)
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
                        if (time == ClockTime.DayTime.AFTERNOON)
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
                        if (time == ClockTime.DayTime.AFTERNOON)
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
                        if (time == ClockTime.DayTime.AFTERNOON)
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
                        if (time == ClockTime.DayTime.EVENING)
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
                        if (time == ClockTime.DayTime.EVENING)
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
                        if (time == ClockTime.DayTime.EVENING)
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
                        if (time == ClockTime.DayTime.EVENING)
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
                        if (time == ClockTime.DayTime.NIGHT)
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
                        if (time == ClockTime.DayTime.NIGHT)
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
                        if (time == ClockTime.DayTime.NIGHT)
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
                        if (time == ClockTime.DayTime.NIGHT)
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

    public void startDosingGame(string medName, int defaultDosage, int canSplit)
    {
        mCamera.SwitchToMinigame1Camera();
        gameObject.transform.Find("BigMedCont").GetComponent<BigMedCont>().Init(medName, defaultDosage, canSplit);
        minigameCanvas.SetActive(false);
        minigameCanvas2.SetActive(true);
        string splittable;
        if (canSplit == 0)
            splittable = "Non-splittable";
        else
            splittable = "Splittable";
        GameObject.FindGameObjectWithTag("MedInfo").GetComponent<Text>().text = medName + " " + defaultDosage + " mg" + "\n" + splittable;
    }

    public void quitDosingGame()
    {
        gameObject.transform.Find("BigMedCont").GetComponent<BigMedCont>().spawnPills = false;
        GameObject pill = GameObject.FindGameObjectWithTag("Pill");
        if (pill != null)
        {
            Destroy(pill);
        }
        mCamera.SwitchToMainCamera();
        minigameCanvas.SetActive(true);
        minigameCanvas2.SetActive(false);
    }

    public void AddCupsToInv()
    {
        GameObject morningCupObj = GameObject.FindGameObjectWithTag("morningCup");
        MedCup morningCup = morningCupObj.GetComponent<MedCup>();

        GameObject afternoonCupObj = GameObject.FindGameObjectWithTag("afternoonCup");
        MedCup afternoonCup = afternoonCupObj.GetComponent<MedCup>();

        GameObject eveningCupObj = GameObject.FindGameObjectWithTag("eveningCup");
        MedCup eveningCup = eveningCupObj.GetComponent<MedCup>();

        GameObject nightCupObj = GameObject.FindGameObjectWithTag("nightCup");
        MedCup nightCup = nightCupObj.GetComponent<MedCup>();

        if (morningCup.medsInThisCup.Count > 0)
            playerInv.AddItems(morningCup.medsInThisCup);

        if (afternoonCup.medsInThisCup.Count > 0)
            playerInv.AddItems(afternoonCup.medsInThisCup);

        if (eveningCup.medsInThisCup.Count > 0)
            playerInv.AddItems(eveningCup.medsInThisCup);

        if (nightCup.medsInThisCup.Count > 0)
            playerInv.AddItems(nightCup.medsInThisCup);

        morningCup.Reset();
        afternoonCup.Reset();
        eveningCup.Reset();
        nightCup.Reset();
    }

    /*
    public void ShowMedsInCups()
    {
        GameObject medCupPanel = GameObject.FindGameObjectWithTag("MedCupPanel");
        string mText = "Empty";
        string aText = "Empty";
        string eText = "Empty";
        string nText = "Empty";

        if (morningCupMedsNoDuplicates.Count > 0)
        {
            mText = "";
            for (int i = 0; i < morningCupMedsNoDuplicates.Count; i++)
            {
                mText += morningCupMedsNoDuplicates[i].ToString() + "\n";
            }
        }

        if (afternoonCupMedsNoDuplicates.Count > 0)
        {
            aText = "";
            for (int i = 0; i < afternoonCupMedsNoDuplicates.Count; i++)
            {
                aText += afternoonCupMedsNoDuplicates[i].ToString() + "\n";
            }
        }

        if (eveningCupMedsNoDuplicates.Count > 0)
        {
            eText = "";
            for (int i = 0; i < eveningCupMedsNoDuplicates.Count; i++)
            {
                eText += eveningCupMedsNoDuplicates[i].ToString() + "\n";
            }
        }

        if (nightCupMedsNoDuplicates.Count > 0)
        {
            nText = "";
            for (int i = 0; i < nightCupMedsNoDuplicates.Count; i++)
            {
                nText += nightCupMedsNoDuplicates[i].ToString() + "\n";
            }
        }
        medCupPanel.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = mText;
        medCupPanel.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = aText;
        medCupPanel.transform.GetChild(2).transform.GetChild(0).GetComponent<Text>().text = eText;
        medCupPanel.transform.GetChild(3).transform.GetChild(0).GetComponent<Text>().text = nText;
    }
    */

    void EnableDropsAnim(bool enable)
    {
        foreach (GameObject d in drops)
            d.SetActive(enable);
    }
}
