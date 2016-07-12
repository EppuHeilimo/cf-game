using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class ScoringSystem : MonoBehaviour {

    public int score = 50;
    public int totalscore = 0;
    public int oldtotalscore = 0;
    int medinactivepunishment = -2;
    int respnpcdeathpunishment = -5;
    public bool gameover = false;

    GameObject positivebar;
	// Use this for initialization
	void Start () {
        positivebar = GameObject.FindGameObjectWithTag("ScoreBar").transform.FindChild("Positive").gameObject;
    }

    public void addToScore(int add)
    {
        if(add < 0)
        {
            if(score > 0)
                score += add;
            if (score <= 0)
            {
                score = 0;
                gameover = true;
            }
                
        }
        else if (add > 0)
        {
            if (score < 100)
                score += add;
            if (score > 100)
                score = 100;
        }
        positivebar.GetComponent<RectTransform>().sizeDelta = new Vector2(score * 2, 25.0f);

    }

    public void medInactive()
    {
        addToScore(medinactivepunishment);
    }

    public void responsibilityNPCDied()
    {
        addToScore(respnpcdeathpunishment);
    }

    public void endDay()
    {
        totalscore += score * 10;
    }

    public void nextDay()
    {
        oldtotalscore = totalscore;
    }


}
