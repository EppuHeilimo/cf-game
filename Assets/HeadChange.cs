using UnityEngine;
using System.Collections;

public class HeadChange : MonoBehaviour {
    public GameObject[] heads = new GameObject[30];
    // Use this for initialization
    void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ChangeToRandomHead()
    {
        int rand = UnityEngine.Random.Range(0, 29);
        Transform iiro = transform.Find("Iiro");
        Transform root = iiro.Find("Root");
        Transform torso = root.Find("Torso");
        Transform chest = torso.Find("Chest");
        Transform Head = chest.Find("Head");
        Transform head = Head.Find("iiro_head");
        MeshFilter mesh = head.GetComponent<MeshFilter>();
        MeshRenderer material = head.GetComponent<MeshRenderer>();
        mesh.sharedMesh = heads[rand].GetComponent<MeshFilter>().sharedMesh;
        material.sharedMaterial = heads[rand].GetComponent<MeshRenderer>().sharedMaterial;
    }

}
