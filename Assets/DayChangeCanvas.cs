using UnityEngine;
using System.Collections;

public class DayChangeCanvas : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void continueButton()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().continueToNextDay();
    }
}
