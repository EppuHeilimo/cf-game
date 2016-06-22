using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Minigame1 : MonoBehaviour
{

    bool active = false;
    GameObject invObj;
    Inventory playerInv;
    ItemDatabase database;
    GameObject uiManagerObj;
    UIManager uiManager;
    GameObject minigameCanvas;
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
    public Text afternoonX = null;
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


    void Start()
    {
        invObj = GameObject.Find("Inventory");
        playerInv = invObj.GetComponent<Inventory>();
        database = invObj.GetComponent<ItemDatabase>();
        minigameCanvas = GameObject.FindGameObjectWithTag("Minigame1Canvas");
        minigameCanvas.SetActive(false);
        uiManagerObj = GameObject.FindGameObjectWithTag("UIManager");
        uiManager = uiManagerObj.GetComponent<UIManager>();
        uiCanvas = GameObject.FindGameObjectWithTag("UICanvas");
        player = GameObject.FindGameObjectWithTag("Player");
        textBoxManager = GameObject.FindGameObjectWithTag("TextBoxManager");
        npcManagerObj = GameObject.FindGameObjectWithTag("NPCManager");
        npcManager = npcManagerObj.GetComponent<NPCManagerV2>();
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
        minigameCanvas.SetActive(true);
        uiManager.pause(true);
        player.GetComponent<PlayerControl>().enabled = false;
        foreach(GameObject n in npcManager.npcList)
        {
            if(n.GetComponent<NPCV2>().diagnosed)
            {
                npcList.Add(n);
            }
        }
        npcList = npcManager.npcList;
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
        GetComponent<MedCabInventory>().Reset();
        active = false;
        minigameCanvas.SetActive(false);
        uiManager.pause(false);
        player.GetComponent<PlayerControl>().enabled = true;
        npcList.Clear();
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
}
