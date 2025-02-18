using System.Collections.Generic;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public int totalEnemyCount;
    public static bool isPaused = false;
    public GameObject pauseMenuUI;
    public GameObject winMenuUI;
    public GameObject loseMenuUI;
    public TMP_Text mouseSensitivityUI;
    public GameObject BeginGameUI;

    GunController gunController;
    FirstPersonController firstPersonController;

    public TMP_Text finalTimeText;
    public TMP_Text inGameTimerText;
    float timerTime;

    public bool gameOver = false;
    public bool beginningOfGame;
    bool stopWatchOn;

    private void Start()
    {
        Time.timeScale = 1f;
        Enemy[] enemies = FindObjectsByType<Enemy>(0);
        ZaceyEnemy[] zaceyEnemies = FindObjectsByType<ZaceyEnemy>(0);

        totalEnemyCount += enemies.Length + zaceyEnemies.Length;

        gunController = FindObjectOfType<GunController>();
        firstPersonController = FindObjectOfType<FirstPersonController>();
        firstPersonController.enabled = false;
        beginningOfGame = true;
        if (BeginGameUI.activeSelf == false) BeginGameUI.SetActive(true);
        if(mouseSensitivityUI.gameObject.activeSelf == false) mouseSensitivityUI.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (totalEnemyCount == 0) GameWon();
        if (stopWatchOn) timerTime += Time.deltaTime; inGameTimerText.text = "Time: " + timerTime.ToString("#.000"); ;
        if (beginningOfGame)
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Escape))
            {
                BeginGame();
            }

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
        mouseSensitivityUI.text = "Mouse Sensitivity: " + firstPersonController.RotationSpeed;
    }

    //Resume game
    public void Resume()
    {
        Time.timeScale = 1;
        pauseMenuUI.SetActive(false);
        mouseSensitivityUI.gameObject.SetActive(false);
        isPaused = false;
    }

    //Pause Game
    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        mouseSensitivityUI.gameObject.SetActive(true);
        isPaused = true;
        Time.timeScale = 0;
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //Game Won
    public void GameWon()
    {
        gameOver = true;
        isPaused = true;
        stopWatchOn = false;
        finalTimeText.gameObject.SetActive(true);
        inGameTimerText.gameObject.SetActive(false);
        finalTimeText.text = "Total Time: " + timerTime.ToString("#.000"); ;
        winMenuUI.SetActive(true);
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
        finalTimeText.text = "Total Time: " + timerTime.ToString("#.000"); ;
        loseMenuUI.SetActive(true);
        Time.timeScale = 0;
    }

    public void BeginGame()
    {
        stopWatchOn = true; 
        inGameTimerText.gameObject.SetActive(true);
        firstPersonController.enabled = true;
        beginningOfGame = false;
        BeginGameUI.SetActive(false);
        mouseSensitivityUI.gameObject.SetActive(false);
        Time.timeScale = 1;
    }
}
