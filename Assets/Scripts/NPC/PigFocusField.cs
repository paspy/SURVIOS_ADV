using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class PigFocusField : MonoBehaviour {
    public Transform neck;
    Transform parent;
    private void Start() {
        parent = GetComponentInParent<Transform>();
    }

    private void Update() {

    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "PlayerA" ||
            other.tag == "PlayerB" ||
            other.tag == "PlayerC" ||
            other.tag == "PlayerD") {
            neck.LookAt(other.transform);
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.tag == "PlayerA" ||
            other.tag == "PlayerB" ||
            other.tag == "PlayerC" ||
            other.tag == "PlayerD") {
            neck.LookAt(other.transform);
        }
    }
    
    private void OnTriggerExit(Collider other) {
        if (other.tag == "PlayerA" ||
            other.tag == "PlayerB" ||
            other.tag == "PlayerC" ||
            other.tag == "PlayerD") {
            neck.transform.eulerAngles = parent.eulerAngles;
        }
    }

}
