using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassBehavior : MonoBehaviour {

    [Tooltip("Attach a fire effect gameObject")]
    public GameObject grassFire;

    [Tooltip("Destroy after this time passed.")]
    public float ignitedDuration = 15.0f;

    bool onFire;
    private void Awake() {
        grassFire.SetActive(false);
        onFire = false;
    }

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

    public void SetOnFire() {
        if (onFire) return;
        onFire = true;
        grassFire.SetActive(true);
        Destroy(gameObject, ignitedDuration);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Projectile") {
            SetOnFire();
        }
    }


}
