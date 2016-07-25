using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class FloatTextNPC : MonoBehaviour {



    public GameObject FloatTextCanvas;
    public Dictionary<string, bool> floatingStrings = new Dictionary<string, bool>();


    void Update()
    {
        if( floatingStrings.Count > 0)
        {
           
            Vector3 pos = new Vector3(transform.position.x, transform.position.y + 100, transform.position.z);
            Quaternion rot = Quaternion.Euler(new Vector3(45, 45, 0));
            GameObject canvas = Instantiate(FloatTextCanvas, pos, rot) as GameObject;

            if (floatingStrings.Count > 0)
            {
                canvas.GetComponent<FloatTextCanvas>().Init(floatingStrings);
                floatingStrings.Clear();
            }
        }

    }



    public void addFloatText(string text, bool positive)
    {
        if (!floatingStrings.ContainsKey(text))
            floatingStrings.Add(text, positive);
    }

}

