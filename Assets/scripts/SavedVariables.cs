using UnityEngine;
using System.Collections;

/* use this to save variables when scene is reloaded */
public class SavedVariables : MonoBehaviour {

    public float sliderValue;
    public bool toggleValue;
    public bool set;

    public void SaveValues(float f, bool b)
    {
        set = true;
        sliderValue = f;
        toggleValue = b;
    }
}
