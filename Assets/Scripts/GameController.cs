using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public GameObject PlayerA;
    public GameObject PlayerB;
    public GameObject[] OutOfRangeIndicators;
    public HexGrid hexGrid;
    public Setting setting;

    public Text StarvingTimer;

    [SerializeField]
    bool isSingleMode = true;

    [SerializeField]
    float maxStarvingTimer = 120.0f;

    float starvingBuffer;


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

        starvingBuffer = maxStarvingTimer;
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

        if ((starvingBuffer -= Time.deltaTime) <= 0) {
            starvingBuffer = maxStarvingTimer;
            PlayerA.GetComponent<PlayerBehavior>().Bacon -= 1;
            PlayerB.GetComponent<PlayerBehavior>().Bacon -= 1;
        }
        StarvingTimer.text = starvingBuffer.ToString("N1");

    }

    public void BackToMainMenu() {
        Destroy(setting.gameObject);
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

}
