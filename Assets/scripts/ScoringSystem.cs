using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class ScoringSystem : MonoBehaviour {

    int score = 50;
    bool scoreWasAltered = false;
    int prevscore = 50;
    GameObject negativebar;
	// Use this for initialization
	void Start () {
        negativebar = GameObject.FindGameObjectWithTag("ScoreBar").transform.FindChild("Negative").gameObject;
    }
	
	// Update is called once per frame
	void Update () {
        if(scoreWasAltered)
        {
            prevscore = score;
            scoreWasAltered = false;
            Vector2 size = negativebar.GetComponent<RectTransform>().sizeDelta;
            negativebar.GetComponent<RectTransform>().sizeDelta = new Vector2(100 - score, size.y);
        }
        else
        {
            if(prevscore != score)
            {
                prevscore = score;
                scoreWasAltered = false;
                score = 50;
                Vector2 size = negativebar.GetComponent<RectTransform>().sizeDelta;
                size = new Vector2(100 - score, size.y);
            }
        }
	}

    public void addToScore(int add)
    {
        if(add < 0)
        {
            if(score > 0)
                score += add;
            if (score < 0)
                score = 0;
        }
        else if (add > 0)
        {
            if (score < 100)
                score += add;
            if (score > 100)
                score = 100;
        }
        scoreWasAltered = true;
    }

}
