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


    void Update()
    {
        if(floatingTexts.Count > 0)
        {
            Vector3 pos = new Vector3(transform.position.x, transform.position.y + 100, transform.position.z);
            Quaternion rot = Quaternion.Euler(new Vector3(45, 45, 0));
            GameObject canvas = Instantiate(FloatTextCanvas, pos, rot) as GameObject;
            canvas.GetComponent<FloatTextCanvas>().Init(floatingTexts);
            floatingTexts.Clear();
        }
    }

    public void addFloatText(FloatText text)
    {
        if(!floatingTexts.Contains(text))
            floatingTexts.Add(text);
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