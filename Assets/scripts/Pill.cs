using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Pill : MonoBehaviour
{
    public string medName;
    public int dosage;

    public float maxStretch = 2.0f;
    float maxStretchSqr;
    bool clickedOn;

    public LineRenderer Line;
    SpringJoint2D spring;
    Transform catapult;
    Rigidbody2D rigbody;
    Ray rayToMouse;  
    Vector2 force;
    Ray catapultToProjectileRay;

    public void Init(string medName, int dosage)
    {
        this.medName = medName;
        this.dosage = dosage;     
    }

    void Awake()
    {
        GameObject bigMedCont = GameObject.FindGameObjectWithTag("BigMedCont");
        spring = GetComponent<SpringJoint2D>();
        Line = bigMedCont.GetComponent<LineRenderer>();
        rigbody = GetComponent<Rigidbody2D>();
        catapult = bigMedCont.transform;
        LineRendererSetup();
        rayToMouse = new Ray(catapult.position, Vector3.zero);
        catapultToProjectileRay = new Ray(Line.transform.position, Vector3.zero);
        maxStretchSqr = maxStretch * maxStretch;       
    }

    void Update()
    {
        if (clickedOn)
        {
            Dragging();
        }

        if (spring != null)
        {
            if (!rigbody.isKinematic)
            {
                Destroy(spring);
                rigbody.velocity = force;
                rigbody.angularVelocity = force.magnitude * 10f;
            }
            LineRendererUpdate();
        }
        else
        {
            Line.enabled = false;
        }
    }

    void LineRendererSetup()
    {      
        Line.SetPosition(0, Line.transform.position);
        Line.sortingOrder = 5;    
    }

    void OnMouseDown()
    {
        if (spring != null)
            spring.enabled = false;
        clickedOn = true;
    }

    void OnMouseUp()
    {
        if (spring != null)
            spring.enabled = true;
        rigbody.isKinematic = false;
        clickedOn = false;
    }

    void Dragging()
    {
        Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 catapultToMouse = mouseWorldPoint - catapult.position;
        force = new Vector2(-catapultToMouse.x * 5, -catapultToMouse.y * 5);
        if (catapultToMouse.sqrMagnitude > maxStretchSqr)
        {
            rayToMouse.direction = catapultToMouse;
            mouseWorldPoint = rayToMouse.GetPoint(maxStretch);
        }
        mouseWorldPoint.z = transform.position.z;
        transform.position = mouseWorldPoint;    
    }

    void LineRendererUpdate()
    {
        Vector2 catapultToProjectile = transform.position - Line.transform.position;
        catapultToProjectileRay.direction = catapultToProjectile;
        Vector3 holdPoint = catapultToProjectileRay.GetPoint(catapultToProjectile.magnitude);
        Line.SetPosition(1, holdPoint);
        if (Line.enabled == false)
            Line.enabled = true;
    }

    //TODO: pillerin puolitus, puolittaa dosagen, slowmotion triggerillä
}
