using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class KasiDesi : MonoBehaviour {

    Image image;
    float timeLeft;
    Color targetColor;
    bool blink;
    GameObject kasDesTxtObj;
    Text kasDesTxt;

    void Start()
    {
        image = GetComponent<Image>();
        kasDesTxtObj = GameObject.FindGameObjectWithTag("KasDesTxt");
        kasDesTxt = kasDesTxtObj.GetComponent<Text>();
    }

    void Update()
    {
        if (blink)
        {
            image.color = new Color(255, 0, 0, (Mathf.Sin(Time.time * 2.0f) + 1.0f) / 2.0f);
        }
    }

    public void StartBlinking()
    {
        kasDesTxt.text = "Click it!";
        blink = true;
    }

    public void StopBlinking()
    {
        kasDesTxt.text = "";
        blink = false;
    }

    public void SetDefaultColor()
    {
        image.color = new Color(255, 255, 255, 1);
    }
}
