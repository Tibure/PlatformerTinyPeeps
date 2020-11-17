using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[RequireComponent(typeof(TilemapCollider2D))]
public class changePlayerPNJ : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] RuntimeAnimatorController whitePlayerController;
    [SerializeField] RuntimeAnimatorController bluePlayerController;

    private void Reset()
    {
        GetComponent<TilemapCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            GameMaster gameMaster = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
            if (gameMaster.getPlayerColorIndex() == 1)
            {
                player.GetComponent<Animator>().runtimeAnimatorController = bluePlayerController;
                gameMaster.setPlayerColorIndex(2);
            }
            else 
            {
                player.GetComponent<Animator>().runtimeAnimatorController = whitePlayerController;
                gameMaster.setPlayerColorIndex(1);
            }
        }
    }
}

