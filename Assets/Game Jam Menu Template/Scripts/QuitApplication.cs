using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuitApplication : MonoBehaviour {

    public Slider slider;
    public Toggle toggle;

	public void Quit()
	{
        //Quit the application
        Application.Quit();

        //If we are running in the editor
    #if UNITY_EDITOR
        //Stop playing the scene
        UnityEditor.EditorApplication.isPlaying = false;
    #endif
    }

    public void RestartGame()
    {
        GameObject menu = GameObject.Find("Menu");
        if (menu != null)
        {
            // save the sound options values before destroying menu
            GameObject.Find("DontDestroy").GetComponent<SavedVariables>().SaveValues(slider.value, toggle.isOn);
            if (menu.GetComponent<Pause>() != null)
                menu.GetComponent<Pause>().UnPause();

            Destroy(menu);
        }
        // reload the whole scene
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
