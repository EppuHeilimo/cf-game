using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class ScoringSystem : MonoBehaviour {



    GameObject FloatingTextCanvas;
    List<GameObject> floatingimages;
    public float score = 50.0f;
    float defaulty = 0;
	// Use this for initialization
	void Start () {
        FloatingTextCanvas = GameObject.FindGameObjectWithTag("FloatingText");
        floatingimages = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
	    for(int i = 0; i < floatingimages.Count; i++)
        {

            floatingimages[i].transform.localPosition = new Vector3(floatingimages[i].transform.localPosition.x,
                floatingimages[i].transform.localPosition.y + Time.deltaTime * 10.5f,
                floatingimages[i].transform.localPosition.z);

            if (floatingimages[i].transform.position.y > defaulty + 100.0f)
            {
                GameObject temp = floatingimages[i];
                floatingimages.RemoveAt(i);
                Destroy(temp);
            }
        }
	}

    public void floatImage(FloatText text)
    {
        switch (text)
        {
            case FloatText.Succesfull:
                GameObject go = new GameObject();

                go.AddComponent<Image>();
                go.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/FloatingSuccess");
                go.GetComponent<Image>().preserveAspect = true;
                go.transform.SetParent(FloatingTextCanvas.transform);
                go.transform.localPosition = Vector3.zero;
                floatingimages.Add(go);
                defaulty = go.transform.position.y;
                break;
        }
    }

    public void addToScore(float add)
    {
        score += add;
    }

    public void reduceFromScore(float sub)
    {
        score -= sub;
    }
}
