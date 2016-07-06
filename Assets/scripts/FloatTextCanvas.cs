using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class FloatTextCanvas : MonoBehaviour {

    List<GameObject> floatingimages = new List<GameObject>();
    float defaulty = 0;
    float floatingHeight = -20.0f; // to this local y position
    float spacing = 10.0f;
    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {

        for (int i = 0; i < floatingimages.Count; i++)
        {
            
            floatingimages[i].transform.localPosition = new Vector3(floatingimages[i].transform.localPosition.x,
                floatingimages[i].transform.localPosition.y + Time.deltaTime * 20.0f,
                floatingimages[i].transform.localPosition.z);


            if (floatingimages[i].transform.localPosition.y > floatingHeight + (floatingimages.Count - 1) * spacing + 30.0f)
            {
                floatingimages[i].GetComponent<Image>().CrossFadeAlpha(0.0f, 0.2f, false);
                if (floatingimages[i].GetComponent<CanvasRenderer>().GetAlpha() < 0.05)
                {
                    GameObject temp = floatingimages[i];
                    floatingimages.RemoveAt(i);
                    Destroy(temp);
                } 

            }
        }
        if(floatingimages.Count == 0)
        {
            Destroy(gameObject);
        }

    }

    public void Init(List<FloatText> txts)
    {
        //Because C# loves references and we need to delete FloatTextNPC's version of floatingtexts after this.
        foreach (FloatText txt in txts)
        {
            floatImage(txt);
        }

    }

    public void floatImage(FloatText text)
    {
        GameObject go = new GameObject();
        go.AddComponent<Image>();
        float smallsize = 20.0f;
        float bigsize = 50.0f;
        switch (text)
        {
            case FloatText.Succesfull:
                go.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/UI/FloatingTexts/Success");
                go.GetComponent<RectTransform>().sizeDelta = new Vector2(bigsize, bigsize);
                break;
            case FloatText.IncorrectMedicine:
                go.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/UI/FloatingTexts/wrong");
                go.GetComponent<RectTransform>().sizeDelta = new Vector2(bigsize, bigsize);
                break;
            case FloatText.IncorrectTime:
                go.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/UI/FloatingTexts/wrongtime");
                go.GetComponent<RectTransform>().sizeDelta = new Vector2(bigsize, bigsize);
                break;
            case FloatText.Minus10:
                go.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/UI/FloatingTexts/-10");
                go.GetComponent<RectTransform>().sizeDelta = new Vector2(smallsize, smallsize);
                break;
            case FloatText.Minus2:
                go.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/UI/FloatingTexts/-2");
                go.GetComponent<RectTransform>().sizeDelta = new Vector2(smallsize, smallsize);
                break;
            case FloatText.Minus20:
                go.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/UI/FloatingTexts/-20");
                go.GetComponent<RectTransform>().sizeDelta = new Vector2(smallsize, smallsize);
                break;
            case FloatText.Minus3:
                go.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/UI/FloatingTexts/-3");
                go.GetComponent<RectTransform>().sizeDelta = new Vector2(smallsize, smallsize);
                break;
            case FloatText.Minus5:
                go.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/UI/FloatingTexts/-5");
                go.GetComponent<RectTransform>().sizeDelta = new Vector2(smallsize, smallsize);
                break;
            case FloatText.Plus10:
                go.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/UI/FloatingTexts/+10");
                go.GetComponent<RectTransform>().sizeDelta = new Vector2(smallsize, smallsize);
                break;
            case FloatText.Plus2:
                go.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/UI/FloatingTexts/+2");
                go.GetComponent<RectTransform>().sizeDelta = new Vector2(smallsize, smallsize);
                break;
            case FloatText.Plus3:
                go.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/UI/FloatingTexts/+3");
                go.GetComponent<RectTransform>().sizeDelta = new Vector2(smallsize, smallsize);
                break;
            case FloatText.Plus5:
                go.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/UI/FloatingTexts/+5");
                go.GetComponent<RectTransform>().sizeDelta = new Vector2(smallsize, smallsize);
                break;
        }
        go.GetComponent<Image>().preserveAspect = true;
        go.transform.SetParent(transform.FindChild("Panel"));

        float offset = 0;

        foreach(GameObject img in floatingimages)
        {
            offset += img.GetComponent<RectTransform>().sizeDelta.y;
        }
        if (go.GetComponent<RectTransform>().sizeDelta.y == bigsize)
        {
            offset += spacing;
        }

         go.transform.localPosition = new Vector3(0.0f, -50.0f + offset, 0.0f);

        print(go.transform.localPosition);

        go.transform.localRotation = Quaternion.identity;        
        floatingimages.Add(go);
        defaulty = go.transform.position.y;
    }
}
