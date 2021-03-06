﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;
using Assets.Scripts;

/* the big medicine container used in the slingshot minigame */
public class BigMedCont : MonoBehaviour
{
    public string medName;
    public int defaultDosage;
    public int canSplit;
    public GameObject pillPrefab;
    GameObject minigameObj;
    Sprite pillSprite;
    public bool spawnPills;
    MinigameManager gameManager;

    void Start()
    {
        minigameObj = GameObject.Find("Minigame1");
        gameManager = GameObject.Find("MinigameManager").GetComponent<MinigameManager>();
    }

    public void Init(string medName, int defaultDosage, int canSplit)
    {
        this.medName = medName;
        this.defaultDosage = defaultDosage;
        this.canSplit = canSplit;
        
        // load container sprite specific to this medicine
        Sprite medSprite = Resources.Load<Sprite>("Sprites/Meds/" + medName);
        if (medSprite == null)
            medSprite = Resources.Load<Sprite>("Sprites/Meds/null"); // sprite not found in the folder
        SpriteRenderer sRenderer = gameObject.GetComponent<SpriteRenderer>();
        sRenderer.sprite = medSprite;
        transform.localScale = new Vector3(0.2f, 0.2f, 0f); // scale sprite smaller

        // load pill sprite specific to this medicine
        pillSprite = Resources.Load<Sprite>("Sprites/Meds/" + medName + "_tab");
        if (pillSprite == null)
            pillSprite = Resources.Load<Sprite>("Sprites/Meds/null_tab"); // sprite not found in the folder
        spawnPill();
        GameObject.FindGameObjectWithTag("Pill").GetComponent<Pill>().Init(this.medName, this.defaultDosage, pillSprite, canSplit, transform.position);
        spawnPills = true;
        gameManager.CurrentMiniGameState = MiniGameState.Start;
    }

    void Update()
    {
        if (spawnPills)
            spawnPill();
        if (spawnPills && Input.GetKeyDown("space")) // reset pill while it's flying with space
            destroyPill();
    }

    void OnMouseDown()
    {
        if (spawnPills)
        {
            destroyPill(); // reset pill while it's flying by tapping the container (for mobile devices)
        }
    }

    void spawnPill()
    {
        // spawn new pill if one doesn't exist already
        GameObject existingPill = GameObject.FindGameObjectWithTag("Pill");
        if (existingPill == null)
        {
            GameObject pillObj = Instantiate(pillPrefab);
            pillObj.transform.SetParent(minigameObj.transform);
            pillObj.transform.position = gameObject.transform.position;
            pillObj.GetComponent<Pill>().Init(this.medName, this.defaultDosage, pillSprite, canSplit, transform.position);
            gameManager.CurrentMiniGameState = MiniGameState.Start;
        }
    }

    void destroyPill()
    {
        GameObject pill = GameObject.FindGameObjectWithTag("Pill");
        if (pill != null)
        {
            Destroy(pill);
        }
    }
}
