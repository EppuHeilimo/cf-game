using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

public class NPCManager : MonoBehaviour
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
    
    const int MAX_NPCS = 10; // change total amount of NPCs according to difficulty level later

    List<string> usedIds; // IDs already used

    // Use this for initialization
    void Start()
    {
        npcList = new List<GameObject>();
        spawnPoint = new Vector3(-265, 0, 125);
        spawnTime = 5; //Random.Range(5, 20); // spawn new NPC somewhere between 5 and 20 seconds
        usedIds = new List<string>();
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
                spawnTime = 5; //Random.Range(5, 20); // spawn new NPC somewhere between 5 and 20 seconds
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
        newNpc.GetComponent<NPC>().Init(myName, myId); // initialize the npc
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
        if (!usedIds.Contains(builder.ToString()))
        {
            usedIds.Add(builder.ToString());
            return builder.ToString();
        }
        else
            return RandomString(3);
    }
}
