using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/*
 * Handles booking for objects. This way 2 npc's wont use the same object at the same time
 */
public class ObjectManager : MonoBehaviour {

    Dictionary<GameObject, GameObject> bookableObjects = new Dictionary<GameObject, GameObject>();
    // Use this for initialization
    void Start () {

        GameObject[] objtemp = GameObject.FindGameObjectsWithTag("BookableObject");

        foreach(GameObject obj in objtemp)
        {
            bookableObjects.Add(obj.transform.parent.gameObject, null);
        }

    }

    /*Books next available bed for the targetee*/
    public GameObject bookBed(GameObject targetee)
    {
        foreach (KeyValuePair<GameObject, GameObject> obj in bookableObjects)
        {
            if (obj.Key.tag == "Bed" && obj.Value == null)
            {
                bookableObjects[obj.Key] = targetee;
                return obj.Key;
            }
        }
        return null;
    }
    

    /* Reserves random chair for the "Reserver", so others wont use the same object at the same time */
    public GameObject bookRandomChair(GameObject reserver)
    {
        List<KeyValuePair<GameObject, GameObject>> temp = new List<KeyValuePair<GameObject, GameObject>>();
        foreach (KeyValuePair<GameObject, GameObject> obj in bookableObjects)
        {
            if( (obj.Key.tag == "Chair2" || obj.Key.tag == "Chair" || obj.Key.tag == "QueueChair") && obj.Value == null )
            {
                temp.Add(obj);
            }
        }
        if (temp.Count > 0)
        {
            GameObject key = temp[Random.Range(0, temp.Count)].Key;
            bookableObjects[key] = reserver;
            return key;
        }
        return null;
    }

    public GameObject bookRandomQueueChair(GameObject reserver)
    {
        List<KeyValuePair<GameObject, GameObject>> temp = new List<KeyValuePair<GameObject, GameObject>>();
        foreach (KeyValuePair<GameObject, GameObject> obj in bookableObjects)
        {
            if ( obj.Key.tag == "QueueChair" && bookableObjects[obj.Key] == null)
            {
                temp.Add(obj);
            }
        }
        if (temp.Count > 0)
        {
            GameObject key = temp[Random.Range(0, temp.Count)].Key;
            bookableObjects[key] = reserver;
            return key;
        }
        return null;
    }

    public GameObject bookRandomPublicToilet(GameObject reserver)
    {
        List<KeyValuePair<GameObject, GameObject>> temp = new List<KeyValuePair<GameObject, GameObject>>();
        foreach (KeyValuePair<GameObject, GameObject> obj in bookableObjects)
        {
            if (obj.Key.tag == "Toilet" && bookableObjects[obj.Key] == null)
            {
                temp.Add(obj);
            }
        }
        if (temp.Count > 0)
        {
            GameObject key = temp[Random.Range(0, temp.Count)].Key;
            bookableObjects[key] = reserver;
            return key;
        }
        return null;
    }


    public bool bookTargetObject(GameObject target, GameObject targetee)
    {
        if(!bookableObjects.ContainsKey(target))
            return false;
        if (bookableObjects[target] == null)
        {
            bookableObjects[target] = targetee;
            return true;
        }
        else
        {
            print("object in use");
            return false;
        }
    }

    public GameObject isObjectBooked(GameObject target)
    {
        if (bookableObjects.ContainsKey(target) && bookableObjects[target] != null)
        {
            return bookableObjects[target];
        }
        else return null;
    }

    public void unbookObject(GameObject go)
    {
        if(go != null)
            if(bookableObjects.ContainsKey(go))
                bookableObjects[go] = null;
    }
}
