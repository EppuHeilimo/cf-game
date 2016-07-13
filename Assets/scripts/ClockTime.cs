using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClockTime : MonoBehaviour {
    float currentTime = 0.0f;
    int currentHours;
    //conversion ratio from real second to game time minute
    //change this to change game time speed
    float secToGameMin = 0.5f;

    //start day
    public int day = 1;
    public bool paused = false;
    
    float dayLengthInRealHours = 24;

    float dayLength;
    //day start hour in minutes (60 * 1 to 24)
    int startHour = 6;
    float startHourInSeconds;
    Text textref;
    string currentText;

    /* MEDICINE TIME CHECKS */
    const string MORNING_CHECK = "08:00";
    const string AFTERNOON_CHECK = "14:00";
    const string EVENING_CHECK = "16:00";
    const string NIGHT_CHECK = "21:00";

    bool someoneislosinghp = false;
    float timer = 0;

    /* DAYTIME CHANGE TIMES */
    const string MORNING_CHANGE = "07:00";
    const string AFTERNOON_CHANGE = "13:00";
    const string EVENING_CHANGE = "15:00";
    const string NIGHT_CHANGE = "20:00";

    bool morningcheck = false;
    bool afternooncheck = false;
    bool eveningcheck = false;
    bool nightcheck = false;

    /*
     * Working shift:
     * 0 = Morning: 06:00 - 15:00
     * 1 = Afternoon: 10:00 - 19:00
     * 2 = Evening: 13:00 - 22:00
     * 3 = Night: 20:00 - 06:00
     */
    int shift = 0;

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
        startHourInSeconds = startHour * 60 * secToGameMin;
        textref = GetComponent<Text>();
        currentText = textref.text;
        GameObject NPCManagerObj = GameObject.Find("NPCManager");
        NPCManager = NPCManagerObj.GetComponent<NPCManagerV2>();
        shift = 0;
    }
	
	// Update is called once per frame
	void Update () {

        if(!paused)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= dayLength - startHourInSeconds)
            {
                currentTime = 0;
                currentHours = 0;
                startHourInSeconds = 0;
                startHour = 0;
            }
            string timeString = getTimeString();
            currentText = timeString; // + " Day " + day + "\n" + currentDayTime.ToString();
                                      // Medicine checks four times a day
            if (timeString == MORNING_CHECK && !morningcheck)
                doMorningCheck();
            else if (timeString == AFTERNOON_CHECK && !afternooncheck)
                doAfternoonCheck();
            else if (timeString == EVENING_CHECK && !eveningcheck)
                doEveningCheck();
            else if (timeString == NIGHT_CHECK && !nightcheck)
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

            if(someoneislosinghp)
            {
                timer += Time.deltaTime;
                isStillLosingHP();
            }
        }
    }

    public void isStillLosingHP()
    {
        bool stilllosinghp = false;
        foreach(GameObject go in NPCManager.responsibilityNpcs)
        {
            NPCV2 npc = go.GetComponent<NPCV2>();
            if(npc.myState != NPCV2.NPCState.STATE_DEAD || npc.myState != NPCV2.NPCState.STATE_LEAVE_HOSPITAL)
            {
                for(int i = 0; i < npc.morningMed.Length; i++)
                {
                    if(npc.morningMed[i].title != null)
                    {
                        if(!npc.morningMed[i].isActive)
                        {
                            stilllosinghp = true;
                        }
                    }
                }
            }
        }
    }

    public bool isWorkShiftOver()
    {
        if(shift == 0 && currentHours >= 22)
        {
            return true;
        }
        return false;
    }

    public void changeDay()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerControl>().resetPlayerPosition();
        player.GetComponent<PlayerControl>().enabled = false;
        GameObject.FindGameObjectWithTag("NPCManager").GetComponent<NPCManagerV2>().nextDay();
        currentTime = 0;
        currentHours = 0;
        startHourInSeconds = 0;
        startHour = 6;

        morningcheck = false;
        afternooncheck = false;
        eveningcheck = false;
        nightcheck = false;
        paused = true;
    }

    public void resumeAfterDayChange()
    {
        GameObject.FindGameObjectWithTag("NPCManager").GetComponent<NPCManagerV2>().nextDayResume();
        paused = false;
        day++;
        resetMeds();
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
        
        morningcheck = true;
        foreach (GameObject npcObj in NPCManager.npcList)
        {
            if (npcObj != null)
            { 
                NPCV2 npc = npcObj.GetComponent<NPCV2>();
                if(npc.diagnosed)
                {
                    for (int i = 0; i < npc.morningMed.Length; i++)
                    {
                        if (npc.morningMed[i].title != null)
                        {
                            if (!npc.playersResponsibility)
                            {
                                if (Random.Range(1, 100) < 8)
                                    npc.isLosingHp = true;
                                break;
                            }
                            else
                            {
                                if (!npc.morningMed[i].isActive)
                                {
                                    GameObject.FindGameObjectWithTag("ScoringSystem").GetComponent<ScoringSystem>().medInactive();
                                    npc.isLosingHp = true;
                                    someoneislosinghp = true;
                                    break;
                                }
                            }
                        }
                    }
                }                
            }
        }
    }

    void doAfternoonCheck()
    {
        afternooncheck = true;
        foreach (GameObject npcObj in NPCManager.npcList)
        {
            if (npcObj != null)
            {
                NPCV2 npc = npcObj.GetComponent<NPCV2>();
                if (npc.diagnosed)
                {
                    for (int i = 0; i < npc.afternoonMed.Length; i++)
                    {
                        if (npc.afternoonMed[i].title != null)
                        {
                            if (!npc.playersResponsibility)
                            {
                                if (Random.Range(1, 10) < 2)
                                    npc.isLosingHp = true;
                                break;
                            }
                            else
                            {
                                if (!npc.morningMed[i].isActive)
                                {
                                    npc.isLosingHp = true;
                                    someoneislosinghp = true;
                                    GameObject.FindGameObjectWithTag("ScoringSystem").GetComponent<ScoringSystem>().medInactive();
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    void doEveningCheck()
    {
        eveningcheck = true;
        foreach (GameObject npcObj in NPCManager.npcList)
        {
            if (npcObj != null)
            {
                NPCV2 npc = npcObj.GetComponent<NPCV2>();
                if (npc.diagnosed)
                {
                    for (int i = 0; i < npc.eveningMed.Length; i++)
                    {
                        if (npc.eveningMed[i].title != null)
                        {
                            if (!npc.playersResponsibility)
                            {
                                if (Random.Range(1, 10) < 2)
                                    npc.isLosingHp = true;
                                break;
                            }
                            else
                            {
                                if (!npc.morningMed[i].isActive)
                                {
                                    npc.isLosingHp = true;
                                    someoneislosinghp = true;
                                    GameObject.FindGameObjectWithTag("ScoringSystem").GetComponent<ScoringSystem>().medInactive();
                                    print(npc.myName);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    void doNightCheck()
    {
        nightcheck = true;
        foreach (GameObject npcObj in NPCManager.npcList)
        {
            if (npcObj != null)
            {
                NPCV2 npc = npcObj.GetComponent<NPCV2>();
                if (npc.diagnosed)
                {
                    for (int i = 0; i < npc.nightMed.Length; i++)
                    {
                        if (npc.nightMed[i].title != null)
                        {
                            if (!npc.playersResponsibility)
                            {
                                if (Random.Range(1, 10) < 2)
                                    npc.isLosingHp = true;
                                break;
                            }
                            else
                            {
                                if (!npc.morningMed[i].isActive)
                                {
                                    npc.isLosingHp = true;
                                    someoneislosinghp = true;
                                    GameObject.FindGameObjectWithTag("ScoringSystem").GetComponent<ScoringSystem>().medInactive();
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void resetMeds()
    {
        someoneislosinghp = false;
        foreach (GameObject npcObj in NPCManager.npcList)
        {
            if (npcObj != null)
            {
                npcObj.GetComponent<NPCV2>().disableAllMeds();
                npcObj.GetComponent<NPCV2>().isLosingHp = false;
            }
        }
    }
}
