using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class QuitApplication : MonoBehaviour {

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
            if (menu.GetComponent<Pause>() != null)
                menu.GetComponent<Pause>().UnPause();
            Destroy(menu);
        }
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
