using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tooltip : MonoBehaviour {
    ItemContainer item;
    string data;
    GameObject tooltip;

    void Start()
    {
        tooltip = GameObject.Find("Tooltip");
        tooltip.SetActive(false);
    }

    void Update()
    {
        // display tooltip at mouse/touch position when active
        if (tooltip.activeSelf)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                for (var i = 0; i < Input.touchCount; ++i)
                {
                    Touch touch = Input.GetTouch(i);
                    if (touch.phase == TouchPhase.Began)
                    {
                        tooltip.transform.position = touch.position;
                    }
                }
            }
            else
                tooltip.transform.position = Input.mousePosition;
        }
    }

    public void Activate(ItemContainer item)
    {
        GameObject.Find("Canvas").GetComponent<Canvas>().sortingOrder = 2000;
        this.item = item;
        ConstructDataString();
        tooltip.SetActive(true);
    }

    public void Deactivate()
    {
        GameObject.Find("Canvas").GetComponent<Canvas>().sortingOrder = 0;
        tooltip.SetActive(false);
    }

    public void ConstructDataString()
    {
        data = "";
        foreach(Item it in item.medicine)
        {
            data += "<color=#0473f0><b>" + it.Title + "</b></color>\n" + it.Desc + "\n" + it.currentDosage + "\n";
        }
        tooltip.transform.GetChild(0).GetComponent<Text>().text = data;
    }
}
