using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

    public GameObject daychangeCanvasPrefab;
    public GameObject tutorialCompletePrefab;
    private GameObject daychangeCanvas; 
    int day = 1;
    ClockTime clock;
    CanvasGroup daychangecanvasgroup;
    float currentCanvasAlpha = 0.0f;
    float blendspeed = 1.0f;

    bool changingday = false;
    bool resuminggame = false;
    bool tutorialCompleted = false;
    bool blendcomplete = false;
    bool waitperiod = false;
    float timer = 0;
    Tutorial tutorial;
    // Use this for initialization
    void Start () {

#if (UNITY_ANDROID || UNITY_IPHONE)
        Application.targetFrameRate = 30;
#endif

        clock = GameObject.FindGameObjectWithTag("Clock").GetComponent<ClockTime>();
        tutorial = GameObject.Find("Tutorial").GetComponent<Tutorial>();
	}
	
	// Update is called once per frame
	void Update () {
        
        if(tutorialCompleted)
        {
            if(tutorialCompleted && !changingday && !waitperiod && !resuminggame)
            {
                GameObject.FindGameObjectWithTag("TextBoxManager").GetComponent<TextBoxManager>().DisableTextBox();
                daychangeCanvas = Instantiate(tutorialCompletePrefab);
                daychangecanvasgroup = daychangeCanvas.GetComponent<CanvasGroup>();
                daychangecanvasgroup.alpha = 0;
                changingday = true;
            }
            if(changingday)
            {
                if (daychangecanvasgroup.alpha < 1.0f)
                {
                    daychangecanvasgroup.alpha += Time.deltaTime * blendspeed;
                }
                else
                {
                    changingday = false;
                    clock.startDayOneAfterTutorial();
                    waitperiod = true;
                }
            }

            if(waitperiod)
            {
                timer += Time.deltaTime;
                if(timer > 3.0f)
                {
                    resuminggame = true;
                    waitperiod = false;
                    startDayOneAfterTutorial();
                }
            }

            if (resuminggame)
            {
                if (daychangecanvasgroup.alpha > 0.1f)
                {
                    daychangecanvasgroup.alpha -= Time.deltaTime * blendspeed;
                }
                else
                {
                    Destroy(daychangeCanvas);
                    resuminggame = false;
                    tutorialCompleted = false;
                }
            }
        }
        else if(!tutorial.tutorialOn)
        {
            if ((clock.isWorkShiftOver() && !changingday && !resuminggame))
            {
                GameObject.FindGameObjectWithTag("TextBoxManager").GetComponent<TextBoxManager>().DisableTextBox();
                GameObject.FindGameObjectWithTag("ScoringSystem").GetComponent<ScoringSystem>().endDay();
                daychangeCanvas = Instantiate(daychangeCanvasPrefab);
                daychangecanvasgroup = daychangeCanvas.GetComponent<CanvasGroup>();
                daychangecanvasgroup.alpha = 0;
                changingday = true;
            }

            if (changingday)
            {
                if (daychangecanvasgroup.alpha < 1.0f)
                {
                    daychangecanvasgroup.alpha += Time.deltaTime * blendspeed;
                }
                else
                {
                    changingday = false;
                    clock.changeDay();
                }
            }

            if (resuminggame)
            {
                if (daychangecanvasgroup.alpha > 0.1f)
                {
                    daychangecanvasgroup.alpha -= Time.deltaTime * blendspeed;
                }
                else
                {
                    Destroy(daychangeCanvas);
                    resuminggame = false;
                }
            }
        }
	    
	}

    public void endTutorial()
    {
        tutorialCompleted = true;
    }

    public void startDayOneAfterTutorial()
    {
        
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>().enabled = true;
        clock.resumeAfterTutorial();
        GameObject.FindGameObjectWithTag("ScoringSystem").GetComponent<ScoringSystem>().reset();
        tutorial.QuitTutorial();
        GameObject.FindGameObjectWithTag("TextBoxManager").GetComponent<TextBoxManager>().DisableTextBox();
    }

    public void continueToNextDay()
    {
        GameObject.FindGameObjectWithTag("ScoringSystem").GetComponent<ScoringSystem>().nextDay();
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>().enabled = true;
        clock.resumeAfterDayChange();
        resuminggame = true;

    }
}
