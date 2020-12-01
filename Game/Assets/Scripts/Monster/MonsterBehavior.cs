using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MonsterBehavior : MonoBehaviour
{
    protected Rigidbody2D Ghost;
    protected SpriteRenderer GhostSprite;
    float maxActionRate = 1f;
    float minActionRate = 7f;
    bool doingAction = false;
    bool hasMercy = false;
    protected GameObject target;

    public int LevelsGoneThrough = 3;
    public int coinsCollected = 0;
    private float timerLastAction = 4f;
    private float ghostDistance = 900f;
    // Start is called before the first frame update
    void Start()
    {   
        Ghost = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player");
        GhostSprite = GetComponent<SpriteRenderer>();
        var name = SceneManager.GetActiveScene().name;
        LevelsGoneThrough = int.Parse(name.Substring(name.LastIndexOf("-")+1));
    }

    // Update is called once per frame
    void Update()
    {   
        
        if(!doingAction && !hasMercy && coinsCollected > 0) {
        timerLastAction += Time.deltaTime;
        }

        if(timerLastAction > Mathf.Clamp((minActionRate - coinsCollected), maxActionRate, minActionRate)) {
            timerLastAction = 0;
            doingAction = true;
            int action = Random.Range(0, LevelsGoneThrough+1);
            switch(action) {
                case 0:
                if(LevelsGoneThrough == 0)
                ActionDone();
                else if(LevelsGoneThrough == 1)
                TeleportNearPlayer();
                else
                AttackPlayer();
                break;
                case 1:
                TeleportNearPlayer();
                break;
                default:
                AttackPlayer();
                break;
            }


        }
        facePlayer();  
    }
    void AttackPlayer() {
        Vector2 dir = target.transform.position - Ghost.transform.position;
        dir = dir.normalized;
        Ghost.AddForce(dir * ghostDistance);
        Invoke("ActionDone" , 1f);     
    }

    void TeleportNearPlayer() {
        StartCoroutine(FadeGhost(true));
        Invoke("TeleportGhost" , 1.5f);

    }
    void TeleportGhost() {
        float yDistanceFromTarget = Random.Range(2f, 5f);
        float xDistanceFromTarget = 5f;
        float xLocation = target.transform.position.x +(xDistanceFromTarget*(Random.Range(0,2)*2-1));
        float yLocation = target.transform.position.y + (yDistanceFromTarget*(Random.Range(0,2)*2-1));
        Ghost.transform.position = new Vector2(xLocation, yLocation);
        StartCoroutine(FadeGhost(false));
        ActionDone();
    }

    IEnumerator FadeGhost(bool fadeAway) { 
        if (fadeAway) {
            for (float i = 1; i >= 0; i -= Time.deltaTime) {
                GhostSprite.color = new Color(1, 1, 1, i);
                yield return null;
            }
        }
        else{
            for (float i = 0; i <= 1; i += Time.deltaTime) {
                GhostSprite.color = new Color(1, 1, 1, i);
                yield return null;
            }
        }
    }
    void ActionDone() {
         doingAction = false;
    }
    void NoMoreMercy() {
        hasMercy = false;
    }
    void facePlayer() {

    }
    public void AddCoinsCollected() {
        coinsCollected++;
    }

    public void ResetCoinsCollected() {
        coinsCollected = 0; 
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.tag == "Player" && !hasMercy) {

            FindObjectOfType<LifeCount>().LoseLife();
            if (FindObjectOfType<LifeCount>().livesRemaining != 0) 
                FindObjectOfType<PlayerPlateformerController>().HurtTrigger();
            hasMercy = true;
            TeleportNearPlayer();
            Invoke("NoMoreMercy", 3f);
        }
    }
}
