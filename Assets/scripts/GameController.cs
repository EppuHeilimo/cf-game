using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

    public GameObject daychangeCanvasPrefab;
    private GameObject daychangeCanvas; 
    int day = 1;
    ClockTime clock;
    CanvasGroup daychangecanvasgroup;
    float currentCanvasAlpha = 0.0f;
    float blendspeed = 1.0f;

    bool changingday = false;
    bool resuminggame = false;
	// Use this for initialization
	void Start () {
        clock = GameObject.FindGameObjectWithTag("Clock").GetComponent<ClockTime>();
        	    
	}
	
	// Update is called once per frame
	void Update () {

	    if(clock.isWorkShiftOver() && !changingday)
        {
            GameObject.FindGameObjectWithTag("TextBoxManager").GetComponent<TextBoxManager>().DisableTextBox();
            daychangeCanvas = Instantiate(daychangeCanvasPrefab);
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
                clock.changeDay();
            }
        }
        if(resuminggame)
        {
            if(daychangecanvasgroup.alpha > 0.1f)
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

    public void continueToNextDay()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>().enabled = true;
        clock.resumeAfterDayChange();
        resuminggame = true;

    }
}
