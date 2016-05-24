using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClockTime : MonoBehaviour {
    float currentTime = 0.0f;
    //conversion ratio from real second to game time minute
    //change this to change game time speed
    float secToGameMin = 0.2f;

    //start day
    int day = 1;

    float dayLengthInRealHours = 8;

    float dayLength;
    //day start hour (0-24)
    int startHour = 8; 
    Text textref;
    string currentText;
	// Use this for initialization
	void Start () {
        dayLength = dayLengthInRealHours * 60 * secToGameMin;
        textref = GetComponent<Text>();
        currentText = textref.text;

    }
	
	// Update is called once per frame
	void Update () {
        currentTime += Time.deltaTime;
        if (currentTime >= dayLength)
        {
            currentTime = 0;
            day++;
        }

        currentText = getTimeString() + " Day " + day;
        
        textref.text = currentText;
	}

    string getTimeString()
    {
        float minutesfloat = currentTime / secToGameMin;
        int minutesfloored = Mathf.FloorToInt(minutesfloat);
        string hour;
        string minutes;
        string ret;

        int hours = 0;

        while(minutesfloored >= 60)
        {
            minutesfloored -= 60;
            hours++;
        }

        //adding the starthour to get relative time
        hours += startHour;

        if (hours < 10)
        {
            if(minutesfloored < 10 )
                ret = "0" + hours + ":" + "0" + minutesfloored;
            else
                ret = "0" + hours + ":" + minutesfloored;
        }
        else
        {
            if (minutesfloored < 10)
                ret = hours + ":" + "0" + minutesfloored;
            else
                ret = hours + ":" + minutesfloored;
        }

        return ret;
    }

}
