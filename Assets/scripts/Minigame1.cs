using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Minigame1 : MonoBehaviour {

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

    /* dosage game stuff */
    CameraMovement mCamera;
    public List<MedCup.Med> morningCupMeds = new List<MedCup.Med>();
    public List<MedCup.Med> afternoonCupMeds = new List<MedCup.Med>();
    public List<MedCup.Med> eveningCupMeds = new List<MedCup.Med>();
    public List<MedCup.Med> nightCupMeds = new List<MedCup.Med>();

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
            //Logic here...
        }
    }

    public void startMinigame()
    {
        active = true;
        kasiDesi = false;
        uiManager.pause(true);
        minigameCanvas.SetActive(true);
        uiCanvas.SetActive(false);
        
        player.GetComponent<PlayerControl>().enabled = false;
        foreach (GameObject n in npcManager.npcList)
        {
            if(n.GetComponent<NPCV2>().diagnosed)
            {
                npcList.Add(n);
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
        kasiDesi = true;
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

    public void startDosingGame(string medName, int defaultDosage)
    {
        mCamera.SwitchToMinigame1Camera();
        gameObject.transform.Find("BigMedCont").GetComponent<BigMedCont>().Init(medName, defaultDosage);
        minigameCanvas.SetActive(false);
        minigameCanvas2.SetActive(true);
    }

    public void quitDosingGame()
    {
        GameObject[] cups = GameObject.FindGameObjectsWithTag("medCup");
        foreach (GameObject cup in cups)
        {
            if (cup.GetComponent<MedCup>().cupName == "morning")
            {
                foreach (MedCup.Med m in cup.GetComponent<MedCup>().medsInThisCup)
                    morningCupMeds.Add(m);
                cup.GetComponent<MedCup>().medsInThisCup.Clear();
            }

            if (cup.GetComponent<MedCup>().cupName == "afternoon")
            {
                foreach (MedCup.Med m in cup.GetComponent<MedCup>().medsInThisCup)
                    afternoonCupMeds.Add(m);
                cup.GetComponent<MedCup>().medsInThisCup.Clear();
            }

            if (cup.GetComponent<MedCup>().cupName == "evening")
            {
                foreach (MedCup.Med m in cup.GetComponent<MedCup>().medsInThisCup)
                    eveningCupMeds.Add(m);
                cup.GetComponent<MedCup>().medsInThisCup.Clear();
            }

            if (cup.GetComponent<MedCup>().cupName == "night")
            {
                foreach (MedCup.Med m in cup.GetComponent<MedCup>().medsInThisCup)
                    nightCupMeds.Add(m);
                cup.GetComponent<MedCup>().medsInThisCup.Clear();
            }
        }

        /* PRINT SHIT TEMPORARY! */
        foreach (MedCup.Med m in morningCupMeds)
        {
            
            print("AAMU KUPIN SISÄLTÖ: " + m.name + m.dosage);
        }

        foreach (MedCup.Med m in afternoonCupMeds)
        {
            print("PÄIVÄ KUPIN SISÄLTÖ: " + m.name + m.dosage);
        }

        foreach (MedCup.Med m in eveningCupMeds)
        {
            print("ILTA KUPIN SISÄLTÖ: " + m.name + m.dosage);
        }

        foreach (MedCup.Med m in nightCupMeds)
        {
            print("YÖ KUPIN SISÄLTÖ: " + m.name + m.dosage);
        }

        mCamera.SwitchToMainCamera();
        minigameCanvas.SetActive(true);
        minigameCanvas2.SetActive(false);
    }

    public void AddCupsToInv()
    {
        List<MedCup.Med> morningTmpList = new List<MedCup.Med>();
        List<MedCup.Med> afternoonTmpList = new List<MedCup.Med>();
        List<MedCup.Med> eveningTmpList = new List<MedCup.Med>();
        List<MedCup.Med> nightTmpList = new List<MedCup.Med>();

        // duplicates -> add up dosage
        if (morningCupMeds.Count > 0)
        { 
            foreach (MedCup.Med m in morningCupMeds)
            {
                if (morningTmpList.Count == 0)
                    morningTmpList.Add(m);
                else
                { 
                    foreach (MedCup.Med mm in morningTmpList)
                    {
                        if (mm.name == m.name)
                        {
                            mm.dosage += m.dosage;
                        }
                        else
                        {
                            morningTmpList.Add(m);
                        }
                    }
                }
            }
            playerInv.AddItems(morningTmpList);
        }

        // duplicates -> add up dosage
        if (afternoonCupMeds.Count > 0)
        {
            foreach (MedCup.Med m in afternoonCupMeds)
            {
                if (afternoonTmpList.Count == 0)
                    afternoonTmpList.Add(m);
                else
                {
                    foreach (MedCup.Med mm in afternoonTmpList)
                    {
                        if (mm.name == m.name)
                        {
                            mm.dosage += m.dosage;
                        }
                        else
                        {
                            afternoonTmpList.Add(m);
                        }
                    }
                }
            }
            playerInv.AddItems(afternoonTmpList);
        }

        // duplicates -> add up dosage
        if (eveningCupMeds.Count > 0)
        {
            foreach (MedCup.Med m in eveningCupMeds)
            {
                if (eveningTmpList.Count == 0)
                    eveningTmpList.Add(m);
                else
                {
                    foreach (MedCup.Med mm in eveningTmpList)
                    {
                        if (mm.name == m.name)
                        {
                            mm.dosage += m.dosage;
                        }
                        else
                        {
                            eveningTmpList.Add(m);
                        }
                    }
                }
            }
            playerInv.AddItems(afternoonTmpList);
        }

        // duplicates -> add up dosage
        if (nightCupMeds.Count > 0)
        {
            foreach (MedCup.Med m in nightCupMeds)
            {
                if (nightTmpList.Count == 0)
                    nightTmpList.Add(m);
                else
                {
                    foreach (MedCup.Med mm in nightTmpList)
                    {
                        if (mm.name == m.name)
                        {
                            mm.dosage += m.dosage;
                        }
                        else
                        {
                            nightTmpList.Add(m);
                        }
                    }
                }
            }
            playerInv.AddItems(eveningTmpList);
        }
    }
}
