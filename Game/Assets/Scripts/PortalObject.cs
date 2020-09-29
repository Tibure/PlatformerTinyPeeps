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
            animator = GetComponent<Animator>();
            animator.SetTrigger("destroyPortal");
            GetComponent<AudioSource>().Stop();
            AudioManager.instance.PlaySFX("portalClose");
            Debug.Log(animator.GetCurrentAnimatorStateInfo(0).length);
            animator.SetTrigger("off");
            Destroy(gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
            SceneManager.LoadScene("Level-1");
        }
    }


}
