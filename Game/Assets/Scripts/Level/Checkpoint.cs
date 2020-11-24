using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.WSA;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class Checkpoint : MonoBehaviour
{
    private GameMaster gm;
    private GameObject player;
    private GameObject[] checkpoints;
    public Sprite RedSprite;
    public Sprite GoldSprite;
    public RuntimeAnimatorController RedAnimation;
    public RuntimeAnimatorController GoldAnimation;
    public AudioClip sfx_checkpoint;
    public Text checkpointText;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
        checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
        checkpointText.text = "desactivated";
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player.GetComponent<AudioSource>().loop = false;
            player.GetComponent<AudioSource>().PlayOneShot(sfx_checkpoint);
            gm.lastCheckPointPos = transform.position;
            UpdateCheckpoint();
        }
    }

    private void UpdateCheckpoint()
    {
        for (int i = 0; i < checkpoints.Length; i++)
        {
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
        if (new Vector3(gm.lastCheckPointPos.x, gm.lastCheckPointPos.y, 0) != GameObject.FindGameObjectWithTag("startPosition").transform.position)
        {
            checkpointText.text = "activated";
        }
    }
}
