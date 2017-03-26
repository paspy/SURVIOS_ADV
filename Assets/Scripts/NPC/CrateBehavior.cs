using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateBehavior : MonoBehaviour {

    public Transform Drop;

    private void Awake() {
        transform.position = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);
    }

    public void SpawnSupply(Transform owner) {
        if (Drop) {
            Drop.GetComponent<CollectibleBehavior>().owner = owner;
            Drop.GetComponent<CollectibleBehavior>().IsRandomSupply = true;
            Instantiate(Drop, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
