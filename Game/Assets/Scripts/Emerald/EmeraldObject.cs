using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PolygonCollider2D))]
public class EmeraldObject : MonoBehaviour
{
    // Start is called before the first frame update
    public Text emeraldText;
    private void Reset()
    {
        GetComponent<PolygonCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            AudioManager.instance.PlaySFX("emeraldCollect");
            FindObjectOfType<EmeraldCounter>().GainEmerald(emeraldText);
            Destroy(gameObject);           
        }
    }

}
