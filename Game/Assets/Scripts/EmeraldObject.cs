using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class EmeraldObject : MonoBehaviour
{
    // Start is called before the first frame update

    private void Reset()
    {
        GetComponent<PolygonCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            AudioManager.instance.PlaySFX("emeraldCollect");
            FindObjectOfType<EmeraldCounter>().GainEmerald();
            Destroy(gameObject);           
        }
    }

}
