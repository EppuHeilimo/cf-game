using UnityEngine;
using System.Collections;
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CombineMesh : MonoBehaviour
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
        //Zero transformation is needed because of localToWorldMatrix transform
        Vector3 position = obj.transform.position;
        obj.transform.position = Vector3.zero;

        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length)
        {
            if (meshFilters[i].sharedMesh != null)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            }
            meshFilters[i].gameObject.SetActive(false);
            i++;
        }
        obj.transform.GetComponent<MeshFilter>().mesh = new Mesh();
        obj.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true, true);
        obj.transform.gameObject.SetActive(true);

        //Reset position
        obj.transform.position = position;

        //Adds collider to mesh
        obj.AddComponent<BoxCollider>();
        combined = true;
    }
}
