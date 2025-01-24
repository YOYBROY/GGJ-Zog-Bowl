using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public static bool isPaused = false;
    public GameObject pauseMenuUI;
    public GameObject winMenuUI;
    public GameObject loseMenuUI;

    public TextMeshProUGUI winTimer;
    public TextMeshProUGUI loseTimer;
    public bool gameOver = false;

    // Update is called once per frame
    void Update()
    {
        //Pause Game on ESC
        if (Input.GetKeyDown(KeyCode.Escape) && !gameOver)
        {
            if(isPaused)
            {
                Resume();
            }
            else {  Pause(); }
        }

        //Player Die
        //if (tongue.isDead) 
        //{
        //    loseMenuUI.SetActive(true);
        //    Time.timeScale = 0;
        //    gameOver = true;
        //}
        //
        ////Goal Reached
        //if (tongue.reachedBottle)
        //{
        //    winMenuUI.SetActive(true);
        //    Time.timeScale = 0;
        //}
        
        //Game Over
        if (gameOver)
        {
            gameOver = true;
            Time.timeScale = 0;
        }
    }

    //Resume game
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    //Pause Game
    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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
        SceneManager.LoadScene("MapLayout");
    }

    //Game Won
    public void GameWon(float time)
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    //Game Lost
    public void GameLose(float time)
    {
        loseMenuUI.SetActive(true);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
