using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/* changes the icon next to the clock depending on the current daytime */
public class ChangeSprite : MonoBehaviour
{
    ClockTime.DayTime currentDayTime;
    Sprite[] dayTimeIcons = new Sprite[4];

    void Start()
    {
        dayTimeIcons[0] = Resources.Load<Sprite>("Sprites/UI/morningIcon");
        dayTimeIcons[1] = Resources.Load<Sprite>("Sprites/UI/afternoonIcon");
        dayTimeIcons[2] = Resources.Load<Sprite>("Sprites/UI/eveningIcon");
        dayTimeIcons[3] = Resources.Load<Sprite>("Sprites/UI/nightIcon");
    }

    void Update()
    {
        currentDayTime = GameObject.FindGameObjectWithTag("Clock").GetComponent<ClockTime>().currentDayTime;
        switch (currentDayTime)
        {
            case ClockTime.DayTime.MORNING:
                gameObject.GetComponent<Image>().sprite = dayTimeIcons[0];
                break;
            case ClockTime.DayTime.AFTERNOON:
                gameObject.GetComponent<Image>().sprite = dayTimeIcons[1];
                break;
            case ClockTime.DayTime.EVENING:
                gameObject.GetComponent<Image>().sprite = dayTimeIcons[2];
                break;
            case ClockTime.DayTime.NIGHT:
                gameObject.GetComponent<Image>().sprite = dayTimeIcons[3];
                break;
        }
    }
}
