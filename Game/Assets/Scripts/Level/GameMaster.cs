using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    private static GameMaster instance;
    public Vector2 lastCheckPointPos;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
            lastCheckPointPos = GameObject.FindGameObjectWithTag("startPosition").transform.position;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
