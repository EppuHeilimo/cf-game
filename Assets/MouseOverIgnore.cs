using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class MouseOverIgnore : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public bool ignore = false;
    CanvasGroup cGroup;
    float timer = 0;
    float speed = 2.0f;
    float targetAlpha = 0.5f;
    bool entered;
    bool exited;


    void Start()
    {
        cGroup = GetComponent<CanvasGroup>();
    }
    
    void Update()
    {
        if(entered)
        {
            if(cGroup.alpha > targetAlpha)
            {
                cGroup.alpha = cGroup.alpha - Time.deltaTime * speed;
            }
            if(cGroup.alpha <= targetAlpha)
            {
                cGroup.alpha = targetAlpha;
                entered = false;
            }
        }
        else if(exited)
        {
            if(cGroup.alpha < 1.0f)
            {
                cGroup.alpha = cGroup.alpha + Time.deltaTime * speed;
            }
            if (cGroup.alpha >= 1.0f)
            {
                cGroup.alpha = 1.0f;
                exited = false;
            }
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        entered = true;
        exited = false;
        ignore = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        exited = true;
        entered = false;
        ignore = false;
    }

}
