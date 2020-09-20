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
        if (livesRemaining==0)
        {
            return;
        }
        livesRemaining--;
        lives[livesRemaining].gameObject.SetActive(false);

        if (livesRemaining==0)
        {
            Restart();
        }
    }
}
