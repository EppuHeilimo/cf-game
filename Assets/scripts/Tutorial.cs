using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tutorial : MonoBehaviour {

    // the tutorial is made with state machine
    public enum TutorialState
    {
        STATE_START = 0,

        STATE_WALK_PRACTICE_1,

        STATE_TARGET_PRACTICE_1,
        STATE_TARGET_PRACTICE_2,
        STATE_TARGET_PRACTICE_3,
        STATE_TARGET_PRACTICE_4,
        STATE_TARGET_PRACTICE_5,

        STATE_MINIGAME_PRACTICE_1,
        STATE_MINIGAME_PRACTICE_2,
        STATE_MINIGAME_PRACTICE_2_1,
        STATE_MINIGAME_PRACTICE_3,
        STATE_MINIGAME_PRACTICE_4,
        STATE_MINIGAME_PRACTICE_5,
        STATE_MINIGAME_PRACTICE_6,
        STATE_MINIGAME_PRACTICE_SPLITTING_1,
        STATE_MINIGAME_PRACTICE_SPLITTING_2,
        STATE_MINIGAME_PRACTICE_SPLITTING_3,

        STATE_ENDING_GOOD_1,
        STATE_ENDING_GOOD_2,
        STATE_ENDING_GOOD_3,
        STATE_ENDING_GOOD_4,
        STATE_ENDING_GOOD_FINAL,
        STATE_ENDING_BAD_1,

        STATE_COMPUTER_PRACTICE,
        STATE_COMPUTER_PRACTICE_1,
        STATE_COMPUTER_PRACTICE_2,
        STATE_TRASH_PRACTICE,
        STATE_COFFEE_PRACTICE,

        STATE_INACTIVE,
        STATE_SHOW_NOTIFICATION
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
    GameObject coffee;
    float timeSpentInThisState;
    const float GO_TO_SLEEP_TIME = 20f;
    bool nextClicked;
    GameObject nextBtn;

    public Slider musicOptionsSlider;
    public Slider musicPauseSlider;
    public Toggle toggle;
    public Toggle toggleMenu;

    string notificationMsg;
    float notificationTime;

    /* UI elements position stuff */
    GameObject[] uiElements;
    Vector3[] uiElementsOrigPos = new Vector3[3];    
    enum uiElementsPos
    {
        ORIG = 0,
        MINIGAME,
        DOSING
    }
    uiElementsPos currPos;

    // Use this for initialization
    void Start () {     
        tutCanvas = transform.GetChild(0).gameObject;
        nextBtn = GameObject.Find("TutorialNextBtn");
        uiElements = GameObject.FindGameObjectsWithTag("TutorialUI");
        // save the tutorial UI elements original positions
        for (int i = 0; i < uiElements.Length; i++)
            uiElementsOrigPos[i] = uiElements[i].GetComponent<RectTransform>().position;
        currPos = uiElementsPos.ORIG;
        mascot = GameObject.Find("Mascot").GetComponent<Mascot>();
        currentState = TutorialState.STATE_INACTIVE;
        HideTutCanvas();
        walkHere.SetActive(false);
        minigame = GameObject.Find("Minigame1").GetComponent<Minigame1>();
    }

    // Update is called once per frame
    void Update () {
        if (tutorialOn)
        {
            // spent too long in one state, play the mascot's sleep animation
            timeSpentInThisState += Time.deltaTime;
            if (timeSpentInThisState >= GO_TO_SLEEP_TIME)
            {
                mascot.ChangeState(Mascot.MascotState.STATE_SLEEP);
            }

            // initialize new state if changed
            if (stateChanged)
            {
                OnStateChange();
            }

            // play mascot's talking animation if message is currently being typed
            if (mascot != null)
                mascot.isTalking = typing;

            // move tutorial UI-elements out of the way in minigame
            if (minigame.active && minigame.dosingActive)
            {
                if (currPos != uiElementsPos.DOSING)
                {
                    MoveuiElements(0, Screen.height / 2 - 60f);
                    currPos = uiElementsPos.DOSING;
                }               
            }
            else if (minigame.active)
            {
                if (currPos != uiElementsPos.MINIGAME)
                {
                    MoveuiElements(-Screen.width / 2, -50f);
                    currPos = uiElementsPos.MINIGAME;
                }
            }
            else
            {
                if (currPos != uiElementsPos.ORIG)
                {
                    MoveuiElements(0, 0);
                    currPos = uiElementsPos.ORIG;
                }
            }

            // update stuff specific to the current state
            switch (currentState)
            {
                case TutorialState.STATE_START:
                    if (nextClicked)
                        ChangeState(TutorialState.STATE_WALK_PRACTICE_1);
                    break;

                case TutorialState.STATE_TARGET_PRACTICE_2:
                    if (nextClicked)
                    {
                        mCamera.lockToPlayer();
                        ChangeState(TutorialState.STATE_TARGET_PRACTICE_3);
                    }
                    break;

                case TutorialState.STATE_TARGET_PRACTICE_3:
                    if (nextClicked)
                        ChangeState(TutorialState.STATE_TARGET_PRACTICE_4);
                    break;

                case TutorialState.STATE_TARGET_PRACTICE_4:
                    // check if player has targeted the patient
                    GameObject tp = GameObject.FindGameObjectWithTag("TargetPanel");
                    if (tp != null && tutorialNPC.GetComponent<NPC>().diagnosed)
                    {
                        ChangeState(TutorialState.STATE_TARGET_PRACTICE_5);
                    }
                    break;

                case TutorialState.STATE_MINIGAME_PRACTICE_1:
                    if (nextClicked)
                        ChangeState(TutorialState.STATE_MINIGAME_PRACTICE_2);
                    break;

                case TutorialState.STATE_MINIGAME_PRACTICE_2:
                    // check if player has opened the medicine cabinet
                    if (minigame.active)
                    {
                        ChangeState(TutorialState.STATE_MINIGAME_PRACTICE_2_1);
                    }
                    break;

                case TutorialState.STATE_MINIGAME_PRACTICE_2_1:
                    if (nextClicked)
                        ChangeState(TutorialState.STATE_MINIGAME_PRACTICE_3);
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
                        ChangeState(TutorialState.STATE_MINIGAME_PRACTICE_SPLITTING_1);
                    }
                    break;

                case TutorialState.STATE_MINIGAME_PRACTICE_SPLITTING_3:
                    if (nextClicked)
                        ChangeState(TutorialState.STATE_MINIGAME_PRACTICE_5);
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
                    if (tutorialNPC.GetComponent<NPC>().morningMed[0].isActive)
                    {
                        ChangeState(TutorialState.STATE_ENDING_GOOD_1);
                    }
                    // wrong medicine/dosage given
                    else if (tutorialNPC.GetComponent<NPC>().myHp < 50)
                    {
                        ChangeState(TutorialState.STATE_ENDING_BAD_1);
                    }
                    break;

                case TutorialState.STATE_ENDING_GOOD_1:
                    if (nextClicked)
                        ChangeState(TutorialState.STATE_COMPUTER_PRACTICE);
                    break;

                case TutorialState.STATE_ENDING_BAD_1:
                    // check if medicine given to the patient
                    if (tutorialNPC.GetComponent<NPC>().morningMed[0].isActive)
                    {
                        ChangeState(TutorialState.STATE_ENDING_GOOD_1);
                    }
                    break;

                case TutorialState.STATE_COMPUTER_PRACTICE:
                    // check if computer turned on
                    if (computer.GetComponent<Computer>().computerOn)
                        ChangeState(TutorialState.STATE_COMPUTER_PRACTICE_1);
                    break;

                case TutorialState.STATE_COMPUTER_PRACTICE_1:
                    // check if schedule opened
                    if (computer.GetComponent<Computer>().scheduleOn)
                        ChangeState(TutorialState.STATE_COMPUTER_PRACTICE_2);
                    break;

                case TutorialState.STATE_COMPUTER_PRACTICE_2:
                    // check if computer turned off
                    if (!computer.GetComponent<Computer>().computerOn)
                        ChangeState(TutorialState.STATE_TRASH_PRACTICE);
                    break;

                case TutorialState.STATE_TRASH_PRACTICE:
                    if (nextClicked)
                    {
                        mCamera.lockToPlayer();
                        ChangeState(TutorialState.STATE_COFFEE_PRACTICE);
                    }
                    break;

                case TutorialState.STATE_COFFEE_PRACTICE:
                    if (nextClicked)
                    {
                        mCamera.lockToPlayer();
                        ChangeState(TutorialState.STATE_ENDING_GOOD_2);
                    }
                    break;

                case TutorialState.STATE_ENDING_GOOD_2:
                    if (nextClicked)
                        ChangeState(TutorialState.STATE_ENDING_GOOD_3);
                    break;

                case TutorialState.STATE_ENDING_GOOD_3:
                    if (nextClicked)
                        ChangeState(TutorialState.STATE_ENDING_GOOD_4);
                    break;

                case TutorialState.STATE_ENDING_GOOD_4:
                    if (nextClicked)
                        ChangeState(TutorialState.STATE_ENDING_GOOD_FINAL);
                    break;

                case TutorialState.STATE_INACTIVE:
                    // do nothing...
                    break;
            }
        }
        // tutorial not on, show notifications
        else
        {
            if (stateChanged)
            {
                OnStateChange();
            }

            if (mascot != null)
                mascot.isTalking = typing;
        }
	}

    // initialize new state
    void OnStateChange()
    {
        timeSpentInThisState = 0f;
        nextClicked = false;
        StopAllCoroutines();
        text.text = "";

        switch (currentState)
        {
            case TutorialState.STATE_START:
                nextBtn.SetActive(true);
                message = "Hi, welcome to the tutorial! Click the V-button to continue.";
                break;

            case TutorialState.STATE_WALK_PRACTICE_1:
                nextBtn.SetActive(false);
                message = "Lets start by walking to the reception. Just click on the ground to move.";
                StartCoroutine(ShowPath());
                break;

            case TutorialState.STATE_TARGET_PRACTICE_1:
                if (indicator != null)
                    Destroy(indicator);
                walkHere.SetActive(false);
                message = "Great job!";
                StartCoroutine(ChangeState(TutorialState.STATE_TARGET_PRACTICE_2, 1f));
                break;

            case TutorialState.STATE_TARGET_PRACTICE_2:
                nextBtn.SetActive(true);
                message = "Here comes a patient! Patients go first to the doctor's office to get diagnosed.";
                ShowNPC();
                break;

            case TutorialState.STATE_TARGET_PRACTICE_3:
                message = "Patients with red cross above their head are your responsibility.";
                break;

            case TutorialState.STATE_TARGET_PRACTICE_4:
                nextBtn.SetActive(false);
                message = "Double-click the patient to walk to him and see his information.";
                break;

            case TutorialState.STATE_TARGET_PRACTICE_5:
                message = "Good job!";
                StartCoroutine(ChangeState(TutorialState.STATE_MINIGAME_PRACTICE_1, 1f));
                break;

            case TutorialState.STATE_MINIGAME_PRACTICE_1:
                nextBtn.SetActive(true);
                message = "You can see on the patient's medicine card that he needs Ibuprofen. Lets go get some!";
                break;

            case TutorialState.STATE_MINIGAME_PRACTICE_2:
                nextBtn.SetActive(false);
                message = "Double-click the medicine cabinet to use it.";
                ShowMedCab();
                break;

            case TutorialState.STATE_MINIGAME_PRACTICE_2_1:
                nextBtn.SetActive(true);
                if (indicator != null)
                    Destroy(indicator);
                message = "This is the administration minigame. You can see all of your patients medicine cards on the left.";
                break;

            case TutorialState.STATE_MINIGAME_PRACTICE_3:
                nextBtn.SetActive(false);
                medCont = GameObject.FindGameObjectWithTag("BigMedCont").GetComponent<BigMedCont>();
                message = "Your patient needs 400 mg of Ibuprofen. Use hand disinfectant first and then choose Ibuprofen.";
                break;

            case TutorialState.STATE_MINIGAME_PRACTICE_4:
                morningCup = GameObject.FindGameObjectWithTag("morningCup").GetComponent<MedCup>();
                afternoonCup = GameObject.FindGameObjectWithTag("afternoonCup").GetComponent<MedCup>();
                eveningCup = GameObject.FindGameObjectWithTag("eveningCup").GetComponent<MedCup>();
                nightCup = GameObject.FindGameObjectWithTag("nightCup").GetComponent<MedCup>();
                message = "Here you gotta shoot pills to medicine cups with slingshot like in Angry Birds. Fire away!";
                break;

            case TutorialState.STATE_MINIGAME_PRACTICE_SPLITTING_1:
                message = "Nice shot!";
                StartCoroutine(ChangeState(TutorialState.STATE_MINIGAME_PRACTICE_SPLITTING_2, 1f));
                break;

            case TutorialState.STATE_MINIGAME_PRACTICE_SPLITTING_2:
                message = "You can split some pills in half by clicking them in the air during slow motion. Try it out!";
                break;

            case TutorialState.STATE_MINIGAME_PRACTICE_SPLITTING_3:
                nextBtn.SetActive(true);
                message = "Cool! Clicking the button on medicine cup removes the last pill from the cup. Clicking it twice removes all pills.";
                break;

            case TutorialState.STATE_MINIGAME_PRACTICE_5:
                nextBtn.SetActive(false);
                message = "Lets go give your patient his medicine. Click the X-button in the top-left corner twice.";
                break;

            case TutorialState.STATE_MINIGAME_PRACTICE_6:
                message = "Find your patient and double-click him. Then click medicine cup in your inventory (bottom-right corner) to give it to him.";
                break;

            case TutorialState.STATE_ENDING_GOOD_1:
                if (mascot.currentState != Mascot.MascotState.STATE_NORMAL)
                    mascot.ChangeState(Mascot.MascotState.STATE_NORMAL);
                nextBtn.SetActive(true);
                message = "Great job! This is the basic idea of the game, give correct medicine at correct times to your patients.";
                break;

            case TutorialState.STATE_COMPUTER_PRACTICE:
                nextBtn.SetActive(false);
                message = "There is a computer in the office. You can see your schedule and patients there. Go use it!";
                ShowComputer();
                break;

            case TutorialState.STATE_COMPUTER_PRACTICE_1:
                nextBtn.SetActive(false);
                message = "Check out your daily schedule by clicking the Skeduler-button.";
                break;

            case TutorialState.STATE_COMPUTER_PRACTICE_2:
                message = "Close the computer by pressing the X-button in the top-left corner.";
                break;

            case TutorialState.STATE_TRASH_PRACTICE:
                nextBtn.SetActive(true);
                message = "Next to the computer, there is a trash can. Double-click it and click a medicine cup in your inventory to delete it.";
                ShowTrashCan();
                break;

            case TutorialState.STATE_COFFEE_PRACTICE:
                nextBtn.SetActive(true);
                message = "There is a coffeemaker in the office too. Use it if you want to skip time.";
                ShowCoffee();
                break;

            case TutorialState.STATE_ENDING_GOOD_2:
                if (indicator != null)
                    Destroy(indicator);
                scoreBarHighlight.SetActive(true);
                message = "In the score bar you can see how well you take care of your patients, and if it reaches zero, you will lose.";
                break;

            case TutorialState.STATE_ENDING_GOOD_3:
                message = "You will lose score points if you give wrong medicine or wrong dosage.";
                break;

            case TutorialState.STATE_ENDING_GOOD_4:
                scoreBarHighlight.SetActive(false);
                message = "Each day the number of patients you gotta take care of increases. The patients will start to arrive at 7 am.";
                break;

            case TutorialState.STATE_ENDING_GOOD_FINAL:
                nextBtn.SetActive(false);
                message = "Good luck on your first day in the hospital, bye for now!";           
                Invoke("QuitTutorial", 6);
                break;

            case TutorialState.STATE_ENDING_BAD_1:
                message = "That medicine was incorrect or the dosage was wrong. Try again, the patient needs 400 mg of Ibuprofen.";
                mascot.ChangeState(Mascot.MascotState.STATE_ANGRY);
                break;

            case TutorialState.STATE_INACTIVE:
                message = "";
                break;

            case TutorialState.STATE_SHOW_NOTIFICATION:
                message = notificationMsg;
                Invoke("HideNotification", notificationTime);
                break;
        }
        StartCoroutine(TypeText());
        stateChanged = false;
    }

    // types the message letter by letter
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

    // start tutorial
    public void TutorialYesClicked()
    {
        startOptions.StartButtonClicked();
        showPanels.HideTutorialPanel();
        tutorialOn = true;
        ShowTutCanvas();
        StartCoroutine(ChangeState(TutorialState.STATE_START, 1f));
        GameObject.Find("Player").GetComponent<PlayerControl>().loadProfile();
        musicPauseSlider.value = musicOptionsSlider.value;
        toggle.isOn = toggleMenu.isOn;
    }

    // skip tutorial
    public void TutorialNoClicked()
    {
        startOptions.StartButtonClicked();
        showPanels.HideTutorialPanel();
        tutorialOn = false;
        GameObject.Find("Player").GetComponent<PlayerControl>().loadProfile();
        musicPauseSlider.value = musicOptionsSlider.value;
        toggle.isOn = toggleMenu.isOn;
    }

    public void QuitTutorial()
    {     
        if (indicator != null)
            Destroy(indicator);
        HideTutCanvas();
        tutorialOn = false;
        currentState = TutorialState.STATE_INACTIVE;
        GameObject.Find("Inventory").GetComponent<Inventory>().ResetInventory();
        if (minigame.active)
        {
            minigame.quitDosingGame();
            minigame.quitMinigame();
        }
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().endTutorial();
    }

    public void ShowTutCanvas()
    {
        tutCanvas.SetActive(true);
    }

    public void HideTutCanvas()
    {
        tutCanvas.SetActive(false);
    }

    // change state after delay
    IEnumerator ChangeState(TutorialState newState, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        currentState = newState;
        stateChanged = true;
    }

    // change state instantly
    void ChangeState(TutorialState newState)
    {
        currentState = newState;
        stateChanged = true;
    }
        
    // shows the path to the reception
    IEnumerator ShowPath()
    {
        yield return new WaitForSeconds(1f);
        mCamera.lockCameraToThisTransformForXTime(walkHere.transform, 3f);
        walkHere.SetActive(true);
        var pos = walkHere.transform.position;
        indicator = (GameObject)Instantiate(moveIndicatorPrefab, pos, new Quaternion(0, 0, 0, 0));
        indicator.transform.localScale = new Vector3(5, 5, 5);
    }

    // called from the walkzone trigger once player has entered the reception zone
    public void ReachedWalkZone()
    {
        ChangeState(TutorialState.STATE_TARGET_PRACTICE_1);
    }

    // spawn and show the tutorial NPC
    void ShowNPC()
    {
        tutorialNPC = GameObject.Find("NPCManager").GetComponent<NPCManager>().spawnTutorialGuy();
        mCamera.lockCameraToThisTransformForXTime(tutorialNPC.transform, 15f);
    }

    // show different gameobjects
    void ShowMedCab()
    {
        medCab = GameObject.FindGameObjectWithTag("MedCabinet");
        var pos = medCab.transform.position;
        pos.x -= 35;
        indicator = (GameObject)Instantiate(moveIndicatorPrefab, pos, new Quaternion(0, 0, 0, 0));
        indicator.transform.localScale = new Vector3(5, 5, 5);      
        mCamera.lockCameraToThisTransformForXTime(medCab.transform, 3f);
    }

    void ShowComputer()
    {
        computer = GameObject.FindGameObjectWithTag("Computer");
        var pos = computer.transform.position;
        pos.x -= 35;
        indicator = (GameObject)Instantiate(moveIndicatorPrefab, pos, new Quaternion(0, 0, 0, 0));
        indicator.transform.localScale = new Vector3(5, 5, 5);
        mCamera.lockCameraToThisTransformForXTime(computer.transform, 5f);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>().setTarget(computer);
    }

    void ShowTrashCan()
    {
        if (indicator != null)
            Destroy(indicator);
        trashCan = GameObject.FindGameObjectWithTag("TrashCan");
        var pos = trashCan.transform.position;
        pos.x -= 35;
        indicator = (GameObject)Instantiate(moveIndicatorPrefab, pos, new Quaternion(0, 0, 0, 0));
        indicator.transform.localScale = new Vector3(5, 5, 5);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>().setTarget(trashCan);
    }

    void ShowCoffee()
    {
        if (indicator != null)
            Destroy(indicator);
        coffee = GameObject.FindGameObjectWithTag("CoffeeMachine");
        var pos = coffee.transform.position;
        pos.x -= 35;
        indicator = (GameObject)Instantiate(moveIndicatorPrefab, pos, new Quaternion(0, 0, 0, 0));
        indicator.transform.localScale = new Vector3(5, 5, 5);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>().setTarget(coffee);
    }

    public void TutorialNextClicked()
    {
        nextClicked = true;
    }

    // called when pill has been split in the minigame
    public void PillSplitted()
    {
        if (currentState == TutorialState.STATE_MINIGAME_PRACTICE_SPLITTING_2)
            ChangeState(TutorialState.STATE_MINIGAME_PRACTICE_SPLITTING_3);
    }

    // moves the tutorial UI-elements in x- and y-direction from the original position
    void MoveuiElements(float x, float y)
    {
        for (int i = 0; i < uiElements.Length; i++)
            uiElements[i].GetComponent<RectTransform>().position = new Vector3(uiElementsOrigPos[i].x + x, uiElementsOrigPos[i].y + y, uiElementsOrigPos[i].z);
    }

    // display the mascot to show a message for a given time, and if the mascot animation should be angry or not
    public void ShowNotification(string msg, float time, bool angry)
    {
        if (currentState != TutorialState.STATE_SHOW_NOTIFICATION)
        { 
            ShowTutCanvas();
            nextBtn.SetActive(false);
            notificationMsg = msg;
            notificationTime = time;
            if (angry)
            { 
                mascot.ChangeState(Mascot.MascotState.STATE_ANGRY);
            }
            else
            {
                if (mascot.currentState != Mascot.MascotState.STATE_NORMAL)
                    mascot.ChangeState(Mascot.MascotState.STATE_NORMAL);
            }
            ChangeState(TutorialState.STATE_SHOW_NOTIFICATION);
        }
    }

    public void HideNotification()
    {
        HideTutCanvas();
        notificationMsg = "";
        notificationTime = 0f;
        ChangeState(TutorialState.STATE_INACTIVE);
    }
}
