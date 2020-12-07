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
        //************************************
    }
    public void GainEmerald(Text emeraldText)
    {
        myEmerald++;
        FindObjectOfType<MonsterBehavior>().AddCoinsCollected();
        UpdateUI(emeraldText);
        if (myEmerald == emeraldNeeded)
        {
            AudioManager.instance.PlaySFX("portalOpen");
            Instantiate(portalObject, portalPosition);
        }       
    }
    private void UpdateUI(Text emeraldText)
    {
        emeraldText.text = myEmerald.ToString() + "/" + emeraldNeeded.ToString();
    }
}
