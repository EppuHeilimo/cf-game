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
    //NoDuplicates
    List<MedCup.Med> morningCupMedsNoDuplicates = new List<MedCup.Med>();
    List<MedCup.Med> afternoonCupMedsNoDuplicates = new List<MedCup.Med>();
    List<MedCup.Med> eveningCupMedsNoDuplicates = new List<MedCup.Med>();
    List<MedCup.Med> nightCupMedsNoDuplicates = new List<MedCup.Med>();

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
        ClearCups();
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
                CombineMeds(0);
                cup.GetComponent<MedCup>().medsInThisCup.Clear();
            }

            if (cup.GetComponent<MedCup>().cupName == "afternoon")
            {
                foreach (MedCup.Med m in cup.GetComponent<MedCup>().medsInThisCup)
                    afternoonCupMeds.Add(m);
                CombineMeds(1);
                cup.GetComponent<MedCup>().medsInThisCup.Clear();
            }

            if (cup.GetComponent<MedCup>().cupName == "evening")
            {
                foreach (MedCup.Med m in cup.GetComponent<MedCup>().medsInThisCup)
                    eveningCupMeds.Add(m);
                CombineMeds(2);
                cup.GetComponent<MedCup>().medsInThisCup.Clear();
            }

            if (cup.GetComponent<MedCup>().cupName == "night")
            {
                foreach (MedCup.Med m in cup.GetComponent<MedCup>().medsInThisCup)
                    nightCupMeds.Add(m);
                CombineMeds(3);
                cup.GetComponent<MedCup>().medsInThisCup.Clear();
            }
        }

        mCamera.SwitchToMainCamera();
        minigameCanvas.SetActive(true);
        minigameCanvas2.SetActive(false);
        ShowMedsInCups();
    }

    public void AddCupsToInv()
    {
        if (morningCupMedsNoDuplicates.Count > 0)
            playerInv.AddItems(morningCupMedsNoDuplicates);

        if (afternoonCupMedsNoDuplicates.Count > 0)
            playerInv.AddItems(afternoonCupMedsNoDuplicates);

        if (eveningCupMedsNoDuplicates.Count > 0)
            playerInv.AddItems(eveningCupMedsNoDuplicates);

        if (nightCupMedsNoDuplicates.Count > 0)
            playerInv.AddItems(nightCupMedsNoDuplicates);
    }

    void CombineMeds(int id)
    {
        switch (id)
        {
            case 0:
                // duplicates -> add up dosage
                if (morningCupMeds.Count > 0)
                {
                    foreach (MedCup.Med m in morningCupMeds)
                    {
                        if (morningCupMedsNoDuplicates.Count == 0)
                            morningCupMedsNoDuplicates.Add(m);
                        else
                        {
                            bool found = false;
                            for (int i = 0; i < morningCupMedsNoDuplicates.Count; i++)
                            {
                                if (morningCupMedsNoDuplicates[i].name == m.name)
                                {
                                    morningCupMedsNoDuplicates[i].dosage += m.dosage;
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                                morningCupMedsNoDuplicates.Add(m);
                        }
                    }
                }
                morningCupMeds.Clear();
                break;

            case 1:
                if (afternoonCupMeds.Count > 0)
                {
                    foreach (MedCup.Med m in afternoonCupMeds)
                    {
                        if (afternoonCupMedsNoDuplicates.Count == 0)
                            afternoonCupMedsNoDuplicates.Add(m);
                        else
                        {
                            bool found = false;
                            for (int i = 0; i < afternoonCupMedsNoDuplicates.Count; i++)
                            {
                                if (afternoonCupMedsNoDuplicates[i].name == m.name)
                                {
                                    afternoonCupMedsNoDuplicates[i].dosage += m.dosage;
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                                afternoonCupMedsNoDuplicates.Add(m);
                        }
                    }
                }
                afternoonCupMeds.Clear();
                break;

            case 2:
                if (eveningCupMeds.Count > 0)
                {
                    foreach (MedCup.Med m in eveningCupMeds)
                    {
                        if (eveningCupMedsNoDuplicates.Count == 0)
                            eveningCupMedsNoDuplicates.Add(m);
                        else
                        {
                            bool found = false;
                            for (int i = 0; i < eveningCupMedsNoDuplicates.Count; i++)
                            {
                                if (eveningCupMedsNoDuplicates[i].name == m.name)
                                {
                                    eveningCupMedsNoDuplicates[i].dosage += m.dosage;
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                                eveningCupMedsNoDuplicates.Add(m);
                        }
                    }
                }
                eveningCupMeds.Clear();
                break;

            case 3:
                if (nightCupMeds.Count > 0)
                {
                    foreach (MedCup.Med m in nightCupMeds)
                    {
                        if (nightCupMedsNoDuplicates.Count == 0)
                            nightCupMedsNoDuplicates.Add(m);
                        else
                        {
                            bool found = false;
                            for (int i = 0; i < nightCupMedsNoDuplicates.Count; i++)
                            {
                                if (nightCupMedsNoDuplicates[i].name == m.name)
                                {
                                    nightCupMedsNoDuplicates[i].dosage += m.dosage;
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                                nightCupMedsNoDuplicates.Add(m);
                        }
                    }
                }
                nightCupMeds.Clear();
                break;
        }     
    }

    public void ClearCups()
    {     
        morningCupMeds.Clear();
        morningCupMedsNoDuplicates.Clear();
        afternoonCupMeds.Clear();
        afternoonCupMedsNoDuplicates.Clear();
        eveningCupMeds.Clear();
        eveningCupMedsNoDuplicates.Clear();
        nightCupMeds.Clear();
        nightCupMedsNoDuplicates.Clear();
        GameObject medCupPanel = GameObject.FindGameObjectWithTag("MedCupPanel");
        medCupPanel.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "Empty";
        medCupPanel.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = "Empty";
        medCupPanel.transform.GetChild(2).transform.GetChild(0).GetComponent<Text>().text = "Empty";
        medCupPanel.transform.GetChild(3).transform.GetChild(0).GetComponent<Text>().text = "Empty";
    }

    public void ClearCup(int id)
    {
        switch (id)
        {
            case 0:
                morningCupMeds.Clear();
                ShowMedsInCups();
                break;
            case 1:
                afternoonCupMeds.Clear();
                ShowMedsInCups();
                break;
            case 2:
                eveningCupMeds.Clear();
                ShowMedsInCups();
                break;
            case 3:
                nightCupMeds.Clear();
                ShowMedsInCups();
                break;
        }
    }

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
}
