using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    // hard-coded pools for names and IDs, probably changed later
    string[] namePool = { "Aleksi", "Pekka", "Matti", "Kalle", "Jorma" };
    string[] hetuPool = { "111", "222", "333", "444", "555" };
    
    const int MAX_NPCS = 10; // change total amount of NPCs according to difficulty level later

    // Use this for initialization
    void Start()
    {
        npcList = new List<GameObject>();
        spawnPoint = new Vector3(-265, 0, 125);
        spawnTime = 5; //Random.Range(0, 20); // spawn new NPC somewhere between 0 and 20 seconds
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
                spawnTime = 5; //Random.Range(0, 1);
                spawnNPC();
            }
        }
    }

    void spawnNPC()
    {
        int nimiIndex = Random.Range(0, namePool.Length);
        int hetuIndex = Random.Range(0, hetuPool.Length);
        string nimi = namePool[nimiIndex];
        string hetu = hetuPool[hetuIndex];

        GameObject newNpc = Instantiate(npcPrefab, spawnPoint, Quaternion.identity) as GameObject; // the method that copies the prefab object
        newNpc.GetComponent<NPC>().Init(nimi, hetu); // initialize the npc
        npcList.Add(newNpc);
    }
}
