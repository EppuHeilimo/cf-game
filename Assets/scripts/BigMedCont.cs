using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;
using Assets.Scripts;

public class BigMedCont : MonoBehaviour
{
    public string medName;
    public int defaultDosage;
    public int canSplit;
    public GameObject pillPrefab;
    GameObject minigameObj;
    Sprite pillSprite;
    public bool spawnPills;
    GameManager gameManager;

    void Start()
    {
        minigameObj = GameObject.Find("Minigame1");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void Init(string medName, int defaultDosage, int canSplit)
    {
        this.medName = medName;
        this.defaultDosage = defaultDosage;
        this.canSplit = canSplit;
        Sprite medSprite = Resources.Load<Sprite>("Sprites/Meds/" + medName);
        if (medSprite == null)
            medSprite = Resources.Load<Sprite>("Sprites/Meds/null");
        SpriteRenderer sRenderer = gameObject.GetComponent<SpriteRenderer>();
        sRenderer.sprite = medSprite;
        transform.localScale = new Vector3(0.2f, 0.2f, 0f); //scale sprite smaller
        pillSprite = Resources.Load<Sprite>("Sprites/Meds/" + medName + "_tab");
        if (pillSprite == null)
            pillSprite = Resources.Load<Sprite>("Sprites/Meds/null_tab");
        spawnPill();
        GameObject.FindGameObjectWithTag("Pill").GetComponent<Pill>().Init(this.medName, this.defaultDosage, pillSprite, canSplit, transform.position);
        spawnPills = true;
        gameManager.CurrentMiniGameState = MiniGameState.Start;
    }

    void Update()
    {
        if (spawnPills)
            spawnPill();
        if (spawnPills && Input.GetKeyDown("space"))
            destroyPill();
    }

    void OnMouseDown()
    {
        if (spawnPills)
        {
            destroyPill();
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
