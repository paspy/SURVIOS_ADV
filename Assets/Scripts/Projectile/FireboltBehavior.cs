using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireboltBehavior : MonoBehaviour {

    public Transform owner;

    public Transform shootingEffect;
    public AudioClip[] shootingSFXs;
    public Transform exposionEffect;
    public AudioClip[] exposionSFXs;

    public float speed = 100.0f;
    public float expolsionRadius = 5.0f;
    public float expolsionPower = 50.0f;
    public float expireTime = 5.0f;
    public Vector3 movingDirection;
    Rigidbody rigid;
    AudioSource sfxSource;
    bool isExpired;
    private void Awake() {
        rigid = GetComponent<Rigidbody>();
        sfxSource = GetComponent<AudioSource>();
        isExpired = false;
        movingDirection = transform.forward;
    }

    private void Start() {
        rigid.velocity = movingDirection * speed;
        sfxSource.clip = shootingSFXs[Random.Range(0, shootingSFXs.Length)];
        if (!sfxSource.isPlaying)
            sfxSource.Play();
    }

    private void FixUpdate() {
        if ((expireTime -= Time.deltaTime) <= 0 && !isExpired) {
            isExpired = true;
            CreateExplosion();
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.transform.name != owner.name && !isExpired) {
            rigid.isKinematic = true;
            rigid.velocity = Vector3.zero;
            CreateExplosion();
        }
    }

    void CreateExplosion() {
        shootingEffect.gameObject.SetActive(false);

        var p = exposionEffect.GetComponentInChildren<ParticleSystem>();
        p.Play();

        sfxSource.Stop();
        sfxSource.clip = exposionSFXs[Random.Range(0, exposionSFXs.Length)];
        sfxSource.Play();

        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, expolsionRadius);
        foreach (Collider hit in colliders) {
            //Debug.Log("Hit: " + hit.gameObject.name);
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            var grass = hit.GetComponent<GrassBehavior>();
            var pig = hit.GetComponent<PigBehavior>();
            if (pig != null)
                pig.HitByFirebolt(owner);
            if (grass != null)
                grass.SetOnFire();
            if (rb != null)
                rb.AddExplosionForce(expolsionPower, explosionPos, expolsionRadius, 3.0F);
        }

        Destroy(gameObject, 1.25f);
    }
}
