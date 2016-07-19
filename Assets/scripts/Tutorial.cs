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

        STATE_MINIGAME_PRACTICE_1,
        STATE_MINIGAME_PRACTICE_2,
        STATE_MINIGAME_PRACTICE_3,
        STATE_MINIGAME_PRACTICE_4,
        STATE_MINIGAME_PRACTICE_5,
        STATE_MINIGAME_PRACTICE_6,
        STATE_MINIGAME_PRACTICE_SPLITTING,

        STATE_ENDING_GOOD_1,
        STATE_ENDING_GOOD_2,
        STATE_ENDING_GOOD_3,
        STATE_ENDING_GOOD_4,
        STATE_ENDING_GOOD_FINAL,
        STATE_ENDING_BAD_1,
        STATE_ENDING_BAD_2,

        STATE_COMPUTER_PRACTICE,
        STATE_TRASH_PRACTICE,

        STATE_INACTIVE
    }
    bool stateChanged;

    public bool tutorialOn;
    public ShowPanels showPanels;
    public StartOptions startOptions;
    public TutorialState currentState;
    public CameraMovement mCamera;

    GameObject tutCanvas;
    Mascot mascot;
    public Text text;
    public float letterPause = 0.05f;
    string message = "";
    public bool typing;

    GameObject indicator;
    public GameObject moveIndicatorPrefab;
    public GameObject walkHere;
    GameObject tutorialNPC;
    GameObject medCab;
    Minigame1 minigame;
    BigMedCont medCont;
    MedCup morningCup;
    MedCup afternoonCup;
    MedCup eveningCup;
    MedCup nightCup;
    public GameObject scoreBarHighlight;
    GameObject computer;
    GameObject trashCan;
    float timeSpentInThisState;
    const float GO_TO_SLEEP_TIME = 20f;

    // Use this for initialization
    void Start () {     
        tutCanvas = transform.GetChild(0).gameObject;
        mascot = GameObject.Find("Mascot").GetComponent<Mascot>();
        currentState = TutorialState.STATE_INACTIVE;
        HidetutCanvas();
        walkHere.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        if (tutorialOn)
        {
            timeSpentInThisState += Time.deltaTime;
            if (timeSpentInThisState >= GO_TO_SLEEP_TIME)
            {
                mascot.ChangeState(Mascot.MascotState.STATE_SLEEP);
            }

            if (stateChanged)
            {
                OnStateChange();
            }

            if (mascot != null)
                mascot.isTalking = typing;

            switch (currentState)
            {
                case TutorialState.STATE_TARGET_PRACTICE_4:
                    // check if player has targeted the patient
                    GameObject tp = GameObject.FindGameObjectWithTag("TargetPanel");
                    if (tp != null)
                    {
                        ChangeState(TutorialState.STATE_TARGET_PRACTICE_5);
                    }
                    break;

                case TutorialState.STATE_MINIGAME_PRACTICE_2:
                    // check if player has opened the medicine cabinet
                    if (minigame.active)
                    {
                        ChangeState(TutorialState.STATE_MINIGAME_PRACTICE_3);
                    }
                    break;

                case TutorialState.STATE_MINIGAME_PRACTICE_3:
                    // check if Ibuprofen has been selected
                    if (medCont.medName == "Ibuprofen")
                    {
                        ChangeState(TutorialState.STATE_MINIGAME_PRACTICE_4);
                    }
                    break;

                case TutorialState.STATE_MINIGAME_PRACTICE_4:
                    // check if pill inside any med cup
                    if (morningCup.medsInThisCup.Count > 0 || afternoonCup.medsInThisCup.Count > 0 || eveningCup.medsInThisCup.Count > 0 || nightCup.medsInThisCup.Count > 0)
                    {
                        ChangeState(TutorialState.STATE_MINIGAME_PRACTICE_SPLITTING);
                    }
                    break;

                case TutorialState.STATE_MINIGAME_PRACTICE_5:
                    // exited minigame
                    if (!minigame.active)
                    {
                        ChangeState(TutorialState.STATE_MINIGAME_PRACTICE_6);
                    }
                    break;

                case TutorialState.STATE_MINIGAME_PRACTICE_6:
                    // check if medicine given to the patient
                    if (tutorialNPC.GetComponent<NPCV2>().morningMed[0].isActive)
                    {
                        ChangeState(TutorialState.STATE_ENDING_GOOD_1);
                    }
                    else if (tutorialNPC.GetComponent<NPCV2>().myHp < 50)
                    {
                        ChangeState(TutorialState.STATE_ENDING_BAD_1);
                    }
                    break;

                case TutorialState.STATE_ENDING_BAD_1:
                    // check if medicine given to the patient
                    if (tutorialNPC.GetComponent<NPCV2>().morningMed[0].isActive)
                    {
                        ChangeState(TutorialState.STATE_ENDING_GOOD_1);
                    }
                    // check if patient dies
                    else if (tutorialNPC.GetComponent<NPCV2>().myHp <= 0)
                    {
                        ChangeState(TutorialState.STATE_ENDING_BAD_2);
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
        timeSpentInThisState = 0f;
        StopAllCoroutines();
        text.text = "";
        switch (currentState)
        {
            case TutorialState.STATE_START:
                message = "Sup!\nWelcome to the tutorial!";
                StartCoroutine(ChangeState(TutorialState.STATE_WALK_PRACTICE_1, 2f));
                break;

            case TutorialState.STATE_WALK_PRACTICE_1:
                message = "Lets start by walking to the reception.";
                StartCoroutine(ShowPath());
                StartCoroutine(ChangeState(TutorialState.STATE_WALK_PRACTICE_2, 6f));
                break;

            case TutorialState.STATE_WALK_PRACTICE_2:
                message = "Just click on the ground to move.";
                break;

            case TutorialState.STATE_TARGET_PRACTICE_1:
                if (indicator != null)
                    Destroy(indicator);
                walkHere.SetActive(false);
                message = "Great job!";
                StartCoroutine(ChangeState(TutorialState.STATE_TARGET_PRACTICE_2, 1f));
                break;

            case TutorialState.STATE_TARGET_PRACTICE_2:
                message = "Here comes a patient!\nPatients go first to the doctor's office to get diagnosed.";
                ShowNPC();
                StartCoroutine(ChangeState(TutorialState.STATE_TARGET_PRACTICE_3, 14f));
                break;

            case TutorialState.STATE_TARGET_PRACTICE_3:
                message = "Patients with red cross above their head are your responsibility.";
                StartCoroutine(ChangeState(TutorialState.STATE_TARGET_PRACTICE_4, 6f));
                break;

            case TutorialState.STATE_TARGET_PRACTICE_4:
                message = "Walk to the patient and click him to see his information.";
                break;

            case TutorialState.STATE_TARGET_PRACTICE_5:
                message = "Good job!\n";
                StartCoroutine(ChangeState(TutorialState.STATE_MINIGAME_PRACTICE_1, 1f));
                break;

            case TutorialState.STATE_MINIGAME_PRACTICE_1:
                message = "You can see on the patients medicine card that he needs Ibuprofen.\nLets go get some!";
                StartCoroutine(ChangeState(TutorialState.STATE_MINIGAME_PRACTICE_2, 6f));
                break;

            case TutorialState.STATE_MINIGAME_PRACTICE_2:
                message = "Walk over to the medicine cabinet and click it.";
                ShowMedCab();
                break;

            case TutorialState.STATE_MINIGAME_PRACTICE_3:
                if (indicator != null)
                    Destroy(indicator);
                medCont = GameObject.FindGameObjectWithTag("BigMedCont").GetComponent<BigMedCont>();
                message = "This is the administration minigame. Use hand disinfectant first and then click Ibuprofen.";
                break;

            case TutorialState.STATE_MINIGAME_PRACTICE_4:
                morningCup = GameObject.FindGameObjectWithTag("morningCup").GetComponent<MedCup>();
                afternoonCup = GameObject.FindGameObjectWithTag("afternoonCup").GetComponent<MedCup>();
                eveningCup = GameObject.FindGameObjectWithTag("eveningCup").GetComponent<MedCup>();
                nightCup = GameObject.FindGameObjectWithTag("nightCup").GetComponent<MedCup>();
                message = "Here you gotta shoot pills to medicine cups with slingshot like in Angry Birds. Fire away!";
                break;

            case TutorialState.STATE_MINIGAME_PRACTICE_SPLITTING:
                message = "Nice shot! Some pills are splittable, and if you click them in the air during slow motion, the dosage will be cut in half.";
                StartCoroutine(ChangeState(TutorialState.STATE_MINIGAME_PRACTICE_5, 10f));
                break;

            case TutorialState.STATE_MINIGAME_PRACTICE_5:
                message = "Lets go give your patient his medicine now. Click the back-button in the top left corner twice.";
                break;

            case TutorialState.STATE_MINIGAME_PRACTICE_6:
                message = "Find your patient and click him.\nThen click medicine cup in your inventory to give it to the patient.";
                break;

            case TutorialState.STATE_ENDING_GOOD_1:
                if (mascot.currentState != Mascot.MascotState.STATE_NORMAL)
                    mascot.ChangeState(Mascot.MascotState.STATE_NORMAL);
                message = "Great job!\nThis is the basic idea of the game, give correct medicine at correct times to your patients.\n";
                StartCoroutine(ChangeState(TutorialState.STATE_COMPUTER_PRACTICE, 7f));
                break;

            case TutorialState.STATE_COMPUTER_PRACTICE:
                message = "There is a computer in the office. You can see your schedule and patients there.";
                ShowComputer();
                StartCoroutine(ChangeState(TutorialState.STATE_TRASH_PRACTICE, 8f));
                break;

            case TutorialState.STATE_TRASH_PRACTICE:
                message = "Next to the computer, there is a trash can.\nSelect it and click a medicine cup in your inventory to delete it.";
                ShowTrashCan();
                StartCoroutine(ChangeState(TutorialState.STATE_ENDING_GOOD_2, 8f));
                break;

            case TutorialState.STATE_ENDING_GOOD_2:
                scoreBarHighlight.SetActive(true);
                message = "In the score bar you can see how well you take care of your patients, and if it reaches zero, you will lose.\n";
                StartCoroutine(ChangeState(TutorialState.STATE_ENDING_GOOD_3, 10f));
                break;

            case TutorialState.STATE_ENDING_GOOD_3:
                message = "You will lose score points if you give wrong medicine or wrong dosage.";
                StartCoroutine(ChangeState(TutorialState.STATE_ENDING_GOOD_4, 7f));
                break;

            case TutorialState.STATE_ENDING_GOOD_4:
                scoreBarHighlight.SetActive(false);
                message = "Each day the number of patients you gotta take care of increases.";
                StartCoroutine(ChangeState(TutorialState.STATE_ENDING_GOOD_FINAL, 6f));
                break;

            case TutorialState.STATE_ENDING_GOOD_FINAL:
                message = "Good luck on your first day in the hospital, bye for now!";           
                Invoke("QuitTutorial", 6);
                break;

            case TutorialState.STATE_ENDING_BAD_1:
                message = "That medicine was incorrect or the dosage was wrong. Try again, the patient needs 400 mg of Ibuprofen.";
                mascot.ChangeState(Mascot.MascotState.STATE_ANGRY);
                break;

            case TutorialState.STATE_ENDING_BAD_2:
                message = "You killed the patient. You did that on purpose, didn't you? Are you satisfied now?!";
                Invoke("QuitTutorial", 12);
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
            typing = true;
            text.text += letter;
            yield return new WaitForSeconds(Time.deltaTime * letterPause);
            typing = false;
        }
    }

    public void TutorialYesClicked()
    {
        startOptions.StartButtonClicked();
        showPanels.HideTutorialPanel();
        tutorialOn = true;
        ShowtutCanvas();
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
        HidetutCanvas();
        tutorialOn = false;
        currentState = TutorialState.STATE_INACTIVE;
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().endTutorial();
    }

    public void ShowtutCanvas()
    {
        tutCanvas.SetActive(true);
    }

    public void HidetutCanvas()
    {
        tutCanvas.SetActive(false);
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
        yield return new WaitForSeconds(2f);
        mCamera.lockCameraToThisTransformForXTime(walkHere.transform, 3f);
        walkHere.SetActive(true);
        var pos = walkHere.transform.position;
        indicator = (GameObject)Instantiate(moveIndicatorPrefab, pos, new Quaternion(0, 0, 0, 0));
        indicator.transform.localScale = new Vector3(5, 5, 5);
    }

    public void ReachedWalkZone()
    {
        ChangeState(TutorialState.STATE_TARGET_PRACTICE_1);
    }

    void ShowNPC()
    {
        tutorialNPC = GameObject.Find("NPCManager").GetComponent<NPCManagerV2>().spawnTutorialGuy();
        mCamera.lockCameraToThisTransformForXTime(tutorialNPC.transform, 13f);
    }

    void ShowMedCab()
    {
        medCab = GameObject.FindGameObjectWithTag("MedCabinet");
        var pos = medCab.transform.position;
        pos.x -= 35;
        indicator = (GameObject)Instantiate(moveIndicatorPrefab, pos, new Quaternion(0, 0, 0, 0));
        indicator.transform.localScale = new Vector3(5, 5, 5);      
        mCamera.lockCameraToThisTransformForXTime(medCab.transform, 3f);
        minigame = GameObject.Find("Minigame1").GetComponent<Minigame1>();
    }

    void ShowComputer()
    {
        computer = GameObject.FindGameObjectWithTag("Computer");        
        mCamera.lockCameraToThisTransformForXTime(computer.transform, 16f);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>().setTarget(computer);
    }

    void ShowTrashCan()
    {
        trashCan = GameObject.FindGameObjectWithTag("TrashCan");
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>().setTarget(trashCan);
    }
}
