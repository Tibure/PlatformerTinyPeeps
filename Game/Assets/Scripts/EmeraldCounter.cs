using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmeraldCounter : MonoBehaviour
{
    public int myEmerald;
    public int emeraldNeeded;
    public Transform portalPosition;
    public GameObject portalObject;
    private void Start()
    {
        //temporaire pour faire afficher le portal sans avoir de script donne de lvl
        myEmerald = 0;
        emeraldNeeded = 7;
        portalPosition.position.Set(13, 2, 0);
        //************************************
    }
    public void GainEmerald()
    {
        myEmerald++;
        if (myEmerald == emeraldNeeded)
        {
            AudioManager.instance.PlaySFX("portalOpen");
            Instantiate(portalObject, portalPosition);
        }       
    }
}
