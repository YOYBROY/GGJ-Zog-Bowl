using StarterAssets;
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

    FirstPersonController firstPersonController;
    float storedRotationSpeed;

    public bool gameOver = false;

    private void Start()
    {
        firstPersonController = FindObjectOfType<FirstPersonController>();
    }

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
        Time.timeScale = 1;
        pauseMenuUI.SetActive(false);
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    //Pause Game
    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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
        SceneManager.LoadScene("MapLayout");
    }

    //Game Won
    public void GameWon(float time)
    {
        pauseMenuUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
    }

    //Game Lost
    public void GameLose(float time)
    {
        loseMenuUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
    }
}
