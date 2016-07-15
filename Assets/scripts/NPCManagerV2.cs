using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public struct NPCINFO
{
    public string name;
    public Sprite headimage;
    public bool dead;

    public NPCINFO(string name, Sprite head, bool dead)
    {
        this.name = name;
        headimage = head;
        this.dead = dead;
    }
}

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
    Vector3 nurseSpawn2 = new Vector3(685, 0, -845);
    Vector3 nurseSpawn = new Vector3(733, 0, -832);
    List<KeyValuePair<GameObject, GameObject>> nurses = new List<KeyValuePair<GameObject, GameObject>>();
    List<KeyValuePair<GameObject, GameObject>> doomednurses = new List<KeyValuePair<GameObject, GameObject>>();

    public List<NPCINFO> respNpcsWhoLeftOrDied = new List<NPCINFO>();

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

    // name pools for males and females, MUST BE SAME LENGTH!
    // name pools for males and females, MUST BE SAME LENGTH!
    const int NAMEPOOL_LENGTH = 40;
    string[] namePoolMale = new string[NAMEPOOL_LENGTH] { "Aleksi", "Pekka", "Matti", "Kalle", "Jorma", "Risto", "Torsti", "Markus", "Antti", "Pentti", "Pertti", "Vertti", "Jussi", "Janne", "Kari", "Tommi", "Timo", "Tomi", "Toni", "Roni", "Henri", "Tuomas", "Miro", "Mikko", "Johannes", "Risto", "Panu", "Åke", "Oskari", "Vesa", "Ville", "Kalevi", "Väinö", "Mikael", "Pasi", "Henrik", "Sami", "Teemu", "Anton", "Riku" };
    string[] namePoolFemale = new string[NAMEPOOL_LENGTH] { "Anna", "Anni", "Aino", "Kaisa", "Veera", "Vilma", "Iina", "Tiina", "Elsa", "Elisa", "Milja", "Mari", "Maria", "Hanna", "Sanna", "Saana", "Saara", "Sara", "Noora", "Laura", "Mia", "Henna", "Leena", "Päivi", "Hannele", "Ella", "Emilia", "Piia", "Anneli", "Birgitta", "Henrietta", "Aune", "Martta", "Taina", "Tuija", "Tuire", "Katariina", "Susanna", "Saija", "Eija" };

    const int MAX_NPCS = 30;
    public static int MAX_NPCS_IN_WARD_AREA = 12;
    public int currentNpcsInWard = 0;

    List<string> usedIds; // IDs already used

    // for generating random problem to patient from database
    GameObject invObj;
    ItemDatabase database;
    ClockTime clock;

    Queue<GameObject> docQueue = new Queue<GameObject>();

    public bool docBusy = false;

    // head change
    List<GameObject> headsMale = new List<GameObject>();
    List<GameObject> headsFemale = new List<GameObject>();
    const int NUM_FEMALE_HEADS = 30;
    const int NUM_MALE_HEADS = 18;

    Tutorial tutorial;

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
        tutorial = GameObject.Find("Tutorial").GetComponent<Tutorial>();
        LoadHeads();
    }

    void LoadHeads()
    {
        // load heads from Resources-folder
        if (headsFemale.Count == 0)
        {
            Object[] headstemp = Resources.LoadAll("heads/female");
            for (int i = 0; i < NUM_FEMALE_HEADS; i++)
            {
                headsFemale.Add((GameObject)headstemp[i]);
            }
        }
        if (headsMale.Count == 0)
        {
            Object[] headstemp = Resources.LoadAll("heads/male");
            for (int i = 0; i < NUM_MALE_HEADS; i++)
            {
                headsMale.Add((GameObject)headstemp[i]);
            }
        }
    }

    public void deleteNpcFromList(GameObject go)
    {
        if (go.GetComponent<NPCV2>().diagnosed)
            currentNpcsInWard--;
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

    public GameObject spawnTutorialGuy()
    {
        return spawnNPC();
    }

    // Update is called once per frame
    void Update()
    {
        if(!paused)
        {
            if(tutorial.tutorialOn)
            {
                //do something if needed
            }
            else
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
            if (nurses.Count > 0)
            {

                for (int i = 0; i < nurses.Count; i++)
                {
                    bool ready = false;
                    ready = nurses[i].Key.GetComponent<NurseAI>().allDone && nurses[i].Value.GetComponent<NurseAI>().allDone;
                    if (ready)
                    {
                        Destroy(nurses[i].Key);
                        Destroy(nurses[i].Value);
                        nurses.RemoveAt(i);
                    }
                }
            }
        }
    }

    public GameObject spawnNPC()
    {
        int nameIndex = Random.Range(0, NAMEPOOL_LENGTH);
        string myId = CreateID();
        int myGender = Random.Range(0, 2);
        string myName = "";
        if (myGender == 0)
            myName = namePoolFemale[nameIndex];
        else
            myName = namePoolMale[nameIndex];

        GameObject newNpc = Instantiate(npcPrefab, spawnPoint, Quaternion.identity) as GameObject; // the method that copies the prefab object
        newNpc.GetComponent<NPCV2>().Init(myName, myId, myGender); // initialize the npc

        //change head according to gender
        string headname = "";
        if (myGender == 0)
            headname = newNpc.GetComponent<HeadChange>().ChangeHead(headsFemale);
        else
            headname = newNpc.GetComponent<HeadChange>().ChangeHead(headsMale);

        //set the file name of the 2d sprite of the head
        newNpc.GetComponent<NPCV2>().myHead2d = Resources.Load<Sprite>("Sprites/heads/" + headname + ".2d");  
        npcList.Add(newNpc);
        return newNpc;
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
        if (go.GetComponent<NPCV2>().responsibilityIndicator != null)
            Destroy(go.GetComponent<NPCV2>().responsibilityIndicatorclone);
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

    public void nextDay()
    {
        if(tutorial.tutorialOn)
        {
            GameObject[] npcs = npcList.ToArray();
            for (int i = 0; i < npcs.Length; i++)
            {
                if(npcs[i].GetComponent<NPCV2>().playersResponsibility)
                {
                    removeNpcFromPlayersResponsibilities(npcs[i]);
                }
                if(npcs[i].GetComponent<NPCV2>().myBed != null)
                    GameObject.FindGameObjectWithTag("ObjectManager").GetComponent<ObjectManager>().unbookObject(npcList[i].GetComponent<NPCV2>().myBed);
                if (npcs[i].GetComponent<NPCV2>().getTarget() != null)
                    GameObject.FindGameObjectWithTag("ObjectManager").GetComponent<ObjectManager>().unbookObject(npcList[i].GetComponent<NPCV2>().getTarget());
                
                npcList.Remove(npcs[i]);
                Destroy(npcs[i]);
            }
        }
        else
        {
            GameObject[] npcs = npcList.ToArray();
            for (int i = 0; i < npcs.Length; i++)
            {
                NPCV2 npc = npcs[i].GetComponent<NPCV2>();
                if (!npc.diagnosed || npc.myState == NPCV2.NPCState.STATE_DEAD || npc.myState == NPCV2.NPCState.STATE_LEAVE_HOSPITAL)
                {
                    if (npc.getTarget() != null)
                    {
                        GameObject.FindGameObjectWithTag("ObjectManager").GetComponent<ObjectManager>().unbookObject(npc.getTarget());
                    }
                    if (npc.myBed != null)
                    {
                        GameObject.FindGameObjectWithTag("ObjectManager").GetComponent<ObjectManager>().unbookObject(npc.myBed);
                    }
                    if (npc.playersResponsibility)
                    {
                        removeNpcFromPlayersResponsibilities(npc.gameObject);
                    }
                    deleteNpcFromList(npcs[i]);
                    Destroy(npcs[i]);
                }
                else if (npc.diagnosed)
                {
                    npc.addStateToQueue(3, NPCV2.NPCState.STATE_DAY_CHANGE);
                    npc.taskCompleted = true;
                }
            }
        }
        
        if(nurses.Count > 0)
        {
            for (int i = 0; i < nurses.Count; i++)
            {
                Destroy(nurses[i].Key);
                Destroy(nurses[i].Value);
                nurses.RemoveAt(i);
            }
        }
        if(respNpcsWhoLeftOrDied.Count > 0)
        {
            respNpcsWhoLeftOrDied.Clear();
        }
        paused = true;
    }

    public void nextDayResume()
    {
        docBusy = false;
        paused = false;
        for (int i = 0; i < npcList.Count; i++)
        {
            NPCV2 npc = npcList[i].GetComponent<NPCV2>();
            npc.stopDayReset();
        }
        if(!tutorial.tutorialOn)
            targetResponsibilityLevel++;

        /*
         * fulfill players responsibility level (If not enough npcs in ward, the new ones from QUE will be added)
        */

        //how many more responsibility patients are needed
        int sub = targetResponsibilityLevel - responsibilityNpcs.Count;
        
        if(sub > 0)
        {
            int i = 0;
            foreach (GameObject go in npcList)
            {
                NPCV2 npc = go.GetComponent<NPCV2>();
                if (npc.diagnosed && !npc.playersResponsibility)
                {
                    npc.addNpcToResponsibilities();
                    //reset health for the new patient
                    npc.myHp = 50;
                    i++;
                }
                if (i == sub)
                {
                    break;
                }
            }
        } 
    }

    public Item getItemByID(int id)
    {
        return database.FetchItemByID(id);
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


    string CreateID()
    {
        // pp
        int randDayInt;
        string randDayStr;
        randDayInt = Random.Range(1, 31);
        if (randDayInt < 10)
            randDayStr = "0" + randDayInt.ToString();
        else
            randDayStr = randDayInt.ToString();

        // kk
        int randMonthInt;
        string randMonthStr;
        randMonthInt = Random.Range(1, 13);
        if (randMonthInt < 10)
            randMonthStr = "0" + randMonthInt.ToString();
        else
            randMonthStr = randMonthInt.ToString();

        // vv
        int randYearInt;
        string randYearStr;
        randYearInt = Random.Range(40, 100);
        randYearStr = randYearInt.ToString();

        // y = -

        // nnn
        int randNumInt;
        string randNumStr;
        randNumInt = Random.Range(2, 900);
        if (randNumInt < 10)
            randNumStr = "00" + randNumInt.ToString();
        else if (randNumInt >= 10 && randNumInt < 100)
            randNumStr = "0" + randNumInt.ToString();
        else
            randNumStr = randNumInt.ToString();

        // t
        char[] tArr = new char[16] { 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'F' };
        int randCharInt = Random.Range(0, 16);
        char t = tArr[randCharInt];


        string id = randDayStr + randMonthStr + randYearStr + "-" + randNumStr + t;
        for (int i = 0; i < usedIds.Count; i++)
        {
            if (usedIds[i] == id)
                return CreateID();
        }
        usedIds.Add(id);
        return id;
    }

    public bool spawnNurseToFetchNPC(GameObject npc)
    {
        if(nurses.Count < 4)
        {
            if (npc == null)
                return false;
            GameObject newNurse = Instantiate(nurseWithTrolleyPrefab, nurseSpawn, Quaternion.identity) as GameObject;
            GameObject newNurse2 = Instantiate(nursePrefab, nurseSpawn2, Quaternion.identity) as GameObject;
            newNurse.GetComponent<HeadChange>().ChangeHead(headsFemale);
            newNurse2.GetComponent<HeadChange>().ChangeHead(headsMale);
            newNurse.GetComponent<NurseAI>().partner = newNurse2.GetComponent<NurseAI>();
            newNurse2.GetComponent<NurseAI>().partner = newNurse.GetComponent<NurseAI>();
            newNurse.GetComponent<NurseAI>().Init(npc, 0);
            newNurse2.GetComponent<NurseAI>().Init(npc, 1);
            nurses.Add(new KeyValuePair<GameObject, GameObject>(newNurse, newNurse2));
            return true;
        }
        return false;
    }
}