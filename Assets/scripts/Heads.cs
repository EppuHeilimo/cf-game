using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

/* Loads head assets */

public class Heads : MonoBehaviour {

    public List<GameObject> headsMale = new List<GameObject>();
    public List<GameObject> headsFemale = new List<GameObject>();
    const int NUM_FEMALE_HEADS = 30;
    const int NUM_MALE_HEADS = 18;

    //players current head
    public GameObject PlayersHead;

    // Use this for initialization
    void Start () {
        LoadHeads();
        Profile profile = GameObject.Find("SavedData").GetComponent<SavedData>().getProfile();
        if (profile != null)
        {
            if (profile.gender == 0)
            {
                PlayersHead = Resources.Load("heads/female/" + profile.head) as GameObject;
            }
            else
            {
                PlayersHead = Resources.Load("heads/male/" + profile.head) as GameObject;
            }
        }
        else
        {
            PlayersHead = Resources.Load("heads/female/head_f_1") as GameObject;
        }

    }

    public void setPlayersHead(GameObject go)
    {
        PlayersHead = go;
    }

    public GameObject getPlayersHead()
    {
        if(PlayersHead == null)
        {
            return headsMale[0];
        }
        return PlayersHead;
    }
	
	// Update is called once per frame
	void Update () {
	
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
}
