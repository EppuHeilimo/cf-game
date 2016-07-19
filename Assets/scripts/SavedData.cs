using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using System.IO;

public class Profile
{
    public string name;
    public string head;
    public int gender;
    public int highscore;

    public Profile(string name, string head, int gender, int highscore)
    {
        this.name = name;
        this.head = head;
        this.gender = gender;
        this.highscore = highscore;
    }
}

public class SavedData : MonoBehaviour {
    
    JsonData profilesData;
    List<Profile> profiles = new List<Profile>();

    Profile selectedProfile;
    
	// Use this for initialization
	void Start () {
        string path = "";
        if (Application.platform == RuntimePlatform.Android)
        {
            path = Application.persistentDataPath + "/Player.json";
        }
        else
        {
            path = Application.persistentDataPath + "/Player.json";
        }

        if (!File.Exists(path))
        {
            File.WriteAllText(path, "");
            profiles.Add(new Profile("Default", "head_f_1", 0, 0));
            saveProfile();
        }

        profilesData = JsonMapper.ToObject(File.ReadAllText(path));


        for (int i = 0; i < profilesData.Count; i++)
        {
            profiles.Add(new Profile(profilesData[i]["name"].ToString(), profilesData[i]["head"].ToString(), (int)profilesData[i]["gender"], (int)(profilesData[i]["highscore"])));
        }
        selectedProfile = profiles[0];


    }


    public Profile getProfile()
    {
        return selectedProfile;
    }

    public void saveProfile()
    {
        
        string jsontext = "[";
        for (int i = 0; i < profiles.Count; i++)
        {
            if (i != 0)
                jsontext += ",";
            jsontext += JsonMapper.ToJson(profiles[i]);
        }
        jsontext += "]";

        string path = "";
        
        if (Application.platform == RuntimePlatform.Android)
        {
            string realPath = Application.persistentDataPath + "/Player.json";
            path = realPath;
        }
        else
        {
            path = Application.persistentDataPath + "/Player.json";
        }
        File.WriteAllText(path, "");
        File.WriteAllText(path, jsontext);
    }

}
