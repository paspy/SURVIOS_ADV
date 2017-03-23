using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PigBehavior : MonoBehaviour {
    public float speed = 3;
    public float directionChangeInterval = 1;
    public float maxHeadingChange = 30;
    public PigFSM behaviorState = PigFSM.eIdle;

    public Animator LegAnimation;
    public Vector3 hexPosition;
    public enum PigFSM {
        eIdle = 0,
        eWalk,
        eEscape,
    }

    CharacterController controller;
    Vector3 targetRotation;
    HexCell spawnHexCell;
    float heading;
    float stateTimer;

    void Awake() {
        controller = GetComponent<CharacterController>();
    }

    void Start() {
        heading = Random.Range(0, 360);
        transform.eulerAngles = new Vector3(0, heading, 0);
        transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        stateTimer = Random.Range(1, 10);
        spawnHexCell = GameObject.FindGameObjectWithTag("HexGrid").GetComponent<HexGrid>().GetCell(transform.position);
        Debug.Log(spawnHexCell.Position);
    }

    void Update() {
        BehaviorFSM();
        hexPosition = HexCoordinates.FromWorldToHexPosition(transform.position);
    }

    void BehaviorFSM() {
        switch (behaviorState) {
            case PigFSM.eIdle: {
                    GetNewHeading();
                    if ((stateTimer -= Time.deltaTime) <= 0) {
                        stateTimer = Random.Range(1, 10);
                        behaviorState = PigFSM.eWalk;
                        LegAnimation.SetBool("Moving", true);
                    }
                }
                break;
            case PigFSM.eWalk: {
                    GetNewHeading();
                    transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, targetRotation, Time.deltaTime * directionChangeInterval);
                    var forward = transform.TransformDirection(Vector3.forward);
                    controller.SimpleMove(forward * speed);

                    if ((stateTimer -= Time.deltaTime) <= 0) {
                        stateTimer = Random.Range(1, 10);
                        behaviorState = PigFSM.eIdle;
                        LegAnimation.SetBool("Moving", false);
                    }

                }
                break;
            case PigFSM.eEscape: {

                }

                break;
            default:
                break;
        }
    }

    void GetNewHeading() {
        var floor = Mathf.Clamp(heading - maxHeadingChange, 0, 360);
        var ceil = Mathf.Clamp(heading + maxHeadingChange, 0, 360);
        heading = Random.Range(floor, ceil);
        targetRotation = new Vector3(0, heading, 0);
    }
}
