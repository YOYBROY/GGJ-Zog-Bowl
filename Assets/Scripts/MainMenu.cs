using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Slider _sensitivitySlider;
    public SavedSettings savedSettings;
    public TMP_Text mouseSensitivityUI;

    private void Start()
    {
        UpdateSensitivitySlider();  
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

    public void SetRotationSpeed()
    {
        savedSettings.sensitivity = _sensitivitySlider.value;
        if(mouseSensitivityUI != null)
        {
            mouseSensitivityUI.text = "MOUSE SENSITIVITY: " + _sensitivitySlider.value.ToString("#.00");
        }
    }

    private void UpdateSensitivitySlider()
    {
        _sensitivitySlider.value = savedSettings.sensitivity;
        if (mouseSensitivityUI != null)
        {
            mouseSensitivityUI.text = "MOUSE SENSITIVITY: " + _sensitivitySlider.value.ToString("#.00");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            savedSettings.ResetSensitivity();
            UpdateSensitivitySlider();
        }
    }
}
