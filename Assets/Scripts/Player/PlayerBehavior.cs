using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.PyroParticles;

public class PlayerBehavior : MonoBehaviour {
    public enum PlayerType { SinglePlayer = 0, Player_A, Player_B }

    public PlayerType playerType = PlayerType.SinglePlayer;
    public Transform firebolt;

    [Range(0, 100)]
    public int hitPoint = 100;

    [Range(0, 5)]
    public int fireBolt = 3;

    [Range(0, 10)]
    public int bacon = 3;

    public bool IsCarryStone = false;
    public bool IsCasting = false;

    FirstPersonController fpctrl;
    Camera eyes;
    Vector3 targetVec3;
    Ray interativeRay;
    RaycastHit rayHitInfo;
    float maxInterativeLength = 10.0f;
    float castingTime = 3.0f;
    private void Awake() {
        eyes = GetComponentInChildren<Camera>();
        fpctrl = GetComponent<FirstPersonController>();

    }

    private void Start() {
        switch (playerType) {
            case PlayerType.SinglePlayer:
                targetVec3 = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                break;
            case PlayerType.Player_A:
                targetVec3 = new Vector3(Screen.width / 4, Screen.height / 2, 0);
                break;
            case PlayerType.Player_B:
                targetVec3 = new Vector3(Screen.width - (Screen.width / 4), Screen.height / 2, 0);
                break;
            default:
                break;
        }

    }

    private void Update() {
        interativeRay = eyes.ScreenPointToRay(targetVec3);

        if (Input.GetMouseButton(0)) {

            if (Physics.Raycast(interativeRay, out rayHitInfo, maxInterativeLength)) {
                Debug.DrawLine(interativeRay.origin, rayHitInfo.point, Color.yellow);

            }

        }
        
        if (!fpctrl.IsJump && !fpctrl.IsJumping && fpctrl.IsWalking) {
            
            if (Input.GetMouseButtonDown(1)) {

                var depolyPos = eyes.ScreenToWorldPoint(targetVec3);
                var facing = eyes.transform.forward;
                var projectile = Instantiate(firebolt, depolyPos, Quaternion.identity);
                projectile.gameObject.GetComponent<FireboltBehavior>().movingDirection = facing;
                projectile.gameObject.GetComponent<FireboltBehavior>().owner = transform.name;

            }
        }

    }

}
