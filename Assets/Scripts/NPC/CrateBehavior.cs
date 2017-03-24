using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateBehavior : MonoBehaviour {
    private void Awake() {
        transform.position = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);
    }
}
