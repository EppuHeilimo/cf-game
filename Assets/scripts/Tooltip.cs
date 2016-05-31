using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tooltip : MonoBehaviour {
    Item item;
    string data;
    GameObject tooltip;

    void Start()
    {
        tooltip = GameObject.Find("Tooltip");
        tooltip.SetActive(false);
    }

    void Update()
    {
        if (tooltip.activeSelf)
        {
            tooltip.transform.position = Input.mousePosition + new Vector3(0, 40, 0);
        }
    }

    public void Activate(Item item)
    {
        this.item = item;
        ConstructDataString();
        tooltip.SetActive(true);
    }

    public void Deactivate()
    {
        tooltip.SetActive(false);
    }

    public void ConstructDataString()
    {
        data = "<color=#0473f0><b>" + item.Title + "</b></color>\n\n" + item.Desc;
        tooltip.transform.GetChild(0).GetComponent<Text>().text = data;
    }
}
