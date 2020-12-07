using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PolygonCollider2D))]
public class PortalObject : MonoBehaviour
{
    public GameObject portal;
    public Animator animator;
    public bool idleIsPlaying = false;

    void Start()
    {
       GetComponent<AudioSource>().Play();
    }
    private void Reset()
    {
        GetComponent<PolygonCollider2D>().isTrigger = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Invoke("EndOfLevelTransition", 1f);
            GameMaster gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            animator = GetComponent<Animator>();
            animator.SetTrigger("destroyPortal");
            GetComponent<AudioSource>().Stop();
            AudioManager.instance.PlaySFX("portalClose");
            player.GetComponent<SpriteRenderer>().enabled = false;
            player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            GameObject.FindGameObjectWithTag("Ghost").GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        }
    }


    private void EndOfLevelTransition()
    {
        FindObjectOfType<CameraEffect>().StartCoroutinePixelisation();
        Invoke("CloseGame", 3f);
    }

    private void CloseGame()
    {
        animator.SetTrigger("off");
        Destroy(gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
        int NextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (NextIndex == 4)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            SceneManager.LoadScene(NextIndex);
        }
    }
}
