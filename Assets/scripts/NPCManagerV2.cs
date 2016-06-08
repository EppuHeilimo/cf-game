using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using System.Linq;

public class NPCManagerV2 : MonoBehaviour
{
    // serialized variable for linking to the prefab object
    [SerializeField]
    GameObject npcPrefab;

    // list to keep track of the NPC instances in the scene
    public List<GameObject> npcList;

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

    const int MAX_NPCS = 15; // ** MUST BE SAME AS MAX_QUE IN QUE MANAGER! **

    List<string> usedIds; // IDs already used

    // for generating random problem to patient from database
    GameObject invObj;
    ItemDatabase database;

    Queue<GameObject> docQueue = new Queue<GameObject>();

    int docque = 0;
    public bool docBusy = false;

    // Use this for initialization
    void Start()
    {
        npcList = new List<GameObject>();
        spawnPoint = new Vector3(-331, 0, -5);
        spawnTime = 5; //Random.Range(5, 20); // spawn new NPC somewhere between 5 and 20 seconds
        usedIds = new List<string>();
        invObj = GameObject.Find("Inventory");
        database = invObj.GetComponent<ItemDatabase>();
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
        // spawn new NPC
        timeSinceLastSpawn += Time.deltaTime;
        npcCount = npcList.Count;

        if (npcCount < MAX_NPCS)
        {
            if (timeSinceLastSpawn > spawnTime)
            {
                timeSinceLastSpawn = 0;
                spawnTime = 5; 
                spawnNPC();
            }
        }
    }

    void spawnNPC()
    {
        int nameIndex = UnityEngine.Random.Range(0, namePool.Length);
        string myName = namePool[nameIndex];
        string myId = RandomString(4);
        GameObject newNpc = Instantiate(npcPrefab, spawnPoint, Quaternion.identity) as GameObject; // the method that copies the prefab object
        // Randomize 1-4 DIFFERENT problems for the NPC
        int numProblems = UnityEngine.Random.Range(1, 5);
        Item[] randMeds = new Item[4];
        // Fetch random medicine items from database
        for (int i = 0; i < numProblems; i++)
        {
            randMeds[i] = RandomItem(randMeds);
        }
        newNpc.GetComponent<NPCV2>().Init(myName, myId); // initialize the npc
        newNpc.GetComponent<NPCV2>().InitMedication(randMeds);
        npcList.Add(newNpc);
    }

    private Item RandomItem(Item[] randMeds)
    {
        Item randItem = database.FetchItemByID(UnityEngine.Random.Range(0, database.database.Count));
        if (!randMeds.Contains(randItem))
            return randItem;
        else
            return RandomItem(randMeds);
    }

    private string RandomString(int size)
    {
        StringBuilder builder = new StringBuilder();
        System.Random random = new System.Random();
        char ch;
        for (int i = 0; i < size; i++)
        {
            ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
            builder.Append(ch);
        }

        // check if the ID was used already, if so, generate a new one
        if (!usedIds.Contains(builder.ToString()))
        {
            usedIds.Add(builder.ToString());
            return builder.ToString();
        }
        else
            return RandomString(3);
    }
}