using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassBehavior : MonoBehaviour {

    public GameObject grassFire;

    public float ignitedDuration = 15.0f;
    public float damageRadius = 2.5f;

    bool onFire;
    float damagePerSec = 0;
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

    private void Update() {
        if (onFire && (damagePerSec += Time.deltaTime) >= 1) {
            damagePerSec = 0;
            Vector3 damagePos = transform.position;
            Collider[] colliders = Physics.OverlapSphere(damagePos, damageRadius);
            foreach (Collider hit in colliders) {
                var pig = hit.GetComponent<PigBehavior>();
                var player = hit.GetComponent<PlayerBehavior>();
                if (pig != null)
                    pig.HitByProjectile(null);
                if (player != null)
                    player.ApplyDamage(-5);
            }
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
