using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameController : MonoBehaviour {

    public GameObject PlayerA;
    public GameObject PlayerB;
    public GameObject[] OutOfRangeIndicators;
    public HexGrid hexGrid;
    public Setting setting;

    [SerializeField]
    bool isSingleMode = true;

    private void Awake() {
        var go = GameObject.FindGameObjectWithTag("Setting");
        if (go) {
            setting = go.GetComponent<Setting>();
            isSingleMode = setting.IsSinglePlayer;
        }
    }

    private void Start() {

        var spwanCandidates = hexGrid.GetCells().ToList().FindAll(x => x.Elevation == 3 && !x.HasTree).ToList();
        var placeOne = spwanCandidates[Random.Range(0, spwanCandidates.Count)];
        var placeTwo = spwanCandidates[Random.Range(0, spwanCandidates.Count)];

        PlayerA.SetActive(true);
        if (isSingleMode) {
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

    private void Update() {
        var hexP1 = HexCoordinates.FromPosition(PlayerA.transform.position);
        var hexP2 = HexCoordinates.FromPosition(PlayerB.transform.position);

        if (HexCoordinates.GetHexDistance(hexP1, hexP2) >= 8) {
            foreach (var indicator in OutOfRangeIndicators) {
                indicator.SetActive(true);
            }
        } else {
            foreach (var indicator in OutOfRangeIndicators) {
                indicator.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.F12)) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            BackToMainMenu();
        }

    }

    public void BackToMainMenu() {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);

    }

}
