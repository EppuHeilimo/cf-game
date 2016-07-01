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
        gameObject.GetComponent<SpriteRenderer>().sprite = medSprite;
        /*
        var bounds = gameObject.GetComponent<SpriteRenderer>().sprite.bounds;
        Vector3 scale = new Vector3(5.0f / bounds.size.x, 5.0f / bounds.size.y, 0);
        transform.localScale = scale;
        */

        pillSprite = Resources.Load<Sprite>("Sprites/Meds/" + medName + "_pill");
        if (pillSprite == null)
            pillSprite = Resources.Load<Sprite>("Sprites/Meds/null_pill");
        GameObject.FindGameObjectWithTag("Pill").GetComponent<Pill>().Init(this.medName, this.defaultDosage, pillSprite);
    }

    void Update()
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
