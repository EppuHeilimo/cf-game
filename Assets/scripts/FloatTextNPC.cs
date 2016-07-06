using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum FloatText
{
    Succesfull,
    IncorrectTime,
    IncorrectMedicine,
    Plus2,
    Plus3,
    Plus5,
    Plus10,
    Minus2,
    Minus3,
    Minus5,
    Minus10,
    Minus20
}

public class FloatTextNPC : MonoBehaviour {



    public GameObject FloatTextCanvas;
    public List<FloatText> floatingTexts = new List<FloatText>();
    public List<string> floatingStrings = new List<string>();


    void Update()
    {
        if(floatingTexts.Count > 0 || floatingStrings.Count > 0)
        {
           
            Vector3 pos = new Vector3(transform.position.x, transform.position.y + 100, transform.position.z);
            Quaternion rot = Quaternion.Euler(new Vector3(45, 45, 0));
            GameObject canvas = Instantiate(FloatTextCanvas, pos, rot) as GameObject;
            if (floatingTexts.Count > 0)
            {
                canvas.GetComponent<FloatTextCanvas>().Init(floatingTexts);
                floatingTexts.Clear();
            }
            if (floatingStrings.Count > 0)
            {
                canvas.GetComponent<FloatTextCanvas>().Init(floatingStrings);
                floatingStrings.Clear();
            }
        }

    }

    public void addFloatText(FloatText text)
    {
        if(!floatingTexts.Contains(text))
            floatingTexts.Add(text);
    }

    public void addFloatText(string text)
    {
        if (!floatingStrings.Contains(text))
            floatingStrings.Add(text);
    }

    public void addFloatText(List<FloatText> txts)
    {
        foreach(FloatText txt in txts)
        {
            floatingTexts.Add(txt);
        }
        
    }



}

//Quaternion.Euler( new Vector3 (parentEuler.x - fixedRotation.x, parentEuler.y - fixedRotation.y, parentEuler.z - fixedRotation.z));