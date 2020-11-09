using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public void Restart()
    {
        GameMaster gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
        gm.lastCheckPointPos = GameObject.FindGameObjectWithTag("startPosition").transform.position;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
