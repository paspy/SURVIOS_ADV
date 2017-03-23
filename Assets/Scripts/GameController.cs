using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public GameObject[] Players;

    private void Awake() {
        Players = GameObject.FindGameObjectsWithTag("Player");
    }

    private void Start() {

    }

    private void Update() {

    }
}
