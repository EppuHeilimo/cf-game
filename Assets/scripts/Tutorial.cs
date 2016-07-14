using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tutorial : MonoBehaviour {

    public enum TutorialState
    {
        STATE_START = 0,
        STATE_WALK_PRACTICE_1,
        STATE_WALK_PRACTICE_2,
        STATE_TARGET_PRACTICE_1,
        STATE_TARGET_PRACTICE_2,
        STATE_TARGET_PRACTICE_3,
        STATE_TARGET_PRACTICE_4,
        STATE_TARGET_PRACTICE_5,
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
    public float letterPause = 0.05f;
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
                case TutorialState.STATE_TARGET_PRACTICE_4:
                    GameObject tp = GameObject.FindGameObjectWithTag("TargetPanel");
                    if (tp != null)
                    {
                        ChangeState(TutorialState.STATE_TARGET_PRACTICE_5);
                    }
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
                StartCoroutine(ChangeState(TutorialState.STATE_WALK_PRACTICE_1, 5f));
                break;

            case TutorialState.STATE_WALK_PRACTICE_1:
                message = "Let's start by walking to the reception.";
                StartCoroutine(ShowPath());
                StartCoroutine(ChangeState(TutorialState.STATE_WALK_PRACTICE_2, 12f));
                break;

            case TutorialState.STATE_WALK_PRACTICE_2:
                message = "Just click on the ground to move and follow the red path.";
                break;

            case TutorialState.STATE_TARGET_PRACTICE_1:
                if (indicator != null)
                    Destroy(indicator);
                walkHere.SetActive(false);
                message = "Great job!";
                StartCoroutine(ChangeState(TutorialState.STATE_TARGET_PRACTICE_2, 3f));
                break;

            case TutorialState.STATE_TARGET_PRACTICE_2:
                message = "Here comes a patient!\nPatients go first to the doctor's office to get diagnosed.";
                ShowNPC();
                StartCoroutine(ChangeState(TutorialState.STATE_TARGET_PRACTICE_3, 16f));
                break;

            case TutorialState.STATE_TARGET_PRACTICE_3:
                message = "The patient has been diagnosed now.\nPatients with red cross above their head are your responsibility.";
                StartCoroutine(ChangeState(TutorialState.STATE_TARGET_PRACTICE_4, 16f));
                break;

            case TutorialState.STATE_TARGET_PRACTICE_4:
                message = "Walk to the patient and click him to see his information.";
                break;

            case TutorialState.STATE_TARGET_PRACTICE_5:
                message = "Good job!\nYou can see on the patient's medicine card that he needs some Ibuprofen.";
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
        ChangeState(TutorialState.STATE_TARGET_PRACTICE_1);
    }

    void ShowNPC()
    {
        tutorialNPC = GameObject.Find("NPCManager").GetComponent<NPCManagerV2>().spawnTutorialGuy();
        mCamera.lockCameraToThisTransformForXTime(tutorialNPC.transform, 30f);
    }

}
