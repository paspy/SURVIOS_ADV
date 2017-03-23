using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassBehavior : MonoBehaviour {
    
    // random distribute grasses
    void Start() {
        var grassSet = GetComponentsInChildren<Transform>();
        foreach (var grass in grassSet) {
            grass.position = new Vector3(
                grass.position.x + Random.Range(-0.25f, 0.25f),
                grass.position.y,
                grass.position.z + Random.Range(-0.25f, 0.25f)
            );
        }
    }
}
