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

    void Start()
    {
        minigameObj = GameObject.Find("Minigame1");
    }

    public void Init(string medName, int defaultDosage)
    {
        this.medName = medName;
        this.defaultDosage = defaultDosage;
        Sprite medSprite = Resources.Load<Sprite>("Sprites/Minigame1/" + medName);
        if (medSprite == null)
            medSprite = Resources.Load<Sprite>("Sprites/Minigame1/null");
        gameObject.GetComponent<SpriteRenderer>().sprite = medSprite;
        GameObject.FindGameObjectWithTag("Pill").GetComponent<Pill>().Init(this.medName, this.defaultDosage);
    }

    /*
    void Update()
    {
        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || (Input.GetMouseButtonDown(0)))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.transform.tag == "BigMedCont")
            {
                GameObject pillObj = (GameObject)Instantiate(pillPrefab, hit.transform.position, Quaternion.identity);
                pillObj.transform.SetParent(minigameObj.transform);
                pillObj.GetComponent<Pill>().Init(this.medName, this.defaultDosage);
            }
        }
    }
    */

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
            pillObj.GetComponent<Pill>().Init(this.medName, this.defaultDosage);
        }
    }
}
