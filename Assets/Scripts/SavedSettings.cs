using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavedSettings : MonoBehaviour
{
    public float sensitivity;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

}
