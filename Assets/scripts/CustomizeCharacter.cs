using UnityEngine;
using System.Collections;

using UnityEngine.UI;

public class CustomizeCharacter : MonoBehaviour {

    float sensitivity = 0.4f;
    Vector3 rotation = new Vector3(0, 0, 0);
    Vector3 mouseReference;
    Vector2 touchReference;
    Vector3 mouseOffset;
    bool isRotating = false;
    Touch touch;
    Transform modeltransform;
    Heads heads;
    SavedData savedData;

    GameObject warning;
    bool warningenabled = false;
    float timer = 0;
    float alphatimer;

    GameObject head;

    Profile currentProfile;

    //0 = female, 1 = male
    int gender = 0;
    int currentheadid = 0;

    // Use this for initialization
    void Start() {

        modeltransform = transform.FindChild("3D").FindChild("Model");
        head = modeltransform.FindChild("iiro_head").gameObject;

        savedData = GameObject.Find("SavedData").GetComponent<SavedData>();
        heads = GameObject.Find("Heads").GetComponent<Heads>();

        currentProfile = savedData.getProfile();

        head.GetComponent<MeshFilter>().sharedMesh = heads.getPlayersHead().GetComponent<MeshFilter>().sharedMesh;
        head.GetComponent<MeshRenderer>().sharedMaterial = heads.getPlayersHead().GetComponent<MeshRenderer>().sharedMaterial;
        transform.FindChild("NameInputField").FindChild("Text").GetComponent<Text>().text = currentProfile.name;
        gender = currentProfile.gender;

        transform.FindChild("NameInputField").GetComponent<InputField>().text = currentProfile.name;

        warning = transform.FindChild("Warning").gameObject;
        warning.SetActive(false);

    }

    public void SaveButton()
    {
        if(!string.IsNullOrEmpty(transform.FindChild("NameInputField").FindChild("Text").GetComponent<Text>().text))
        {
            GameObject.Find("Menu").GetComponent<ShowPanels>().HideCustomizePanel();

            currentProfile.name = transform.FindChild("NameInputField").FindChild("Text").GetComponent<Text>().text;
            currentProfile.gender = gender;
            currentProfile.head = head.GetComponent<MeshRenderer>().sharedMaterial.name;
            print("Saved: " + currentProfile.name + currentProfile.head + currentProfile.gender + currentProfile.highscore);
            heads.PlayersHead = head;

            savedData.saveProfile();
        }
        else
        {
            timer = 0;
            alphatimer = 0;
            warning.SetActive(true);
            warning.GetComponent<Text>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            warningenabled = true;
        }
    }

    public void CancelButton()
    {
        GameObject.Find("Menu").GetComponent<ShowPanels>().HideCustomizePanel();
    }

    public void MaleButton()
    {
        gender = 1;
        head.GetComponent<MeshFilter>().sharedMesh = heads.headsMale[0].GetComponent<MeshFilter>().sharedMesh;
        head.GetComponent<MeshRenderer>().sharedMaterial = heads.headsMale[0].GetComponent<MeshRenderer>().sharedMaterial;
    }

    public void FemaleButton()
    {
        gender = 0;
        head.GetComponent<MeshFilter>().sharedMesh = heads.headsFemale[0].GetComponent<MeshFilter>().sharedMesh;
        head.GetComponent<MeshRenderer>().sharedMaterial = heads.headsFemale[0].GetComponent<MeshRenderer>().sharedMaterial;
        print(heads.headsFemale[0].GetComponent<MeshRenderer>().sharedMaterial.name);
    }

    public void nextHead()
    {
        if(gender == 1)
        {
            if(heads.headsMale.Count > currentheadid + 1)
            {
                currentheadid++;
            }
            else
            {
                currentheadid = 0;
            }
            head.GetComponent<MeshFilter>().sharedMesh = heads.headsMale[currentheadid].GetComponent<MeshFilter>().sharedMesh;
            head.GetComponent<MeshRenderer>().sharedMaterial = heads.headsMale[currentheadid].GetComponent<MeshRenderer>().sharedMaterial;
        }
        else if (gender == 0)
        {
            if (heads.headsFemale.Count > currentheadid + 1)
            {
                currentheadid++;
            }
            else
            {
                currentheadid = 0;
            }
            head.GetComponent<MeshFilter>().sharedMesh = heads.headsFemale[currentheadid].GetComponent<MeshFilter>().sharedMesh;
            head.GetComponent<MeshRenderer>().sharedMaterial = heads.headsFemale[currentheadid].GetComponent<MeshRenderer>().sharedMaterial;
        }

    }

    public void prevHead()
    {
        if (gender == 1)
        {
            if (currentheadid == 0)
            {
                currentheadid = heads.headsMale.Count - 1;
            }
            else
            {
                currentheadid--;
            }
            head.GetComponent<MeshFilter>().sharedMesh = heads.headsMale[currentheadid].GetComponent<MeshFilter>().sharedMesh;
            head.GetComponent<MeshRenderer>().sharedMaterial = heads.headsMale[currentheadid].GetComponent<MeshRenderer>().sharedMaterial;
        }
        else if (gender == 0)
        {
            if (currentheadid == 0)
            {
                currentheadid = heads.headsFemale.Count - 1;
            }
            else
            {
                currentheadid--;
            }
            head.GetComponent<MeshFilter>().sharedMesh = heads.headsFemale[currentheadid].GetComponent<MeshFilter>().sharedMesh;
            head.GetComponent<MeshRenderer>().sharedMaterial = heads.headsFemale[currentheadid].GetComponent<MeshRenderer>().sharedMaterial;
        }
    }



    // Update is called once per frame
    void Update() {

        if(warningenabled)
        {
            timer += Time.deltaTime;
            if(timer > 1.5f)
            {
                alphatimer += Time.deltaTime;
                warning.GetComponent<Text>().color = new Color(1.0f, 1.0f, 1.0f, warning.GetComponent<Text>().color.a - alphatimer);
                if(warning.GetComponent<Text>().color.a < 0.01f)
                {
                    warningenabled = false;
                    warning.SetActive(false);
                }
            }
            else
            {
                warning.GetComponent<Text>().color = new Color(1.0f, 1.0f, 1.0f, Mathf.Abs(Mathf.Sin(timer * 1.5f)));
            }
        }


#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        if (Input.GetMouseButtonDown(0))
        {
            LayerMask layerMask = 1 << 15;
            Ray ray = new Ray();
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //for touch device
            if (Physics.Raycast(ray, 500.0f, layerMask))
            {
                isRotating = true;
                mouseReference = Input.mousePosition;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isRotating = false;
        }

        if (isRotating)
        {
            mouseOffset = (Input.mousePosition - mouseReference);
            rotation.z = (mouseOffset.x) * sensitivity;
            modeltransform.Rotate(rotation);
            mouseReference = Input.mousePosition;
        }





        //Touch device
#elif (UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8)
        if (Input.touchCount == 1)
        {
            touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                LayerMask layerMask = 1 << 15;
                Ray ray = new Ray();
                ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                if (Physics.Raycast(ray, 500.0f, layerMask))
                {
                    isRotating = true;
                    touchReference = touch.position;
                }
            }
            else if (isRotating && touch.phase == TouchPhase.Moved)
            {
                mouseOffset = (touch.position - touchReference);
                rotation.z = mouseOffset.x * sensitivity;
                modeltransform.Rotate(rotation);
                touchReference = touch.position;
            }
            else if(isRotating && touch.phase == TouchPhase.Ended)
            {
                isRotating = false;
            }
        }
#endif

    }

}
