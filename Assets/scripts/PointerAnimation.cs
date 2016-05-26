using UnityEngine;
using System.Collections;

public class PointerAnimation : MonoBehaviour {
    Vector3 defPos;
    float animSpeed = 5.1f;
    float ypos;
    float x = 0.0f;
    float maxpos = 10.0f;
    // Use this for initialization
    void Start () {
        defPos = transform.position;
        ypos = defPos.y;
    }
	
	// Update is called once per frame
	void Update () {
        if (x > 10000.0f)
            x = 0.0f;
        x += Time.deltaTime * animSpeed;
        ypos = Mathf.Abs(Mathf.Sin(x)) * maxpos;
        transform.position = new Vector3(defPos.x, ypos, defPos.z );
	}
}
