using UnityEngine;
using System.Collections;

public class ShowPanels : MonoBehaviour {

	public GameObject optionsPanel;							//Store a reference to the Game Object OptionsPanel 
	public GameObject optionsTint;							//Store a reference to the Game Object OptionsTint 
	public GameObject menuPanel;							//Store a reference to the Game Object MenuPanel 
	public GameObject pausePanel;							//Store a reference to the Game Object PausePanel 
    public GameObject tutorialPromptPanel;
    public GameObject customizePanel;
    public CameraMovement mainCamera;
    public GameObject gameCanvas;

    public GameObject player; 

    void Start()
    {
        mainCamera = GameObject.Find("Main Camera").GetComponent<CameraMovement>();
        gameCanvas = GameObject.Find("Canvas");
        player = GameObject.FindGameObjectWithTag("Player");
        customizePanel.SetActive(false);
    }

    //Call this function to activate and display the Options panel during the main menu
    public void ShowOptionsPanel()
	{
		optionsPanel.SetActive(true);
		optionsTint.SetActive(true);
        
	}

    public void ShowCustomizePanel()
    {
        customizePanel.SetActive(true);
        menuPanel.SetActive(false);
        gameCanvas.SetActive(false);
        player.SetActive(false);
        mainCamera.SwitchToCustomizeCamera();

    }

    public void HideCustomizePanel()
    {
        menuPanel.SetActive(true);
        customizePanel.SetActive(false);
        gameCanvas.SetActive(true);
        player.SetActive(true);
        mainCamera.SwitchBackFromCustomizeCamera();
    }

	//Call this function to deactivate and hide the Options panel during the main menu
	public void HideOptionsPanel()
	{
		optionsPanel.SetActive(false);
		optionsTint.SetActive(false);
	}

	//Call this function to activate and display the main menu panel during the main menu
	public void ShowMenu()
	{
		menuPanel.SetActive (true);
	}

	//Call this function to deactivate and hide the main menu panel during the main menu
	public void HideMenu()
	{
		menuPanel.SetActive (false);
	}
	
	//Call this function to activate and display the Pause panel during game play
	public void ShowPausePanel()
	{
		pausePanel.SetActive (true);
		optionsTint.SetActive(true);
	}

	//Call this function to deactivate and hide the Pause panel during game play
	public void HidePausePanel()
	{
		pausePanel.SetActive (false);
		optionsTint.SetActive(false);

	}

    public void ShowTutorialPanel()
    {
        tutorialPromptPanel.SetActive(true);
        optionsTint.SetActive(true);
    }

    public void HideTutorialPanel()
    {
        tutorialPromptPanel.SetActive(false);
        optionsTint.SetActive(false);
    }
}
