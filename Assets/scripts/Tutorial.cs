using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {

    public enum TutorialState
    {
        STATE_START = 0,
        STATE_WALK_PRACTICE,
        STATE_TARGET_PRACTICE,
        STATE_INACTIVE
    }

    public bool tutorialOn;
    public ShowPanels showPanels;
    public StartOptions startOptions;
    public TutorialState currentState;

    GameObject mascot;

	// Use this for initialization
	void Start () {     
        mascot = transform.GetChild(0).gameObject;
        currentState = TutorialState.STATE_INACTIVE;
        HideMascot();
    }
	
	// Update is called once per frame
	void Update () {
        if (tutorialOn)
        {
            switch (currentState)
            {
                case TutorialState.STATE_START:
                    
                    break;

                case TutorialState.STATE_WALK_PRACTICE:
                    break;

                case TutorialState.STATE_TARGET_PRACTICE:
                    break;

                case TutorialState.STATE_INACTIVE:
                    // do nothing...
                    break;
            }
        }
	}

    public void TutorialYesClicked()
    {
        startOptions.StartButtonClicked();
        showPanels.HideTutorialPanel();
        tutorialOn = true;
        ShowMascot();
        currentState = TutorialState.STATE_START;
    }

    public void TutorialNoClicked()
    {
        startOptions.StartButtonClicked();
        showPanels.HideTutorialPanel();
        tutorialOn = false;
    }

    public void QuitTutorial()
    {
        HideMascot();
        tutorialOn = false;
        currentState = TutorialState.STATE_INACTIVE;
    }

    public void ShowMascot()
    {
        mascot.SetActive(true);
    }

    public void HideMascot()
    {
        mascot.SetActive(false);
    }

}
