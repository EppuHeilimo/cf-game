using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

    public GameObject daychangeCanvasPrefab;
    public GameObject tutorialCompletePrefab
        ;
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
    float timer = 0;
    // Use this for initialization
    void Start () {

#if (UNITY_ANDROID || UNITY_IPHONE)
        Application.targetFrameRate = 30;
#endif

        clock = GameObject.FindGameObjectWithTag("Clock").GetComponent<ClockTime>();
        	
	}
	
	// Update is called once per frame
	void Update () {
        
	    if((clock.isWorkShiftOver() && !changingday) || (tutorialCompleted && !changingday))
        {
            if(tutorialCompleted)
            {
                GameObject.FindGameObjectWithTag("TextBoxManager").GetComponent<TextBoxManager>().DisableTextBox();
                daychangeCanvas = Instantiate(tutorialCompletePrefab);
                daychangecanvasgroup = daychangeCanvas.GetComponent<CanvasGroup>();
                daychangecanvasgroup.alpha = 0;
                changingday = true;
            }
            else
            {
                GameObject.FindGameObjectWithTag("TextBoxManager").GetComponent<TextBoxManager>().DisableTextBox();
                GameObject.FindGameObjectWithTag("ScoringSystem").GetComponent<ScoringSystem>().endDay();
                daychangeCanvas = Instantiate(daychangeCanvasPrefab);
                daychangecanvasgroup = daychangeCanvas.GetComponent<CanvasGroup>();
                daychangecanvasgroup.alpha = 0;
                changingday = true;
            }

        }

        if(changingday)
        {
            if(tutorialCompleted)
            {
                if(!blendcomplete)
                {
                    if (daychangecanvasgroup.alpha < 1.0f)
                    {
                        daychangecanvasgroup.alpha += Time.deltaTime * blendspeed;
                    }
                    else
                    {
                        changingday = false;
                        clock.changeDay();
                        blendcomplete = true;
                    }
                }
                else
                {
                    timer += Time.deltaTime;
                    if(timer > 3.0f)
                    {
                        timer = 0;
                        blendcomplete = false;
                        startDayOneAfterTutorial();
                    }
                }

            }
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
        if(resuminggame)
        {
            if(tutorialCompleted)
            {
                tutorialCompleted = false;
            }

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

    public void endTutorial()
    {
        tutorialCompleted = true;
    }

    public void startDayOneAfterTutorial()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>().enabled = true;
        clock.resumeAfterDayChange();
        resuminggame = true;
        GameObject.Find("Tutorial").GetComponent<Tutorial>().tutorialOn = false;
    }

    public void continueToNextDay()
    {
        GameObject.FindGameObjectWithTag("ScoringSystem").GetComponent<ScoringSystem>().nextDay();
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>().enabled = true;
        clock.resumeAfterDayChange();
        resuminggame = true;

    }
}
