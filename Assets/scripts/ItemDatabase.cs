using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;
using System.IO;
using System;

public class ItemDatabase : MonoBehaviour
{
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
            database.Add(new Item((int)itemData[i]["id"], itemData[i]["title"].ToString(), itemData[i]["desc"].ToString(), itemData[i]["substance"].ToString(), itemData[i]["usage"].ToString()));
        }
    }
}

public class Item
{
    public int ID { get; set; }
    public string Title { get; set; }
    public string Desc { get; set; }
    public string Substance { get; set; }
    public Sprite Sprite { get; set; }
    public string Usage { get; set; }

    public Item(int id, string title, string desc, string substance, string usage)
    {
        this.ID = id;
        this.Title = title;
        this.Desc = desc;
        this.Substance = substance;
        this.Sprite = Resources.Load<Sprite>("Sprites/Items/" + title); // name the sprites same as titles!
        if (Sprite == null)
        {
            this.Sprite = Resources.Load<Sprite>("Sprites/Items/null");
        }
        this.Usage = usage;
    }

    public Item()
    {
        this.ID = -1;
    }
}