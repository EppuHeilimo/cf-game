using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClockTime : MonoBehaviour {
    float currentTime = 0.0f;
    int currentHours;
    //conversion ratio from real second to game time minute
    //change this to change game time speed
    float secToGameMin = 0.1f;

    //start day
    int day = 1;

    
    float dayLengthInRealHours = 24;

    float dayLength;
    //day start hour (0-24)
    int startHour = 0; 
    Text textref;
    string currentText;

    /* MEDICINE TIME CHECKS */
    const string MORNING_CHECK = "08:00";
    const string AFTERNOON_CHECK = "14:00";
    const string EVENING_CHECK = "16:00";
    const string NIGHT_CHECK = "21:00";

    /* DAYTIME CHANGE TIMES */
    const string MORNING_CHANGE = "07:00";
    const string AFTERNOON_CHANGE = "13:00";
    const string EVENING_CHANGE = "15:00";
    const string NIGHT_CHANGE = "20:00";

    public enum DayTime
    {
        MORNING,
        AFTERNOON,
        EVENING,
        NIGHT
    }

    public DayTime currentDayTime;

    NPCManagerV2 NPCManager;

    // Use this for initialization
    void Start () {
        dayLength = dayLengthInRealHours * 60 * secToGameMin;
        textref = GetComponent<Text>();
        currentText = textref.text;
        GameObject NPCManagerObj = GameObject.Find("NPCManager");
        NPCManager = NPCManagerObj.GetComponent<NPCManagerV2>();
    }
	
	// Update is called once per frame
	void Update () {
        currentTime += Time.deltaTime;
        if (currentTime >= dayLength)
        {
            currentTime = 0;
            currentHours = 0;
            day++;
        }
        string timeString = getTimeString();
        currentText = timeString + " Day " + day + "\n" + currentDayTime.ToString();

        // Medicine checks four times a day
        if (timeString == MORNING_CHECK)
            doMorningCheck();
        else if (timeString == AFTERNOON_CHECK)
            doAfternoonCheck();
        else if (timeString == EVENING_CHECK)
            doEveningCheck();
        else if (timeString == NIGHT_CHECK)
            doNightCheck();

        // When daytime changes, reset all NPCs meds
        if (timeString == MORNING_CHANGE)
            resetMeds();
        else if (timeString == AFTERNOON_CHANGE)
            resetMeds();
        else if (timeString == EVENING_CHANGE)
            resetMeds();
        else if (timeString == NIGHT_CHANGE)
            resetMeds();

        textref.text = currentText;

        /* Change daytime */
        /*******************
            07:00 - 12:59 aamu 
            13:00 - 14:59 päivä
            15:00 - 19:59 ilta
            20:00 - 06:59 yö
         *******************/
        if (currentHours > 6 && currentHours < 13)
            currentDayTime = DayTime.MORNING;
        else if (currentHours > 12 && currentHours < 15)
            currentDayTime = DayTime.AFTERNOON;
        else if (currentHours > 14 && currentHours < 20)
            currentDayTime = DayTime.EVENING;
        else
            currentDayTime = DayTime.NIGHT;
    }

    string getTimeString()
    {
        float minutesfloat = currentTime / secToGameMin;
        int minutesfloored = Mathf.FloorToInt(minutesfloat);
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
        currentHours = hours;
        return ret;
    }

    void doMorningCheck()
    {
        foreach (GameObject npcObj in NPCManager.npcList)
        {
            if (npcObj.GetComponent<NPCV2>().morningMed.title != null)
            {
                if (!npcObj.GetComponent<NPCV2>().morningMed.isActive)
                    npcObj.GetComponent<NPCV2>().isLosingHp = true;
            }            
        }
        print("MORNING CHECK!");
    }

    void doAfternoonCheck()
    {
        foreach (GameObject npcObj in NPCManager.npcList)
        {
            if (npcObj.GetComponent<NPCV2>().afternoonMed.title != null)
            {
                if (!npcObj.GetComponent<NPCV2>().afternoonMed.isActive)
                    npcObj.GetComponent<NPCV2>().isLosingHp = true;
            }
        }
        print("AFTERNOON CHECK!");
    }

    void doEveningCheck()
    {
        foreach (GameObject npcObj in NPCManager.npcList)
        {
            if (npcObj.GetComponent<NPCV2>().eveningMed.title != null)
            {
                if (!npcObj.GetComponent<NPCV2>().eveningMed.isActive)
                    npcObj.GetComponent<NPCV2>().isLosingHp = true;
            }
        }
        print("EVENING CHECK!");
    }

    void doNightCheck()
    {
        foreach (GameObject npcObj in NPCManager.npcList)
        {
            if (npcObj.GetComponent<NPCV2>().nightMed.title != null)
            {
                if (!npcObj.GetComponent<NPCV2>().nightMed.isActive)
                    npcObj.GetComponent<NPCV2>().isLosingHp = true;
            }
        }
        print("NIGHT CHECK!");
    }

    public void resetMeds()
    {
        foreach (GameObject npcObj in NPCManager.npcList)
        {
            npcObj.GetComponent<NPCV2>().disableAllMeds();
            npcObj.GetComponent<NPCV2>().isLosingHp = false;
        }
    }
}
