using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FallingItem : MonoBehaviour {

    public Transform fallEnd;
    
    float speed;
    Sprite sprite;
    int randItem;
    int randSpeed;
    int randNumb;
    public int randRotation;
    int[] RandRotations = new int[4] { 180, -120, 140, 180 };

    void Start()
    {
        fallEnd = GameObject.Find("FallEnd").transform;
        randItem = Random.Range(1, 4);
        sprite = Resources.Load<Sprite>("Sprites/Menu/menu_item" + randItem.ToString());
        gameObject.GetComponent<Image>().sprite = sprite;
        randSpeed = Random.Range(10, 15);
        randNumb = Random.Range(0, 4);
        randRotation = RandRotations[randNumb];
        var tween = Go.to(transform, randSpeed, new GoTweenConfig()
            .position(new Vector3(transform.position.x, fallEnd.position.y, transform.position.z))
            .rotation(new Vector3(0, 0, randRotation))
            .setIterations(-1, GoLoopType.RestartFromBeginning));
    }

}
