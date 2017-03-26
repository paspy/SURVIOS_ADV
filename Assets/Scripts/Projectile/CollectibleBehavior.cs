using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleBehavior : MonoBehaviour {

    public enum CollectibleType {
        Health = 0,
        Mana,
        Bacon,
        TYPE_COUNT
    }

    public CollectibleType type = CollectibleType.Health;
    public Material[] mats;
    public bool IsRandomSupply;
    public Transform owner;

    float speed = 1.0f;

    private void Awake() {

    }

    private void Start() {
        if (IsRandomSupply) {
            type = (CollectibleType)Random.Range((int)CollectibleType.Health, (int)CollectibleType.TYPE_COUNT);
        }
        transform.position = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);
        GetComponent<MeshRenderer>().material = mats[(int)type];
    }

    private void Update() {
        if (owner != null && Vector3.Distance(transform.position, owner.position) <= 20.0f) {
            transform.position = Vector3.Lerp(transform.position, owner.position, Time.deltaTime * speed);
            if (Vector3.Distance(transform.position, owner.position) <= 1.0f) {
                var player = owner.GetComponent<PlayerBehavior>();
                switch (type) {
                    case CollectibleType.Health:
                        player.HP += 20;
                        break;
                    case CollectibleType.Mana:
                        player.Mana = 100;
                        break;
                    case CollectibleType.Bacon:
                        player.Bacon += 1;

                        break;
                    default:
                        break;
                }

                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player" && owner != null) {
            owner = other.transform;
        }
    }

}
