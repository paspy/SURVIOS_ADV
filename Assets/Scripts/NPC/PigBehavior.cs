﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigBehavior : MonoBehaviour {
    public Transform Drop;
    public float speed = 3;
    public int freezeDistance = 2;
    public bool IsFroze;
    public float directionChangeInterval = 1;
    public float maxHeadingChange = 30;
    public PigFSM behaviorState = PigFSM.eIdle;

    public Animator LegAnimation;
    public Vector3 hexPosition;
    public enum PigFSM {
        eIdle = 0,
        eWalk
    }

    CharacterController characterCtrl;
    GameController gameCtrl;
    Vector3 targetRotation;
    float heading;
    float stateTimer;
    bool Hit = false;

    void Awake() {
        characterCtrl = GetComponent<CharacterController>();
        gameCtrl = FindObjectOfType<GameController>();
    }

    void Start() {
        heading = Random.Range(0, 360);
        transform.eulerAngles = new Vector3(0, heading, 0);
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        stateTimer = Random.Range(1, 10);
        if (Drop != null) {
            var cb = Drop.GetComponent<CollectibleBehavior>();
            cb.type = CollectibleBehavior.CollectibleType.Bacon;
            cb.IsRandomSupply = false;
        }
    }

    void Update() {

        if (gameCtrl.PlayerA != null) {
            var playerHexPos = HexCoordinates.FromPosition(gameCtrl.PlayerA.transform.position);
            var myHexPos = HexCoordinates.FromPosition(transform.position);
            IsFroze = (HexCoordinates.GetHexDistance(playerHexPos, myHexPos) > freezeDistance);
        }
        if (gameCtrl.PlayerB != null && !gameCtrl.setting.IsSinglePlayer) {
            var playerHexPos = HexCoordinates.FromPosition(gameCtrl.PlayerB.transform.position);
            var myHexPos = HexCoordinates.FromPosition(transform.position);
            IsFroze = (HexCoordinates.GetHexDistance(playerHexPos, myHexPos) > freezeDistance);
        }

        BehaviorFSM();
        hexPosition = HexCoordinates.FromWorldToHexPosition(transform.position);
    }

    void BehaviorFSM() {
        if (IsFroze) return;
        switch (behaviorState) {
            case PigFSM.eIdle: {
                    if ((stateTimer -= Time.deltaTime) <= 0) {
                        stateTimer = Random.Range(1, 10);
                        behaviorState = PigFSM.eWalk;
                        LegAnimation.SetBool("Moving", true);
                    }
                }
                break;
            case PigFSM.eWalk: {
                    if ((stateTimer -= Time.deltaTime) <= 0) {
                        stateTimer = Random.Range(1, 10);
                        behaviorState = PigFSM.eIdle;
                        LegAnimation.SetBool("Moving", false);
                    }
                    GetNewHeading();
                    transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, targetRotation, Time.deltaTime * directionChangeInterval);
                    var forward = transform.TransformDirection(Vector3.forward);
                    if (characterCtrl)
                        characterCtrl.SimpleMove(forward * speed);

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

    public void HitByProjectile(Transform owner, float time = 3.5f) {
        if (!Hit) {
            if (Random.Range(0, 100) <= 5 && Drop != null) {
                Drop.GetComponent<CollectibleBehavior>().owner = owner;
                Instantiate(Drop, transform.position, Quaternion.identity);
            }
        }
        stateTimer = time;
        Destroy(characterCtrl);
        Destroy(gameObject, time);
        GetComponent<Rigidbody>().isKinematic = false;
        Hit = true;

    }
}
