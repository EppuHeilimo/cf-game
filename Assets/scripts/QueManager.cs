using UnityEngine;
using System.Collections;

public class QueManager : MonoBehaviour {

    const int MAX_QUE = 1 * 2; // max number of NPCs in queue (change the first number, the second [multiplier] is hack fix) ** MUST BE SAME AS MAX_NPCS IN NPC MANAGER! **
    bool[] queArr; // que slot taken = 1, que slot free = 0
    float offset; // x-space between NPCs in queue
    float[] quePos; // predefined array of queue x-positions

    // Use this for initialization
    void Start()
    {
        queArr = new bool[MAX_QUE];
        offset = 80;
        quePos = new float[MAX_QUE];
        for (int i = 0; i < MAX_QUE; i++)
        {
            quePos[i] = offset;
            offset = offset - 20;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public float addToQue()
    {
        for (int i = 0; i < MAX_QUE; i++)
        {
            if (!queArr[i])
            {
                queArr[i] = true;
                return quePos[i];
            }
        }
        return 666; // que full
    }

    // TODO: moveQue(), removeFromQue()
}
