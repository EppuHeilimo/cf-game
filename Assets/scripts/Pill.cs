using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Pill : MonoBehaviour
{
    public string medName;
    public int dosage;
    public int canSplit;

    public float maxStretch = 2.0f;
    float maxStretchSqr;
    bool clickedOn;
    bool isFlying;

    public LineRenderer Line;
    SpringJoint2D spring;
    Transform catapult;
    Rigidbody2D rigbody;
    Ray rayToMouse;  
    Vector2 force;
    Ray catapultToProjectileRay;

    bool pillSplitOn;
    bool splitted;

    Sprite pillSpriteHalf;

    public void Init(string medName, int dosage, Sprite pillSprite, int canSplit)
    {
        this.medName = medName;
        this.dosage = dosage;
        this.canSplit = canSplit;
        gameObject.GetComponent<SpriteRenderer>().sprite = pillSprite;
        if (canSplit != 0)
            pillSpriteHalf = Resources.Load<Sprite>("Sprites/Meds/" + medName + "_tab_half");
        transform.localScale = new Vector3(0.1f, 0.1f, 0f); //scale sprite smaller
        isFlying = false;
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;
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

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Pill" || coll.gameObject.tag == "disabledPill")
            Physics2D.IgnoreCollision(coll.gameObject.GetComponent<CircleCollider2D>(), GetComponent<CircleCollider2D>());
    }

    void Update()
    {
        if (clickedOn && !isFlying)
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

        if (pillSplitOn)
        {
            if (!splitted)
            {
                this.dosage = this.dosage / 2;
                splitted = true;
                gameObject.GetComponent<SpriteRenderer>().sprite = pillSpriteHalf;
            }
        }
    }

    void OnMouseUp()
    {
        if (spring != null)
            spring.enabled = true;
        rigbody.isKinematic = false;
        clickedOn = false;
        isFlying = true;
        Invoke("StuckCheck", 10);
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

    public void splitPill(bool b)
    {
        pillSplitOn = b;
    }

    public void StuckCheck()
    {
        if (isFlying && gameObject.tag == "Pill")
            Destroy(gameObject);
    }

}
