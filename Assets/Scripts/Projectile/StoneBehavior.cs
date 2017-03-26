using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StoneBehavior : MonoBehaviour {

    public Transform Grabber;
    public float speed;

    public int damage = 10;
    public float damageForce = 15.0f;
    public float damageRadius = 2.5f;

    Camera GrabberEyes;
    Rigidbody rigid;
    Transform originalParent;
    Transform lastOwner;
    string originalTag;
    private void Awake() {
        rigid = GetComponent<Rigidbody>();
        originalTag = tag;
    }

    private void Start() {
        originalParent = transform.parent;
        damage *= (int)rigid.mass;
    }

    private void Update() {

        if (Grabber != null) {
            rigid.isKinematic = true;
            transform.parent = GrabberEyes.transform;

        }

        speed = rigid.velocity.magnitude;
        if (tag != originalTag && speed < 1.0f) {
            tag = originalTag;
        }

    }

    private void OnTriggerEnter(Collider other) {
        if (lastOwner == null) return;
        ReleaseStone();
        if (lastOwner.name != other.transform.name && speed > 5.0f) {
            CreateDamage();
        }
    }

    public void AssignToGrabber(Transform player, string tag = "Projectile") {
        if (Grabber == null) {
            Grabber = player;
            lastOwner = player;
            GrabberEyes = Grabber.GetComponentInChildren<Camera>();
            this.tag = tag;
        }
    }

    public void ReleaseStone() {
        Grabber = null;
        rigid.isKinematic = false;
        transform.parent = originalParent;
    }

    public void ThrowStone(Vector3 direction) {
        if (Grabber != null) {
            ReleaseStone();
            rigid.velocity = direction;
        }
    }

    void CreateDamage() {
        Vector3 damagePos = transform.position;
        var colliders = Physics.OverlapSphere(damagePos, damageRadius).ToList();
        var playerCol = colliders.Find(x => x.GetComponent<PlayerBehavior>() != null);
        if (playerCol != null)
            playerCol.GetComponent<PlayerBehavior>().ApplyDamage(-damage);

        foreach (Collider hit in colliders) {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            var pig = hit.GetComponent<PigBehavior>();
            if (pig != null && rb != null) {
                pig.HitByProjectile(lastOwner);
                rb.AddExplosionForce(damageForce, damagePos, damageRadius, 3.0f);
            }
        }

    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }

}
