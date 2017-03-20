using UnityEngine;
using System.Collections;

public class DayNightShifting : MonoBehaviour {

    public Transform Sun;
    public Transform Moon;
    public Transform Star;

    public float dayRotateSpeed = 10;
    public float nightRotateSpeed = 20;

    public HexGrid hexGrid;

    float skySpeed = 1;

    void Awake() {

    }

    void Start() {
        var cells = hexGrid.GetCells();
        var midpoint = cells[cells.Length / 2 + (int)(Mathf.Sqrt(cells.Length) / 2)];
        Star.transform.position = midpoint.Position;
    }

    void Update() {
        Sun.transform.RotateAround(Vector3.zero, Vector3.right, dayRotateSpeed * Time.deltaTime * skySpeed);
        Moon.transform.RotateAround(Vector3.zero, Vector3.right, dayRotateSpeed * Time.deltaTime * skySpeed);
        Star.transform.Rotate(Vector3.right, dayRotateSpeed * Time.deltaTime * skySpeed*0.5f);

    }
}