using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class Checkpoint : MonoBehaviour
{
    private GameMaster gm;
    private GameObject[] checkpoints;
    public Sprite RedSprite;
    public Sprite GoldSprite;
    public RuntimeAnimatorController RedAnimation;
    public RuntimeAnimatorController GoldAnimation;
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
        checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            gm.lastCheckPointPos = transform.position;
            UpdateCheckpoint();
        }
    }

    private void UpdateCheckpoint()
    {
        for (int i = 0; i < checkpoints.Length; i++)
        {
            print("boucle");
            if (checkpoints[i].transform.position != new Vector3(gm.lastCheckPointPos.x, gm.lastCheckPointPos.y, 0))
            {
                checkpoints[i].GetComponent<Animator>().runtimeAnimatorController = RedAnimation;
                checkpoints[i].GetComponent<SpriteRenderer>().sprite = RedSprite;
            }
            else 
            {
                checkpoints[i].GetComponent<Animator>().runtimeAnimatorController = GoldAnimation;
                checkpoints[i].GetComponent<SpriteRenderer>().sprite = GoldSprite;
            }
        }
    }
}
