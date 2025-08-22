using UnityEngine;
using TMPro;

public class RobotsActiveTextDisplay : MonoBehaviour
{
    [SerializeField] TextMeshPro textMeshPro;

    Animator animator;
    private int enemyCountDisplayed;


    void Start()
    {
        animator = GetComponent<Animator>();
        enemyCountDisplayed = PauseMenu.totalEnemyCount;
        UpdateText();
    }

    void Update()
    {
        if(enemyCountDisplayed != PauseMenu.totalEnemyCount)
        {
            PlayAnimation();
            enemyCountDisplayed = PauseMenu.totalEnemyCount;
        }
    }

    void PlayAnimation()
    {
        animator.SetTrigger("Kill");
    }

    void UpdateText()
    {
        textMeshPro.text = PauseMenu.totalEnemyCount.ToString();
    }
}