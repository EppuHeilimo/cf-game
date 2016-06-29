using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class NPCManagerV2 : MonoBehaviour
{
    // serialized variable for linking to the prefab object
    [SerializeField]
    GameObject npcPrefab;
    //Nurse
    [SerializeField]
    GameObject nursePrefab;
    [SerializeField]
    GameObject nurseWithTrolleyPrefab;
    Vector3 nurseSpawn = new Vector3(733, 0, -742);
    Vector3 nurseSpawn2 = new Vector3(700, 0, -700);
    public bool nursesDeployed = false;

    public bool paused = false;

    // list to keep track of the NPC instances in the scene
    public List<GameObject> npcList;

    //List of npc which are player's responsibility
    public List<GameObject> responsibilityNpcs;

    //how many patients should player have
    int targetResponsibilityLevel = 3;

    // total amount of NPCs in the scene currently
    int npcCount;

    // spawn point for the NPCs
    Vector3 spawnPoint;

    // time elapsed since last NPC was spawned
    float timeSinceLastSpawn;

    // interval between spawning new NPCs
    float spawnTime;

    // hard-coded pool for names, probably changed later
    string[] namePool = { "Aleksi", "Pekka", "Matti", "Kalle", "Jorma" };

    const int MAX_NPCS = 30; 

    List<string> usedIds; // IDs already used

    // for generating random problem to patient from database
    GameObject invObj;
    ItemDatabase database;
    ClockTime clock;

    Queue<GameObject> docQueue = new Queue<GameObject>();

    int docque = 0;
    public bool docBusy = false;

    // Use this for initialization
    void Start()
    {
        npcList = new List<GameObject>();
        responsibilityNpcs = new List<GameObject>();
        spawnPoint = new Vector3(-331, 0, -5);
        spawnTime = 3; //Random.Range(5, 20); // spawn new NPC somewhere between 5 and 20 seconds
        usedIds = new List<string>();
        invObj = GameObject.Find("Inventory");
        database = invObj.GetComponent<ItemDatabase>();
        clock = GameObject.FindGameObjectWithTag("Clock").GetComponent<ClockTime>();
    }

    public void deleteNpcFromList(GameObject go)
    {
        npcList.Remove(go);
    }

    public bool isDocBusy()
    {
        return docBusy;
    }

    public void setDocBusy()
    {
        docBusy = true;
    }

    public void setDocFree()
    {
        docBusy = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!paused)
        {
            // spawn new NPC
            timeSinceLastSpawn += Time.deltaTime;
            npcCount = npcList.Count;

            if (npcCount < MAX_NPCS && (clock.currentDayTime == ClockTime.DayTime.MORNING || clock.currentDayTime == ClockTime.DayTime.AFTERNOON))
            {
                if (timeSinceLastSpawn > spawnTime)
                {
                    timeSinceLastSpawn = 0;
                    spawnTime = 3;
                    spawnNPC();
                }
            }
        }

    }

    void spawnNPC()
    {
        int nameIndex = UnityEngine.Random.Range(0, namePool.Length);
        string myName = namePool[nameIndex];
        string myId = RandomString(4);
        GameObject newNpc = Instantiate(npcPrefab, spawnPoint, Quaternion.identity) as GameObject; // the method that copies the prefab object
        newNpc.GetComponent<NPCV2>().Init(myName, myId); // initialize the npc
        //change to random head
        string headname = newNpc.GetComponent<HeadChange>().ChangeToRandomHead();
        //set the file name of the 2d sprite of the head
        newNpc.GetComponent<NPCV2>().myHead2d = Resources.Load<Sprite>("Sprites/heads/" + headname + ".2d");
        
        npcList.Add(newNpc);
    }

    public void addNpcToPlayersResponsibilities(GameObject go)
    {
        if(responsibilityNpcs.Count < targetResponsibilityLevel)
        {
            responsibilityNpcs.Add(go);
        }
    }

    public void removeNpcFromPlayersResponsibilities(GameObject go)
    {
        responsibilityNpcs.Remove(go);
    }

    public bool isPlayerResponsibilityLevelFulfilled()
    {
        if (responsibilityNpcs.Count < targetResponsibilityLevel)
        {
            return false;
        }
        return true;
    }

    public Item RandomItem(Item[] randMeds)
    {
        Item randItem = database.FetchItemByID(UnityEngine.Random.Range(0, database.database.Count));
        bool found = false;
        for (int i = 0; i < randMeds.Length; i++)
        {
            if (randMeds[i] != null)
            {
                if (randMeds[i].Title == randItem.Title)
                {
                    found = true;
                    break;
                }
            }
        }
        if (found)
            return RandomItem(randMeds);
        else
            return randItem;
    }

    private string RandomString(int length)
    {
        char[] c = new char[length];
        for (int i = 0; i < length; i++)
        {
            int num = Random.Range(0, 26);
            char let = (char)('a' + num);
            c[i] = let;
        }
        string s = new string(c);
        s = s.ToUpper();

        for (int i = 0; i < usedIds.Count; i++)
        {
            if (usedIds[i] == s)
                return RandomString(4);
        }
        usedIds.Add(s);
        return s;
    }

    public void spawnNurseToFetchNPC(GameObject npc)
    {

        GameObject newNurse = Instantiate(nurseWithTrolleyPrefab, nurseSpawn, Quaternion.identity) as GameObject;
        GameObject newNurse2 = Instantiate(nursePrefab, nurseSpawn2, Quaternion.identity) as GameObject;

        newNurse.GetComponent<HeadChange>().ChangeToRandomHead();
        newNurse2.GetComponent<HeadChange>().ChangeToRandomHead();
        newNurse.GetComponent<NurseAI>().partner = newNurse2.GetComponent<NurseAI>();
        newNurse2.GetComponent<NurseAI>().partner = newNurse.GetComponent<NurseAI>();

        newNurse.GetComponent<NurseAI>().Init(npc, 0);
        newNurse2.GetComponent<NurseAI>().Init(npc, 1);

        nursesDeployed = true;


    }
}