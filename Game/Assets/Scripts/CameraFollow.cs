using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public GameObject followObject;
    public Vector2 followOffset;
    private Vector2 treshold;
    public float speedCamera = 3f;
    private Rigidbody2D myRigidBody;

    void Start()
    {
        treshold = calculerTreshold();
        myRigidBody = followObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector2 follow = followObject.transform.position;
        float xDifference = Vector2.Distance(Vector2.right * transform.position.x, Vector2.right * follow.x);
        float yDifference = Vector2.Distance(Vector2.up * transform.position.y, Vector2.up * follow.y);

        Vector3 newPosition = transform.position;
        if (Mathf.Abs(xDifference) >= treshold.x)
        {
            newPosition.x = follow.x;
        }
        if (Mathf.Abs(yDifference) >= treshold.y)
        {
            newPosition.y = follow.y;
        }
        //float moveSpeed = myRigidBody.velocity.magnitude > speedCamera ? myRigidBody.velocity.magnitude : speedCamera;
        float moveSpeed = 25;
        transform.position = Vector3.MoveTowards(transform.position, newPosition, moveSpeed * Time.deltaTime);
    }

    private Vector3 calculerTreshold()
    {
        Rect aspect = Camera.main.pixelRect;
        Vector2 t = new Vector2(Camera.main.orthographicSize * aspect.width / aspect.height, Camera.main.orthographicSize);
        t.x -= followOffset.x;
        t.y -= followOffset.y;
        return t;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Vector2 border = calculerTreshold();
        Gizmos.DrawWireCube(transform.position, new Vector3(border.x * 2, border.y * 2, 1));
    }

}
