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
    public Text morningInfo = null;
    public Text morningX = null;
    public Text afternoonInfo = null;
    public Text afternoonX = null;
    public Text eveningInfo = null;
    public Text eveningX = null;
    public Text nightInfo = null;
    public Text nightX = null;

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
        minigameCanvas.SetActive(true);
        uiManager.pause(true);
        player.GetComponent<PlayerControl>().enabled = false;
        npcList = npcManager.npcList;
        currNpc = 0;
        GameObject npcObj = npcList[currNpc];
        NPCV2 npc = npcObj.GetComponent<NPCV2>();
        showMedCard(npc.myName, npc.myId, npc.morningMed.title, npc.afternoonMed.title, npc.eveningMed.title, npc.nightMed.title, npc.morningDos, npc.afternoonDos, npc.eveningDos, npc.nightDos);
        kasiDesi = false;
        GetComponent<MedCabInventory>().Init();
    }

    public void quitMinigame()
    {
        active = false;
        minigameCanvas.SetActive(false);
        uiManager.pause(false);
        player.GetComponent<PlayerControl>().enabled = true;
    }

    public void nextNPC()
    {
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
        showMedCard(npc.myName, npc.myId, npc.morningMed.title, npc.afternoonMed.title, npc.eveningMed.title, npc.nightMed.title, npc.morningDos, npc.afternoonDos, npc.eveningDos, npc.nightDos);
    }

    public void prevNPC()
    {
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
        showMedCard(npc.myName, npc.myId, npc.morningMed.title, npc.afternoonMed.title, npc.eveningMed.title, npc.nightMed.title, npc.morningDos, npc.afternoonDos, npc.eveningDos, npc.nightDos);
    }

    public void kasiVitunDesi()
    {
        kasiDesi = true;
    }

    public void showMedCard(string myName, string myId, string morningMed, string afternoonMed, string eveningMed, string nightMed, int morningDos, int afternoonDos, int eveningDos, int nightDos)
    {
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
}
