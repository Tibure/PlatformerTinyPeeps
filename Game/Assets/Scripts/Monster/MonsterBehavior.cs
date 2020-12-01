using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MonsterBehavior : MonoBehaviour
{
    public GameObject Ghost;
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
        Ghost = GameObject.FindGameObjectWithTag("Ghost");
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
            print("YEET");
            ghostChasePlayer();
            }

            /* ghostChangeSides(); */

        /* } */  
    }
    void ghostChasePlayer(){
        /* Vector2 ghostPos = new Vector2(rb2d.transform.position.x, rb2d.transform.position.y);
        Vector2 TargetPos = new Vector2(target.transform.position.x, target.transform.position.y); */
        float angle = Vector2.Angle(Ghost.transform.position, target.transform.position);
        print(angle);
        Vector2 finalPos = new Vector2 (Mathf.Cos(angle)*ghostDistance, Mathf.Sin(angle)*ghostDistance);
        print(finalPos);
        var position = Vector3.MoveTowards(Ghost.transform.position, finalPos , ghostSpeed * Time.deltaTime );
        print(position);
        Ghost.transform.position = position;
    }
    void ghostChangeSides(){

    }

    private void OnTriggerEnter2D(Collider2D collision) {
        print("YOUTOUCHED ME");
        if(collision.tag == "Player"){

        }
    }
}
