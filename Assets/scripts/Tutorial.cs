using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tutorial : MonoBehaviour {

    public enum TutorialState
    {
        STATE_START = 0,
        STATE_WALK_PRACTICE,
        STATE_TARGET_PRACTICE,
        STATE_INACTIVE
    }
    bool stateChanged;

    public bool tutorialOn;
    public ShowPanels showPanels;
    public StartOptions startOptions;
    public TutorialState currentState;
    public CameraMovement mCamera;

    GameObject mascot;
    public Text text;
    public float letterPause = 0.3f;
    string message = "";

    public Transform walkHere;

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
            if (stateChanged)
            {
                OnStateChange();
            }

            switch (currentState)
            {
                case TutorialState.STATE_START:
                    // do nothing...
                    break;

                case TutorialState.STATE_WALK_PRACTICE:
                    // check if player has walked to the destination...
                    break;

                case TutorialState.STATE_TARGET_PRACTICE:
                    // check if player has targeted correct npc...
                    break;

                case TutorialState.STATE_INACTIVE:
                    // do nothing...
                    break;
            }
        }
	}

    void OnStateChange()
    {
        text.text = "";
        switch (currentState)
        {
            case TutorialState.STATE_START:
                message = "Sup dude!\nLet's begin the tutorial...";
                StartCoroutine(ChangeState(TutorialState.STATE_WALK_PRACTICE, 6f));
                break;

            case TutorialState.STATE_WALK_PRACTICE:
                message = "Let's start by walking to the reception to meet your patients!";
                StartCoroutine(ShowPath());
                break;

            case TutorialState.STATE_TARGET_PRACTICE:
                break;

            case TutorialState.STATE_INACTIVE:
                message = "";
                break;
        }
        StartCoroutine(TypeText());
        stateChanged = false;
    }

    IEnumerator TypeText()
    {
        foreach (char letter in message.ToCharArray())
        {
            text.text += letter;
            yield return new WaitForSeconds(letterPause);
        }
    }

    public void TutorialYesClicked()
    {
        startOptions.StartButtonClicked();
        showPanels.HideTutorialPanel();
        tutorialOn = true;
        ShowMascot();
        StartCoroutine(ChangeState(TutorialState.STATE_START, 1f));
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

    IEnumerator ChangeState(TutorialState newState, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        currentState = newState;
        stateChanged = true;
    }

    IEnumerator ShowPath()
    {
        yield return new WaitForSeconds(8f);
        mCamera.lockCameraToThisTransformForXTime(walkHere, 5f);
    }

}
