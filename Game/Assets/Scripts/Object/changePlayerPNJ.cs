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
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            GameMaster gameMaster = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
            if (gameMaster.getPlayerColor() == Color.white)
            {
                player.GetComponent<Animator>().runtimeAnimatorController = bluePlayerController;
                gameMaster.setPlayerColor(Color.blue);
                FindObjectOfType<PlayerPlateformerController>().PlayChangePlayerSound();
            }
            else 
            {
                player.GetComponent<Animator>().runtimeAnimatorController = whitePlayerController;
                gameMaster.setPlayerColor(Color.white);
                FindObjectOfType<PlayerPlateformerController>().PlayChangePlayerSound();
            }
        }
    }
}

