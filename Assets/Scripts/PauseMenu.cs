using System.Collections.Generic;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public static int totalEnemyCount;
    public static bool isPaused = false;
    public GameObject pauseMenuUI;
    public GameObject settingsMenuUI;
    public GameObject winMenuUI;
    public GameObject loseMenuUI;
    public TMP_Text mouseSensitivityUI;
    public Slider _sensitivitySlider;

    GunController gunController;
    FirstPersonController firstPersonController;

    public TMP_Text finalTimeText;
    public TMP_Text inGameTimerText;
    float timerTime;

    float storedTimeScale;

    public bool gameOver = false;
    public bool beginningOfGame;
    bool stopWatchOn;

    public Image reticleUI;
    public Sprite sodaReticle;
    public Sprite champagneReticle;
    public Sprite emptyReticle;
    public SavedSettings savedSettings;

    [SerializeField] GameObject destroyAllHumansUI;

    private void Start()
    {
        Time.timeScale = 1f;
        Enemy[] enemies = FindObjectsByType<Enemy>(0);
        ZaceyEnemy[] zaceyEnemies = FindObjectsByType<ZaceyEnemy>(0);

        totalEnemyCount = enemies.Length + zaceyEnemies.Length;

        EventSystem.current.SetSelectedGameObject(gameObject);

        savedSettings = FindObjectOfType<SavedSettings>();
        

        gunController = FindObjectOfType<GunController>();
        firstPersonController = FindObjectOfType<FirstPersonController>();
        beginningOfGame = true;
        if (mouseSensitivityUI.gameObject.activeSelf == false) mouseSensitivityUI.gameObject.SetActive(true);
        //Debug.Log("Game Started");
        LoadSensitivity();
        BeginGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (totalEnemyCount == 0) GameWon();
        if (stopWatchOn) timerTime += Time.deltaTime; inGameTimerText.text = timerTime.ToString("#.00");

        if (Input.GetKeyDown(KeyCode.Minus))
        {
            if (firstPersonController.RotationSpeed > 0)
            {
                firstPersonController.RotationSpeed--;
                gunController.storedRotationSpeed = firstPersonController.RotationSpeed;
            }
        }

        if (Input.GetKeyDown(KeyCode.Equals))
        {
            if (firstPersonController.RotationSpeed < 20)
            {
                firstPersonController.RotationSpeed++;
                gunController.storedRotationSpeed = firstPersonController.RotationSpeed;
            }
        }


        //Pause Game on ESC
        if (Input.GetKeyDown(KeyCode.Escape) && !gameOver)
        {
            if (isPaused)
            {
                Resume();
            }
            else { Pause(); }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Restart();
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            QuitGame();
        }
        mouseSensitivityUI.text = "MOUSE SENSITIVITY: " + firstPersonController.RotationSpeed.ToString("#.00"); ;
    }

    public void UpdateReticleUI(string gunType)
    {
        if (gunType == "Soda")
        {
            reticleUI.sprite = sodaReticle;
        }
        else if (gunType == "Champ")
        {
            reticleUI.sprite = champagneReticle;
        }
        else if (gunType == "Null")
        {
            reticleUI.sprite = emptyReticle;
        }
    }

    public void SensitivitySlider(float rotationSpeed)
    {
        _sensitivitySlider.maxValue = 20f;
        _sensitivitySlider.minValue = 0.1f;

        float roundedNum = Mathf.Round(rotationSpeed * 10) / 10;

        _sensitivitySlider.value = roundedNum;
    }

    private void LoadSensitivity()
    {
        _sensitivitySlider.value = savedSettings.sensitivity;
        firstPersonController.RotationSpeed = savedSettings.sensitivity;
    }

    public void SetRotationSpeed()
    {
        firstPersonController.RotationSpeed = _sensitivitySlider.value;
    }
    //Resume game
    public void Resume()
    {
        Time.timeScale = 1;
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);
        mouseSensitivityUI.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPaused = false;
    }

    //Pause Game
    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        mouseSensitivityUI.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isPaused = true;
        Time.timeScale = 0;
    }

    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    //Quit Game
    public void QuitGame()
    {
        Application.Quit();
    }

    //Restart Game
    public void Restart()
    {
        Time.timeScale = 1;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    //Game Won
    public void GameWon()
    {
        gameOver = true;
        isPaused = true;
        stopWatchOn = false;
        finalTimeText.gameObject.SetActive(true);
        inGameTimerText.gameObject.SetActive(false);
        finalTimeText.text = "Total Time: " + timerTime.ToString("#.00"); ;
        winMenuUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
    }

    //Game Lost
    public void GameLose()
    {
        gameOver = true;
        isPaused = true;
        stopWatchOn = false;
        finalTimeText.gameObject.SetActive(true);
        inGameTimerText.gameObject.SetActive(false);
        finalTimeText.text = "Total Time: " + timerTime.ToString("#.00"); ;
        loseMenuUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
    }

    public void BeginGame()
    {
        inGameTimerText.gameObject.SetActive(true);
        firstPersonController.enabled = true;
        beginningOfGame = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        mouseSensitivityUI.gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    public void StartStopWatch()
    {
        stopWatchOn = true;
        destroyAllHumansUI.SetActive(true);
    }
}
