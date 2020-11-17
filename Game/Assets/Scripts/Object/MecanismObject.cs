using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[RequireComponent(typeof(TilemapCollider2D))]
public class MecanismObject : MonoBehaviour
{
    [SerializeField] Tilemap SecretRoom;
    [SerializeField] Tilemap SecretRoomBackGround;
    [SerializeField] Tilemap SecretRoomWall;
    [SerializeField] Tilemap SecretRoomWallTransparent;
    [SerializeField] Tilemap changePlayerPNJ;
    private static bool isActive = false;
    private void Reset()
    {
        GetComponent<TilemapCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && isActive == false)
        {
            SecretRoomWall.GetComponent<TilemapCollider2D>().enabled = false;
            SecretRoomWall.GetComponent<TilemapRenderer>().enabled = false;
            SecretRoom.GetComponent<TilemapRenderer>().enabled = true;
            changePlayerPNJ.GetComponent<TilemapRenderer>().enabled = true;
            SecretRoomBackGround.GetComponent<TilemapRenderer>().enabled = true;
            SecretRoomWallTransparent.GetComponent<TilemapRenderer>().enabled = true;
            FindObjectOfType<PlayerPlateformerController>().PlayDiscoveringEasterEggSound();
            isActive = true;
        }
    }
}

