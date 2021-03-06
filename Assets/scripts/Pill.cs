﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using Assets.Scripts;

public static class Constants
{
    public static readonly float MinVelocity = 0.05f;
    /// <summary>
    /// The collider of the pill is bigger when on idle state
    /// This will make it easier for the user to tap on it
    /// </summary>
    public static readonly float PillColliderRadiusNormal = 5f;
    public static readonly float PillColliderRadiusBig = 10f;
}

public class Pill : MonoBehaviour
{
    public PillState State
    {
        get;
        private set;
    }

    public string medName;
    public int dosage;
    public int canSplit;

    bool pillSplitOn;
    bool splitted;
    public bool doNotDestroy;

    Sprite pillSpriteHalf;
    public AudioSource splitSound;
    public AudioSource throwSound;

    public void Init(string medName, int dosage, Sprite pillSprite, int canSplit, Vector3 pos)
    {
        this.medName = medName;
        this.dosage = dosage;
        this.canSplit = canSplit;
        gameObject.GetComponent<SpriteRenderer>().sprite = pillSprite;
        // if this pill can be split, load the sprite for half a tab
        if (canSplit != 0)
            pillSpriteHalf = Resources.Load<Sprite>("Sprites/Meds/" + medName + "_tab_half");
        transform.localScale = new Vector3(0.1f, 0.1f, 0f); // scale sprite smaller
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;
        GetComponent<CircleCollider2D>().radius = Constants.PillColliderRadiusBig;
        State = PillState.BeforeThrown;
    }

    void Awake()
    {
        GameObject bigMedCont = GameObject.FindGameObjectWithTag("BigMedCont");   
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        // ignore collision between pills
        if (coll.gameObject.tag == "Pill" || coll.gameObject.tag == "disabledPill")
            Physics2D.IgnoreCollision(coll.gameObject.GetComponent<CircleCollider2D>(), GetComponent<CircleCollider2D>());
        // stop slowmo if hit the ground
        if (coll.gameObject.tag == "Ground")
        {
            Time.timeScale = 1.0f;
            Time.fixedDeltaTime = 0.02F * Time.timeScale;
        }
    }

    // called when pill enters split trigger
    public void splitPill(bool b)
    {
        pillSplitOn = b;
    }

    void FixedUpdate()
    {
        // if we've thrown the pill and it's not inside a cup (tag has not been changed)
        // and it's speed is very small
        if (State == PillState.Thrown && gameObject.tag == "Pill" &&
            GetComponent<Rigidbody2D>().velocity.sqrMagnitude <= Constants.MinVelocity)
        {
            // destroy the pill after 1 second
            StartCoroutine(DestroyAfter(1));
        }
    }

    IEnumerator DestroyAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (!doNotDestroy)
            Destroy(gameObject);
    }

    // called when pill is thrown
    public void OnThrow()
    {
        // play the sound
        throwSound.Play();
        // show the trail renderer
        GetComponent<TrailRenderer>().enabled = true;
        // allow for gravity forces
        GetComponent<Rigidbody2D>().isKinematic = false;
        // make the collider normal size
        GetComponent<CircleCollider2D>().radius = Constants.PillColliderRadiusNormal;
        State = PillState.Thrown;
    }

    void OnMouseDown()
    {
        // split the pill's dosage in half and change the sprite if clicked during slow motion
        if (pillSplitOn)
        {
            if (!splitted)
            {
                splitSound.Play();
                this.dosage = this.dosage / 2;
                splitted = true;
                gameObject.GetComponent<SpriteRenderer>().sprite = pillSpriteHalf;
                Time.timeScale = 1.0f;
                Time.fixedDeltaTime = 0.02F * Time.timeScale;
                if (GameObject.Find("Tutorial").GetComponent<Tutorial>().tutorialOn)
                    GameObject.Find("Tutorial").GetComponent<Tutorial>().PillSplitted();
            }
        }
    }

}
