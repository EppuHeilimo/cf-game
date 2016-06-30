using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;
using System.IO;
using System;

public class ItemDatabase : MonoBehaviour {
    public List<Item> database = new List<Item>();
    JsonData itemData;

    void Start()
    {
        string path = "";
        if (Application.platform == RuntimePlatform.Android)
        {  
            // Android
            string oriPath = System.IO.Path.Combine(Application.streamingAssetsPath, "Items.json");

            // Android only use WWW to read file
            WWW reader = new WWW(oriPath);
            while (!reader.isDone) { }

            string realPath = Application.persistentDataPath + "/db";
            System.IO.File.WriteAllBytes(realPath, reader.bytes);

            path = realPath;
        }
        else
        {
            path = Application.dataPath + "/StreamingAssets/Items.json";
        }
        itemData = JsonMapper.ToObject(File.ReadAllText(path));
        ConstructItemDatabase();
    }

    public Item FetchItemByID(int id)
    {
        for (int i = 0; i < database.Count; i++)
            if (database[i].ID == id)
                return database[i];
        return null;
    }

    public Item FetchItemByTitle(string title)
    {
        for (int i = 0; i < database.Count; i++)
            if (string.Equals(database[i].Title, title, StringComparison.CurrentCultureIgnoreCase))
                return database[i];
        return null;
    }

    void ConstructItemDatabase()
    {
        for (int i = 0; i < itemData.Count; i++)
        {
            database.Add(new Item((int)itemData[i]["id"], itemData[i]["title"].ToString(), itemData[i]["desc"].ToString(), itemData[i]["usage"].ToString(),
                                  (int)itemData[i]["dosages"]["small"], (int)itemData[i]["dosages"]["medium"], (int)itemData[i]["dosages"]["high"], (int)itemData[i]["defaultDos"]));
        }
    }
}



public class Item : ICloneable
{
    public int ID { get; set; }
    public string Title { get; set; }
    public string Desc { get; set; }
    public Sprite Sprite { get; set; }
    public string Usage { get; set; }
    public int SmallDosage { get; set; }
    public int MediumDosage { get; set; }
    public int HighDosage { get; set; }
    public int DefaultDosage { get; set; }
    public int timesPerDay { get; set; }
    public int currentDosage { get; set; }
    public Item(int id, string title, string desc, string usage, int smallDosage, int mediumDosage, int highDosage, int defaultDosage, int timesPerDay)
    {
        this.ID = id;
        this.Title = title;
        this.Desc = desc;
        this.Sprite = Resources.Load<Sprite>("Sprites/Items/" + title); // name the sprites same as titles!
        if (this.Sprite == null)
        {
            this.Sprite = Resources.Load<Sprite>("Sprites/Items/null");
        }
        this.Usage = usage;
        this.SmallDosage = smallDosage;
        this.MediumDosage = mediumDosage;
        this.HighDosage = highDosage;
        this.DefaultDosage = defaultDosage;
        this.timesPerDay = timesPerDay;
    }

    public Item(int id, string title, string desc, string usage, int smallDosage, int mediumDosage, int highDosage, int defaultDosage)
    {
        this.ID = id;
        this.Title = title;
        this.Desc = desc;
        this.Sprite = Resources.Load<Sprite>("Sprites/Items/" + title); // name the sprites same as titles!
        if (this.Sprite == null)
        {
            this.Sprite = Resources.Load<Sprite>("Sprites/Items/null");
        }
        this.Usage = usage;
        this.SmallDosage = smallDosage;
        this.MediumDosage = mediumDosage;
        this.HighDosage = highDosage;
        this.DefaultDosage = defaultDosage;
        this.timesPerDay = 0;
    }

    public Item()
    {
        this.ID = -1;
    }

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}
