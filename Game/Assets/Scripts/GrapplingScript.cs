using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GrapplingScript : MonoBehaviour
{

    private Vector3 myMousePos;
    private Vector3 posTempo;
    private Camera myCamera;
    private bool checkClick;
    private DistanceJoint2D myDistanceJoint2D;
    private LineRenderer myLineRenderer;
    RaycastHit2D myRaycast;
    // Start is called before the first frame update
    void Start()
    {
        myCamera = Camera.main;
        myDistanceJoint2D = GetComponent<DistanceJoint2D>();
        myLineRenderer = GetComponent<LineRenderer>();
        myDistanceJoint2D.enabled = false;
        checkClick = true;
        myLineRenderer.positionCount = 0;

    }

    // Update is called once per frame
    void Update()
    {
        UpdateMousePosition();
        Debug.DrawLine(gameObject.transform.position, myMousePos, new Color(1,0,0), 5);
        myRaycast = Physics2D.Raycast(myMousePos, myMousePos - gameObject.transform.position);
        if (myRaycast.collider != null)
        {
            print(myRaycast.collider.tag);
        }
        
        if (Input.GetMouseButtonDown(0) && checkClick )
        {
            gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            myDistanceJoint2D.enabled = true;
            myDistanceJoint2D.connectedAnchor = myMousePos;
            myLineRenderer.positionCount = 2;
            posTempo = myMousePos;
            checkClick = false;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            myDistanceJoint2D.enabled = false;
            myLineRenderer.positionCount = 0;
            checkClick = true;
            
            gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
            gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
        }
        DrawGrapplinLine();
    }

    private void DrawGrapplinLine()
    {
        if (myLineRenderer.positionCount <= 0)
        {
            return;
        } 
        myLineRenderer.SetPosition(0, transform.position);
        myLineRenderer.SetPosition(1, posTempo);
    }

    private void UpdateMousePosition()
    {
        myMousePos = myCamera.ScreenToWorldPoint(Input.mousePosition);
    }

}
