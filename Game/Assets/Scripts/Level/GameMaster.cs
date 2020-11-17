using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    private static GameMaster instance;
    private static Color playerColor;
    public Vector2 lastCheckPointPos;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            playerColor = Color.white;
            DontDestroyOnLoad(instance);
            lastCheckPointPos = GameObject.FindGameObjectWithTag("startPosition").transform.position;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public Color getPlayerColor()
    {
        return playerColor;
    }
    public void setPlayerColor(Color newColor)
    {
        playerColor = newColor;
    }
}
