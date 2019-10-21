using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectReposition : MonoBehaviour
{
    public List<Transform> destinations;
    public List<Transform> newObject;

    [ContextMenu("Reposition")]
    public void Reposition()
    {
        for (int i = 0; i < destinations.Count; i++)
        {
            newObject[i].position = destinations[i].position;
        }
    }
}
