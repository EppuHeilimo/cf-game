using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    // enums for the state of the slingshot, the 
    // state of the game and the state of the pill
    public enum SlingshotState
    {
        Inactive,
        Idle,
        UserPulling,
        PillFlying
    }

    public enum MiniGameState
    {
        Inactive,
        Start,
        PillMovingToSlingshot,
        Playing
    }

    public enum PillState
    {
        BeforeThrown,
        Thrown
    }

}
