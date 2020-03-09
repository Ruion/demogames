using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class EssentialSpawner : SerializedMonoBehaviour
{
    [SerializeField]
    private Dictionary<string, GameObject> essentialComponents = new Dictionary<string, GameObject>();

    private void Awake()
    {
        foreach (KeyValuePair<string, GameObject> entry in essentialComponents)
        {
            // do something with entry.Value or entry.Key
            if (GameObject.FindGameObjectWithTag(entry.Key) == null)
                Instantiate(entry.Value);
        }
    }
}