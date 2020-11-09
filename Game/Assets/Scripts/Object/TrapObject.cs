using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[RequireComponent(typeof(TilemapCollider2D))]
public class TrapObject : MonoBehaviour
{
    private void Reset()
    {
        GetComponent<TilemapCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            FindObjectOfType<LifeCount>().LoseLife();
            FindObjectOfType<PlayerPlateformerController>().HurtTrigger();
        }
    }
}
