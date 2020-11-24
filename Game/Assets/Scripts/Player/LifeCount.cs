using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeCount : LevelManager
{
    public Image[] lives;
    public int livesRemaining;

    public void LoseLife()
    {
        if (livesRemaining == 0)
        {
            return;
        }
        livesRemaining--;
        lives[livesRemaining].gameObject.SetActive(false);

        if (livesRemaining == 0)
        {
            FindObjectOfType<PlayerPlateformerController>().PlayDeathSound();
            FindObjectOfType<PlayerPlateformerController>().canMove = false;
            FindObjectOfType<PlayerPlateformerController>().targetVelocity = Vector2.zero;
            gameObject.GetComponent<Animator>().SetTrigger("DeathTrigger");
            Invoke("DisabledPlayerRenderer", 0.80f);
        }
    }
    public void RestartTheGame()
    {
        Restart();
    }
    public void DisabledPlayerRenderer()
    {
        Invoke("RestartTheGame", 0.80f);
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }
}
