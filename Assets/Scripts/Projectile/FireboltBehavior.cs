using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireboltBehavior : MonoBehaviour {

    public Transform flyingEffect;
    public Transform exposionEffect;

    public string owner;

    public float speed = 100.0f;
    public float expolsionRadius = 5.0f;
    public float expolsionPower = 50.0f;
    public float expireTime = 5.0f;
    public Vector3 movingDirection;
    Rigidbody rigid;
    bool isExpired;
    private void Awake() {
        rigid = GetComponent<Rigidbody>();
        isExpired = false;
        movingDirection = transform.forward;
    }

    private void Start() {
        //rigid.AddForce(transform.forward * speed);
        rigid.velocity = movingDirection * speed;

    }

    private void Update() {
        if ((expireTime -= Time.deltaTime) <= 0) {
            isExpired = true;
            CreateExplosion();
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.transform.name != owner && !isExpired) {
            rigid.isKinematic = true;
            rigid.velocity = Vector3.zero;
            CreateExplosion();
        }
    }

    void CreateExplosion() {
        flyingEffect.gameObject.SetActive(false);
        var p = exposionEffect.GetComponentInChildren<ParticleSystem>();
        p.Play();

        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, expolsionRadius);
        foreach (Collider hit in colliders) {
            //Debug.Log("Hit: " + hit.gameObject.name);
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            var grass = hit.GetComponent<GrassBehavior>();
            var pig = hit.GetComponent<PigBehavior>();
            if (pig != null)
                pig.HitByFirebolt();
            if (grass != null)
                grass.SetOnFire();
            if (rb != null)
                rb.AddExplosionForce(expolsionPower, explosionPos, expolsionRadius, 3.0F);
        }

        Destroy(gameObject, 1.0f);
    }
}
