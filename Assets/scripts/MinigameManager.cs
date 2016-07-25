using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts;

/* handles slingshot minigame's flow */
public class MinigameManager : MonoBehaviour
{
    public SlingShot slingshot;
    public MiniGameState CurrentMiniGameState;
    private GameObject pill;

    public void Start()
    {
        CurrentMiniGameState = MiniGameState.Inactive;
        slingshot.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        switch (CurrentMiniGameState)
        {
            case MiniGameState.Start:
                PillToSlingshot();
                break;
            case MiniGameState.PillMovingToSlingshot:
                break;
            case MiniGameState.Playing:
                break;
            case MiniGameState.Inactive:
                slingshot.slingshotState = SlingshotState.Inactive;
                break;
            default:
                break;
        }
    }

    /* moves the pill to the slingshot */
    public void PillToSlingshot()
    {
        CurrentMiniGameState = MiniGameState.PillMovingToSlingshot;
        pill = GameObject.FindGameObjectWithTag("Pill");
        if (pill == null)
            return;
        else
        {
            slingshot.enabled = true;
            slingshot.PillToThrow = pill;
            slingshot.slingshotState = SlingshotState.Idle;
            CurrentMiniGameState = MiniGameState.Playing;
        }
    }
}