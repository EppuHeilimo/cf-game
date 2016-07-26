using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Mascot : MonoBehaviour {

    // the mascot is made with state machine
    public enum MascotState
    {
        STATE_NORMAL = 0,
        STATE_ANGRY,
        STATE_SLEEP
    }
    public MascotState currentState;

    public Sprite[] sprites = new Sprite[3];   
    public bool isTalking;
    public Animator anim;
    float delay = 3f;
    Tutorial tutorial;

    void Start()
    {
        currentState = MascotState.STATE_NORMAL;
        gameObject.GetComponent<Image>().sprite = sprites[0];
        tutorial = GameObject.Find("Tutorial").GetComponent<Tutorial>();
    }

    void Update()
    {
        // play mascot animation specific to the current state
        switch (currentState)
        {
            case MascotState.STATE_NORMAL:
                if (isTalking)
                    anim.Play("MascotTalk");
                else
                {
                    delay -= Time.deltaTime;
                    if (delay <= 0)
                    {
                        anim.Play("MascotPulse");
                        delay = 3f;
                        anim.Play("MascotPulse", -1, 0f);
                    }
                }
                break;

            case MascotState.STATE_ANGRY:
                if (isTalking)
                    anim.Play("MascotTalkAngry");
                else
                {
                    delay -= Time.deltaTime * 2;
                    if (delay <= 0)
                    {
                        anim.Play("MascotPulseAngry");
                        delay = 3f;
                        anim.Play("MascotPulseAngry", -1, 0f);
                    }
                }
                break;

            case MascotState.STATE_SLEEP:
                if (isTalking)
                { 
                    if (tutorial.currentState == Tutorial.TutorialState.STATE_ENDING_BAD_1)
                        ChangeState(MascotState.STATE_ANGRY);
                    else
                        ChangeState(MascotState.STATE_NORMAL);
                }
                else
                {
                    anim.Play("MascotSleep");
                }
                break;
        }     
    }

    public void ChangeState(MascotState newState)
    {
        currentState = newState;
        switch (newState)
        {
            case MascotState.STATE_NORMAL:
                gameObject.GetComponent<Image>().sprite = sprites[0];
                break;

            case MascotState.STATE_ANGRY:
                gameObject.GetComponent<Image>().sprite = sprites[1];
                break;

            case MascotState.STATE_SLEEP:
                gameObject.GetComponent<Image>().sprite = sprites[2];
                break;
        }
    }
}
