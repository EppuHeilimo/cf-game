using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Mascot : MonoBehaviour {

    public enum MascotState
    {
        STATE_NORMAL = 0,
        STATE_ANGRY
    }

    public Sprite[] sprites = new Sprite[2];
    public MascotState currentState;
    public bool isTalking;
    public Animator anim;
    float delay = 3f;

    void Start()
    {
        currentState = MascotState.STATE_NORMAL;
        gameObject.GetComponent<Image>().sprite = sprites[0];
    }

    void Update()
    {
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
                    delay -= Time.deltaTime;
                    if (delay <= 0)
                    {
                        anim.Play("MascotPulseAngry");
                        delay = 3f;
                        anim.Play("MascotPulseAngry", -1, 0f);
                    }
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
        }
    }
}
