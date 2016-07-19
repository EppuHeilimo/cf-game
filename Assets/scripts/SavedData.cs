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
            // Android
            string oriPath = System.IO.Path.Combine(Application.streamingAssetsPath, "Player.json");

            // Android only use WWW to read file
            WWW reader = new WWW(oriPath);
            while (!reader.isDone) { }

            string realPath = Application.persistentDataPath + "/db";
            System.IO.File.WriteAllBytes(realPath, reader.bytes);

            path = realPath;
        }
        else
        {
            path = Application.dataPath + "/StreamingAssets/Player.json";
        }

        if(!File.Exists(path))
        {
            fixFile(path);
        }

        profilesData = JsonMapper.ToObject(File.ReadAllText(path));


        if (profilesData.Count == 0)
        {
            fixFile(path);
        }

        for (int i = 0; i < profilesData.Count; i++)
        {
            profiles.Add(new Profile(profilesData[i]["name"].ToString(), profilesData[i]["head"].ToString(), (int)profilesData[i]["gender"], (int)(profilesData[i]["highscore"])));
        }
        selectedProfile = profiles[0];

    }

    void fixFile(string path)
    {
        Profile temp = new Profile("Default", "head_f_1", 0, 0);
        profiles.Add(temp);
        FileStream s = File.Create(path);
        s.Close();
        saveProfile();
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
            if (i != profiles.Count - 1)
            {
                jsontext += ",";
            }
            jsontext += JsonMapper.ToJson(profiles[i]);
        }
        jsontext += "]";

        string path = "";
        if (Application.platform == RuntimePlatform.Android)
        {
            // Android
            string oriPath = System.IO.Path.Combine(Application.streamingAssetsPath, "Player.json");

            // Android only use WWW to read file
            WWW reader = new WWW(oriPath);
            while (!reader.isDone) { }

            string realPath = Application.persistentDataPath + "/db";
            System.IO.File.WriteAllBytes(realPath, reader.bytes);

            path = oriPath;//realPath;
        }
        else
        {
            path = Application.dataPath + "/StreamingAssets/Player.json";
        }

        File.WriteAllText(path, jsontext);
    }

}
