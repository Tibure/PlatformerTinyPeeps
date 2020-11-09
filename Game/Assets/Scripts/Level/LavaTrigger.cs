using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LavaTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RespawnToCheckPoint();
        }
    }

    private void RespawnToCheckPoint()
    {
        GameMaster gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = gm.lastCheckPointPos;
        FindObjectOfType<LifeCount>().LoseLife();
        FindObjectOfType<PlayerPlateformerController>().HurtTrigger();
    }
}
