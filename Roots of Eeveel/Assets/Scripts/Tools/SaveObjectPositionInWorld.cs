using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class SaveObjectPositionInWorld : MonoBehaviour
{
    public bool runSave;
    public bool runLoad;
    public string filename;


    private void Update()
    {
        if (runSave)
        {
            Save();
        }
        if (runLoad)
        {
            Load();
        }
    }

    void Save()
    {
        Saved save1 = new Saved();
        save1.children = gameObject.GetComponentsInChildren<Transform>();
        string save2 = JsonUtility.ToJson(save1);
        File.WriteAllText(filename, save2);
    }

    void Load()
    {
        Transform[] children = gameObject.GetComponentsInChildren<Transform>();
        string save1 = File.ReadAllText(filename);
        Saved save2 = JsonUtility.FromJson<Saved>(save1);
        Transform[] savedChildren = save2.children;

        for (int i = 0; i < children.Length; i++)
        {
            children[i].position = savedChildren[i].position;
            children[i].rotation = savedChildren[i].rotation;
        }
    }
}

[Serializable]
public class Saved
{
    public Transform[] children;
}
