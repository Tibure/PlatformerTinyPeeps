using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MonsterBehavior : MonoBehaviour
{
    Rigidbody2D rb2d;
    float maxActionRate = 1;
    float minActionRate = 7;
    bool DoingAction = false;
    public GameObject target;
    public float coinsCollected = 0;
    private float timerLastAction = 0;
    private float ghostSpeed = 5f;

    private float ghostDistance = 100f;
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponentInParent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {   
        
        /* if(!DoingAction)
        timerLastAction += Time.deltaTime;  
        if(timerLastAction > Mathf.Clamp((minActionRate - coinsCollected), maxActionRate, minActionRate)){
            timerLastAction = 0;
            DoingAction = true;
             */
            if(!DoingAction){
            DoingAction = true;
            ghostChasePlayer();
            }

            /* ghostChangeSides(); */

        /* } */  
    }
    void ghostChasePlayer(){
        /* Vector2 ghostPos = new Vector2(rb2d.transform.position.x, rb2d.transform.position.y);
        Vector2 TargetPos = new Vector2(target.transform.position.x, target.transform.position.y); */
        float angle = Vector2.Angle(rb2d.transform.position, target.transform.position);
        Vector2 finalPos = new Vector2 (Mathf.Cos(angle)*ghostDistance, Mathf.Sin(angle)*ghostDistance);
        var position = Vector2.MoveTowards(rb2d.transform.position, finalPos , ghostSpeed * Time.deltaTime );
        rb2d.MovePosition(position);
    }
    void ghostChangeSides(){

    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.tag == "Player"){

        }
    }
}
