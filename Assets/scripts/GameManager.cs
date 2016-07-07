using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts;


public class GameManager : MonoBehaviour
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

    /// <summary>
    /// Moves the pill from the waiting position to the slingshot
    /// </summary>
    public void PillToSlingshot()
    {
        CurrentMiniGameState = MiniGameState.PillMovingToSlingshot;
        /*
        pill = GameObject.FindGameObjectWithTag("Pill");
        pill.transform.positionTo
            (Vector2.Distance(pill.transform.position / 10,
            slingshot.PillWaitPosition.transform.position) / 10, //duration
            slingshot.PillWaitPosition.transform.position). //final position
                setOnCompleteHandler((x) =>
                {
                    x.complete();
                    x.destroy(); //destroy the animation
                    CurrentMiniGameState = MiniGameState.Playing;
                    slingshot.enabled = true; //enable slingshot
                    slingshot.PillToThrow = pill;
                    slingshot.slingshotState = SlingshotState.Idle;
                });
        */
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