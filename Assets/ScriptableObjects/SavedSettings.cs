using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SavedSettings : ScriptableObject
{
    public float sensitivity;

    public void ResetSensitivity()
    {
        sensitivity = 5f;
    }
}