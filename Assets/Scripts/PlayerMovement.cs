using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public LevelManager theLevelManager;

    public Transform rayPosition1;
    public Transform rayPosition2;

    private RaycastHit raycastHit1;
    private RaycastHit raycastHit2;

    public GameObject pivotMarker;

    private bool turnExecuted = false;

    private Vector3 pivotPoint;
    private Vector3 pivotAxis;
    private float movementX;
    private float movementZ;


    private Vector3 pivotPointXOffset;
    private Vector3 pivotPointYOffset;
    private Vector3 pivotPointZOffset;

    private bool standingUpright = true;
    private bool lyingOnXAxis = false;


    // Update is called once per frame
    void Update()
    {
        // If you have a keydown
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) {
            if (!turnExecuted) {
                MovePlayer();
                CheckPlayerPosition();
                turnExecuted = true;
            }
        }
        // No keys down
        else {
            turnExecuted = false;
        }
    }

    public void MovePlayer() {
        
        pivotPointXOffset = Vector3.zero;
        pivotPointYOffset = Vector3.zero;
        pivotPointZOffset = Vector3.zero;

        movementX = Input.GetAxisRaw("Horizontal");
        movementZ = Input.GetAxisRaw("Vertical");

        // If moving right
        if (movementX != 0) {
            pivotPointXOffset = movementX > 0 ? Vector3.right : Vector3.left;
            pivotAxis = movementX > 0 ? Vector3.back : Vector3.forward;
        } 
        else {
            pivotPointZOffset = movementZ > 0 ? Vector3.forward : Vector3.back;
            pivotAxis = movementZ > 0 ? Vector3.right : Vector3.left;
        }

        // Player is standing up
        if (transform.position.y > 0.5f) {
            pivotPointYOffset = Vector3.down;
            lyingOnXAxis = movementX != 0;
            pivotPointXOffset.x /= 2;
            pivotPointZOffset.z /= 2;
        }
        // Player is lying down
        else {
            pivotPointYOffset = Vector3.down * 0.5f;

            // If moving along x axis
            if (movementX != 0) {
                // If lying on the z azis
                if (!lyingOnXAxis) {
                    pivotPointXOffset.x /= 2;
                }
            }
            else {
                // If lying on the x azis
                if (lyingOnXAxis) {
                    pivotPointZOffset.z /= 2;
                }
            }
        }

        pivotPoint = transform.position + pivotPointXOffset + pivotPointYOffset + pivotPointZOffset;
        pivotMarker.transform.position = pivotPoint;
        pivotMarker.transform.rotation = Quaternion.LookRotation(pivotAxis);

        this.transform.RotateAround(pivotPoint, pivotAxis, 90f);
            
    }

    public void CheckPlayerPosition() {

        bool ray1Hit = Physics.Raycast(rayPosition1.transform.position, Vector3.down, out raycastHit1, 10f);
        bool ray2Hit = Physics.Raycast(rayPosition2.transform.position, Vector3.down, out raycastHit2, 10f);

        Debug.DrawRay(rayPosition1.position, Vector3.down * 10f, Color.red);
        Debug.DrawRay(rayPosition2.position, Vector3.down * 10f, Color.red);

        // If ray 1 or 2 has missed it means we fell off board
        if (!ray1Hit || !ray2Hit) {
            // Debug.Log("Game over!");
            transform.position = theLevelManager.startingPosition.position;
            transform.rotation = theLevelManager.startingPosition.rotation;
            return;
        }

        if (CheckStandingOnLightFloor(raycastHit1, raycastHit2)) {
            transform.position = theLevelManager.startingPosition.position;
            transform.rotation = theLevelManager.startingPosition.rotation;
            return;
        }

        CheckHitSwitch(raycastHit1, raycastHit2);


        if (raycastHit1.transform.CompareTag("Goal") && raycastHit2.transform.CompareTag("Goal")) {
            Debug.Log("Game won!");
            theLevelManager.NextLevel();
        }
    }

    public bool CheckStandingUp(RaycastHit rayHit1, RaycastHit rayHit2) {
        return rayHit1.transform.gameObject == rayHit2.transform.gameObject;
    }

    public void CheckHitSwitch(RaycastHit rayHit1, RaycastHit rayHit2) {

        SwitchTile ray1SwitchTile = rayHit1.transform.GetComponent<SwitchTile>();
        SwitchTile ray2SwitchTile = rayHit2.transform.GetComponent<SwitchTile>();


        if (ray1SwitchTile) {
            // If on a soft switch
            if (ray1SwitchTile.switchInfo.isSoftSwitch) {
                ray1SwitchTile.TriggerSwitch();
            }
            // If on a hard switch
            else {
                // If both rays hit same switch it means we're standing upright
                if (ray1SwitchTile == ray2SwitchTile) {
                    ray1SwitchTile.TriggerSwitch();
                }
            }
        }
        else if (ray2SwitchTile)  {
            if (ray2SwitchTile.switchInfo.isSoftSwitch) {
                ray2SwitchTile.TriggerSwitch();
            }
        }
        
    }

    public bool CheckStandingOnLightFloor(RaycastHit rayHit1, RaycastHit rayHit2) {
        return CheckStandingUp(rayHit1, rayHit2) && rayHit1.transform.CompareTag("Light floor");
    }
}
