using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using StarterAssets;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Slider _sensitivitySlider;
    public SavedSettings savedSettings;
    private void Start()
    {
        savedSettings = FindObjectOfType<SavedSettings>();
        SetRotationSpeed();  
    }
    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitApp()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }

    public void SensitivitySlider(float rotationSpeed)
    {
        _sensitivitySlider.maxValue = 20f;
        _sensitivitySlider.minValue = 0.1f;

        float roundedNum = Mathf.Round(rotationSpeed * 10) / 10;

        _sensitivitySlider.value = roundedNum;
    }
    public void SetRotationSpeed()
    {
        savedSettings.sensitivity = _sensitivitySlider.value;
    }
}
