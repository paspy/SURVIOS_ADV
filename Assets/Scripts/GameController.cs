using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour {

    public GameObject PlayerA;
    public GameObject PlayerB;
    public HexGrid hexGrid;
    public Setting setting;
    private void Awake() {
        setting = GameObject.FindGameObjectWithTag("Setting").GetComponent<Setting>();

    }

    private void Start() {

        var spwanCandidates = hexGrid.GetCells().ToList().FindAll(x => x.Elevation == 3).ToList();
        var placeOne = spwanCandidates[Random.Range(0, spwanCandidates.Count)];
        var placeTwo = spwanCandidates[Random.Range(0, spwanCandidates.Count)];

        if (setting) {
            PlayerA.SetActive(true);
            if (setting.IsSinglePlayer) {
                PlayerB.SetActive(false);
                PlayerA.GetComponent<PlayerBehavior>().playerType = PlayerBehavior.PlayerType.SinglePlayer;
            } else {
                PlayerB.SetActive(true);
                PlayerA.GetComponent<PlayerBehavior>().playerType = PlayerBehavior.PlayerType.Player_A;
                PlayerB.GetComponent<PlayerBehavior>().playerType = PlayerBehavior.PlayerType.Player_B;
            }
            PlayerA.transform.position = new Vector3(placeOne.Position.x, PlayerA.transform.position.y, placeOne.Position.z);
            PlayerB.transform.position = new Vector3(placeTwo.Position.x, PlayerB.transform.position.y, placeTwo.Position.z);
        }

    }

    private void Update() {

    }

}
