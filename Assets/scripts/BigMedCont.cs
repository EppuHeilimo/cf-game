using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class BigMedCont : MonoBehaviour
{
    public string medName;
    public int defaultDosage;
    public GameObject pillPrefab;
    GameObject minigameObj;
    Sprite pillSprite;

    void Start()
    {
        minigameObj = GameObject.Find("Minigame1");
    }

    public void Init(string medName, int defaultDosage)
    {
        this.medName = medName;
        this.defaultDosage = defaultDosage;
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
        GameObject.FindGameObjectWithTag("Pill").GetComponent<Pill>().Init(this.medName, this.defaultDosage, pillSprite);
    }

    void Update()
    {
        spawnPill();
    }

    void spawnPill()
    {
        // spawn new pill if one doesn't exist already
        GameObject existingPill = GameObject.FindGameObjectWithTag("Pill");
        if (existingPill == null)
        {
            GameObject pillObj = Instantiate(pillPrefab);
            pillObj.transform.SetParent(minigameObj.transform);
            pillObj.transform.position = gameObject.transform.position + new Vector3(-1, -1, 0);
            pillObj.GetComponent<SpringJoint2D>().connectedBody = gameObject.GetComponent<Rigidbody2D>();
            pillObj.GetComponent<Pill>().Init(this.medName, this.defaultDosage, pillSprite);
        }
    }
}
