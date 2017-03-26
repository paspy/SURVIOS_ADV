using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class PigFocusField : MonoBehaviour {
    public Transform neck;
    Transform parent;
    Transform target;
    public float turnSpeed = 5;
    private void Start() {
        parent = GetComponentInParent<Transform>();
        target = parent;
    }

    private void Update() {
        if (!GetComponentInParent<PigBehavior>().IsFroze) {
            var targetRotation = Quaternion.LookRotation(target.transform.position - neck.position);
            neck.rotation = Quaternion.Slerp(neck.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            target = other.GetComponentInChildren<Camera>().transform;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player") {
            target = parent;
        }
    }

}
