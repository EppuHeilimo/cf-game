using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tutorial : MonoBehaviour {

    public enum TutorialState
    {
        STATE_START = 0,
        STATE_WALK_PRACTICE_FIRST,
        STATE_WALK_PRACTICE_SECOND,
        STATE_TARGET_PRACTICE_FIRST,
        STATE_TARGET_PRACTICE_SECOND,
        STATE_TARGET_PRACTICE_THIRD,
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
    public float letterPause = 0.1f;
    string message = "";

    GameObject indicator;
    public GameObject moveIndicatorPrefab;
    public GameObject walkHere;
    GameObject tutorialNPC;

    // Use this for initialization
    void Start () {     
        mascot = transform.GetChild(0).gameObject;
        currentState = TutorialState.STATE_INACTIVE;
        HideMascot();
        walkHere.SetActive(false);
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

                case TutorialState.STATE_WALK_PRACTICE_FIRST:
                    // check if player has walked to the destination...
                    break;

                case TutorialState.STATE_WALK_PRACTICE_SECOND:
                    // check if player has walked to the destination...
                    break;

                case TutorialState.STATE_TARGET_PRACTICE_FIRST:
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
                message = "Sup!\nWelcome to the tutorial!";
                StartCoroutine(ChangeState(TutorialState.STATE_WALK_PRACTICE_FIRST, 6f));
                break;

            case TutorialState.STATE_WALK_PRACTICE_FIRST:
                message = "Let's start by walking to the reception.";
                StartCoroutine(ShowPath());
                StartCoroutine(ChangeState(TutorialState.STATE_WALK_PRACTICE_SECOND, 12f));
                break;

            case TutorialState.STATE_WALK_PRACTICE_SECOND:
                message = "Just click on the ground to move and follow the red path.";
                break;

            case TutorialState.STATE_TARGET_PRACTICE_FIRST:
                if (indicator != null)
                    Destroy(indicator);
                walkHere.SetActive(false);
                message = "Great job!";
                StartCoroutine(ChangeState(TutorialState.STATE_TARGET_PRACTICE_SECOND, 3f));
                break;

            case TutorialState.STATE_TARGET_PRACTICE_SECOND:
                message = "Here comes a patient!\nPatients go first to the doctor's office to get diagnosed.";
                ShowNPC();
                StartCoroutine(ChangeState(TutorialState.STATE_TARGET_PRACTICE_THIRD, 12f));
                break;

            case TutorialState.STATE_TARGET_PRACTICE_THIRD:
                message = "The patient has been diagnosed with fever.\nPatients with red cross above their head are your responsibility.";
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
        if (indicator != null)
            Destroy(indicator);
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

    void ChangeState(TutorialState newState)
    {
        currentState = newState;
        stateChanged = true;
    }

    IEnumerator ShowPath()
    {
        yield return new WaitForSeconds(6f);
        mCamera.lockCameraToThisTransformForXTime(walkHere.transform, 5f);
        walkHere.SetActive(true);
        indicator = (GameObject)Instantiate(moveIndicatorPrefab, walkHere.transform.position, new Quaternion(0, 0, 0, 0));
        indicator.transform.localScale = new Vector3(5, 5, 5);
    }

    public void ReachedWalkZone()
    {
        ChangeState(TutorialState.STATE_TARGET_PRACTICE_FIRST);
    }

    void ShowNPC()
    {
        tutorialNPC = GameObject.FindGameObjectWithTag("NPC");
        mCamera.lockCameraToThisTransformForXTime(tutorialNPC.transform, 20f);
    }

}
