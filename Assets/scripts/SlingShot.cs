using UnityEngine;
using System.Collections;
using Assets.Scripts;
using System;

public class SlingShot : MonoBehaviour {

    //a vector that points in the middle between left and right parts of the slingshot
    private Vector3 SlingshotMiddleVector;

    public SlingshotState slingshotState;

    //the left and right parts of the slingshot
    public Transform LeftSlingshotOrigin, RightSlingshotOrigin;

    //two line renderers to simulate the "strings" of the slingshot
    public LineRenderer SlingshotLineRenderer1;
    public LineRenderer SlingshotLineRenderer2;

    //this linerenderer will draw the projected trajectory of the thrown pill
    public LineRenderer TrajectoryLineRenderer;

    //the pill to throw
    public GameObject PillToThrow;

    //the position of the pill tied to the slingshot
    public Transform PillWaitPosition;

    public float ThrowSpeed;

    public float TimeSinceThrown;

    Vector3 v2;

    // Use this for initialization
    void Start()
    {    
        SlingshotLineRenderer1.SetPosition(0, LeftSlingshotOrigin.position);
        SlingshotLineRenderer2.SetPosition(0, RightSlingshotOrigin.position);

        //pointing at the middle position of the two vectors
        SlingshotMiddleVector = new Vector3((LeftSlingshotOrigin.position.x + RightSlingshotOrigin.position.x) / 2,
            (LeftSlingshotOrigin.position.y + RightSlingshotOrigin.position.y) / 2, 0);
    }

    // Update is called once per frame
    void Update()
    {
        switch (slingshotState)
        {
            case SlingshotState.Idle:
                //fix pill's position
                InitializePill();
                //display the slingshot "strings"
                DisplaySlingshotLineRenderers();
                if (Input.GetMouseButtonDown(0))
                {
                    //get the point on screen user has tapped
                    Vector3 location = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    //if user has tapped onto the pill
                    if (PillToThrow.GetComponent<CircleCollider2D>() == Physics2D.OverlapPoint(location))
                    {
                        GetComponent<AudioSource>().Play();
                        slingshotState = SlingshotState.UserPulling;
                    }
                }
                break;
            case SlingshotState.UserPulling:
                DisplaySlingshotLineRenderers();

                if (Input.GetMouseButton(0))
                {
                    //get where user is tapping
                    Vector3 location = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    location.z = PillToThrow.transform.position.z;
                    //we will let the user pull the pill up to a maximum distance
                    if (Vector3.Distance(location, SlingshotMiddleVector) > 1.5f)
                    {
                        //basic vector maths
                        Vector3 maxPosition = (location - SlingshotMiddleVector).normalized * 4f + SlingshotMiddleVector;
                        maxPosition.z = PillToThrow.transform.position.z;
                        PillToThrow.transform.position = maxPosition;
                    }
                    else
                    {
                        PillToThrow.transform.position = location;
                    }
                    float distance = Vector3.Distance(SlingshotMiddleVector, PillToThrow.transform.position);
                    //display projected trajectory based on the distance
                    DisplayTrajectoryLineRenderer2(distance);
                }
                else //user has removed the tap 
                {
                    SetTrajectoryLineRenderesActive(false);
                    //throw the pill!!!
                    TimeSinceThrown = Time.time;
                    float distance = Vector3.Distance(SlingshotMiddleVector, PillToThrow.transform.position);
                    SetSlingshotLineRenderersActive(false);
                    slingshotState = SlingshotState.PillFlying;
                    ThrowBird(distance);
                }
                break;
            case SlingshotState.PillFlying:
                GetComponent<AudioSource>().Stop();
                break;
            case SlingshotState.Inactive:
                break;
            default:
                break;
        }
    }

    private void ThrowBird(float distance)
    {
        //get velocity
        Vector3 velocity = SlingshotMiddleVector - PillToThrow.transform.position;
        PillToThrow.GetComponent<Pill>().OnThrow(); //make the pill aware of it
        //old and alternative way
        PillToThrow.GetComponent<Rigidbody2D>().AddForce
           (new Vector2(v2.x, v2.y) * ThrowSpeed * distance * 300 * Time.deltaTime);
        //set the velocity
        PillToThrow.GetComponent<Rigidbody2D>().velocity = new Vector2(velocity.x, velocity.y) * ThrowSpeed * distance;
    }

    private void InitializePill()
    {
        //initialization of the ready to be thrown pill
        if (PillToThrow == null) return;
        PillToThrow.transform.position = PillWaitPosition.position;
        slingshotState = SlingshotState.Idle;
        SetSlingshotLineRenderersActive(true);
    }

    void DisplaySlingshotLineRenderers()
    {
        if (PillToThrow == null) return;
        SlingshotLineRenderer1.SetPosition(1, PillToThrow.transform.position);
        SlingshotLineRenderer2.SetPosition(1, PillToThrow.transform.position);
    }

    void SetSlingshotLineRenderersActive(bool active)
    {
        SlingshotLineRenderer1.enabled = active;
        SlingshotLineRenderer2.enabled = active;
    }

    void SetTrajectoryLineRenderesActive(bool active)
    {
        TrajectoryLineRenderer.enabled = active;
    }


    /// <summary>
    /// Another solution can be found here
    /// http://wiki.unity3d.com/index.php?title=Trajectory_Simulation
    /// </summary>
    /// <param name="distance"></param>
    void DisplayTrajectoryLineRenderer2(float distance)
    {
        SetTrajectoryLineRenderesActive(true);
        v2 = SlingshotMiddleVector - PillToThrow.transform.position;
        int segmentCount = 15;
        float segmentScale = 2;
        Vector2[] segments = new Vector2[segmentCount];

        // The first line point is wherever the slingshot is
        segments[0] = PillToThrow.transform.position;

        // The initial velocity
        Vector2 segVelocity = new Vector2(v2.x, v2.y) * ThrowSpeed * distance;

        float angle = Vector2.Angle(segVelocity, new Vector2(1, 0));
        float time = segmentScale / segVelocity.magnitude;
        for (int i = 1; i < segmentCount; i++)
        {
            //x axis: spaceX = initialSpaceX + velocityX * time
            //y axis: spaceY = initialSpaceY + velocityY * time + 1/2 * accelerationY * time ^ 2
            //both (vector) space = initialSpace + velocity * time + 1/2 * acceleration * time ^ 2
            float time2 = i * Time.fixedDeltaTime * 5;
            segments[i] = segments[0] + segVelocity * time2 + 0.5f * Physics2D.gravity * Mathf.Pow(time2, 2);
        }

        TrajectoryLineRenderer.SetVertexCount(segmentCount);
        for (int i = 0; i < segmentCount; i++)
            TrajectoryLineRenderer.SetPosition(i, segments[i]);
    }

}
