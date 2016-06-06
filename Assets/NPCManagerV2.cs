using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

public class NPCManagerV2 : MonoBehaviour
{
    // serialized variable for linking to the prefab object
    [SerializeField]
    GameObject npcPrefab;

    // list to keep track of the NPC instances in the scene
    List<GameObject> npcList;

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
    Dictionary<GameObject, GameObject> beds = new Dictionary<GameObject, GameObject>();
    Dictionary<GameObject, GameObject> queueChairs = new Dictionary<GameObject, GameObject>();

    // for generating random problem to patient from database
    GameObject invObj;
    ItemDatabase database;

    Queue<GameObject> docQueue = new Queue<GameObject>();

    // Use this for initialization
    void Start()
    {
        npcList = new List<GameObject>();
        spawnPoint = new Vector3(-331, 0, -5);
        spawnTime = 5; //Random.Range(5, 20); // spawn new NPC somewhere between 5 and 20 seconds
        usedIds = new List<string>();
        GameObject[] bedstemp = GameObject.FindGameObjectsWithTag("Bed");
        GameObject[] chairstemp = GameObject.FindGameObjectsWithTag("QueueChair");
        foreach (GameObject chair in chairstemp)
        {
            queueChairs.Add(chair, null);
        }
        foreach (GameObject bed in bedstemp)
        {
            beds.Add(bed, null);
        }
        invObj = GameObject.Find("Inventory");
        database = invObj.GetComponent<ItemDatabase>();
    }

    public GameObject bookBed(GameObject npc)
    {
        foreach (KeyValuePair<GameObject, GameObject> bed in beds)
        {
            if (bed.Value == null)
            {
                beds[bed.Key] = npc;
                return bed.Key;
            }
        }
        return null;
    }

    public void unbookBed(GameObject bed)
    {
        beds[bed] = null;
    }

    public GameObject bookChair(GameObject npc)
    {
        List<KeyValuePair<GameObject, GameObject>> temp = new List<KeyValuePair<GameObject, GameObject>>();
        foreach (KeyValuePair<GameObject, GameObject> chair in queueChairs)
        {
            if (chair.Value == null)
            {
                temp.Add(chair);
            }
        }
        if(temp.Count > 0)
        {
            System.Random rnd = new System.Random();
            int rand = rnd.Next(0, temp.Count);
            GameObject key = temp[rand].Key;
            queueChairs[key] = npc;
            return key;
        }
        return null;
    }

    public void unbookChair(GameObject chair)
    {
        queueChairs[chair] = null;
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
        // fetch random medicine item from database
        Item randItem = database.FetchItemByID(UnityEngine.Random.Range(0, database.database.Count));
        newNpc.GetComponent<NPCV2>().Init(myName, myId, randItem.Usage, randItem.Title); // initialize the npc
        npcList.Add(newNpc);
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
