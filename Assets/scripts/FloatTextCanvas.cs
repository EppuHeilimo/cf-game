using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class FloatTextCanvas : MonoBehaviour {
    public GameObject FloatTextGO;
    List<GameObject> floatingtexts = new List<GameObject>();
    float defaulty = 0;
    float floatingHeight = 10.0f; // to this local y position
    float spacing = 5.0f;
    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {

        for (int i = 0; i < floatingtexts.Count; i++)
        {
            
            floatingtexts[i].transform.localPosition = new Vector3(floatingtexts[i].transform.localPosition.x,
                floatingtexts[i].transform.localPosition.y + Time.deltaTime * 20.0f,
                floatingtexts[i].transform.localPosition.z);

            if (floatingtexts[i].transform.localPosition.y > floatingHeight + i * spacing)
            {
                floatingtexts[i].GetComponent<Text>().CrossFadeAlpha(0.0f, 0.2f, false);
                if (floatingtexts[i].GetComponent<CanvasRenderer>().GetAlpha() < 0.05)
                {
                    GameObject temp = floatingtexts[i];
                    floatingtexts.RemoveAt(i);
                    Destroy(temp);
                } 

            }
        }
        if(floatingtexts.Count == 0)
        {
            Destroy(gameObject);
        }

    }



    public void Init(Dictionary<string, bool> txts)
    {
        //Because C# loves references and we need to delete FloatTextNPC's version of floatingtexts after this.
        foreach (KeyValuePair<string,bool> txt in txts)
        {
            floatString(txt);
        }

    }

    public void floatString(KeyValuePair<string, bool> text)
    {
        GameObject go = Instantiate(FloatTextGO, Vector3.zero, Quaternion.identity) as GameObject;
        go.transform.SetParent(transform.FindChild("Panel"));
        if(text.Value)
            go.GetComponent<Text>().color = Color.green;
        else
            go.GetComponent<Text>().color = Color.red;
        go.GetComponent<Text>().text = text.Key;

        float offset = 0;
        foreach (GameObject img in floatingtexts)
        {
            offset += img.GetComponent<RectTransform>().sizeDelta.y / 2;
        }
        go.transform.localPosition = new Vector3(0.0f, -50.0f + offset, 0.0f);
        go.transform.localRotation = Quaternion.identity;
        floatingtexts.Add(go);
        defaulty = go.transform.position.y;
    }


}
