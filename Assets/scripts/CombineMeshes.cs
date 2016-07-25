using UnityEngine;
using System.Collections;
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

/*
 * Combines children's meshes to one mesh.
 * Give this component to parent and set texture
 * removes all the old meshes
 * 
 * Gives significant performance boost.
 * 
 * DON'T USE IF MESHES REQUIRE INDIVIDUAL FUNCTIONS LIKE TARGETING
 * 
 */

public class CombineMeshes : MonoBehaviour
{

    bool combined = false;

    // Update is called once per frame
    void Update()
    {
        if (!combined)
        {
            combineMeshes(gameObject);
        }
    }

    void combineMeshes(GameObject obj)
    {
        //Save transformation information
        Vector3 position = obj.transform.position;
        obj.transform.position = Vector3.zero;
        Quaternion rotation = obj.transform.rotation;
        obj.transform.rotation = Quaternion.identity;

        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length)
        {
            if (meshFilters[i].sharedMesh != null )
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            }
            meshFilters[i].gameObject.SetActive(false);
            if (meshFilters[i].gameObject.transform.childCount == 0)
            {
                Destroy(meshFilters[i].gameObject);
            }
            i++;
        }
        obj.transform.GetComponent<MeshFilter>().mesh = new Mesh();
        obj.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true, true);
        obj.transform.gameObject.SetActive(true);

        //Reset transform
        obj.transform.position = position;
        obj.transform.rotation = rotation;

        //Add collider to mesh
        obj.AddComponent<BoxCollider>();
        combined = true;
    }
}
