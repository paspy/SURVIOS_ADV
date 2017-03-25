using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneBehavior : MonoBehaviour {

    public Transform Grabber;
    public float speed;

    public float damage;

    Camera GrabberEyes;
    Rigidbody rigid;
    Transform originalParent;
    string originalTag;
    private void Awake() {
        rigid = GetComponent<Rigidbody>();
        originalTag = tag;
    }

    private void Start() {
        originalParent = transform.parent;
        damage *= rigid.mass;
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
        ReleaseStone();
    }

    public void AssignToGrabber(Transform player, string tag = "Projectile") {
        if (Grabber == null) {
            Grabber = player;
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
}
