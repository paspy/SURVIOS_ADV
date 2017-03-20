using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PigBehavior : MonoBehaviour {

    public float speed = 5;
    public float directionChangeInterval = 1;
    public float maxHeadingChange = 30;
    public PigFSM state = PigFSM.eIdle;

    public Animator LegAnimation;
    public enum PigFSM {
        eUnknown = -1,
        eIdle = 0,
        eWalk,
        eEscape,
    }

    CharacterController controller;
    float heading;
    Vector3 targetRotation;
    float stateTime;
    void Awake() {
        controller = GetComponent<CharacterController>();
    }

    void Start() {
        // Set random initial rotation
        heading = Random.Range(0, 360);
        transform.eulerAngles = new Vector3(0, heading, 0);
        stateTime = Random.Range(1, 10);
    }

    void Update() {

        switch (state) {
            case PigFSM.eIdle: {
                    NewHeadingRoutine();
                    if ((stateTime -= Time.deltaTime) <= 0) {
                        stateTime = Random.Range(1, 10);
                        state = PigFSM.eWalk;
                        LegAnimation.SetBool("Moving", true);
                    }
                }
                break;
            case PigFSM.eWalk: {
                    NewHeadingRoutine();
                    transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, targetRotation, Time.deltaTime * directionChangeInterval);
                    var forward = transform.TransformDirection(Vector3.forward);
                    controller.SimpleMove(forward * speed);
                    
                    if ((stateTime -= Time.deltaTime) <= 0) {
                        stateTime = Random.Range(1, 10);
                        state = PigFSM.eIdle;
                        LegAnimation.SetBool("Moving", false);
                    }
                }
                break;
            case PigFSM.eEscape: {

                }

                break;
            case PigFSM.eUnknown:
            default:
                break;
        }


    }

    /// <summary>
    /// Calculates a new direction to move towards.
    /// </summary>
    void NewHeadingRoutine() {
        var floor = Mathf.Clamp(heading - maxHeadingChange, 0, 360);
        var ceil = Mathf.Clamp(heading + maxHeadingChange, 0, 360);
        heading = Random.Range(floor, ceil);
        targetRotation = new Vector3(0, heading, 0);
    }
}
