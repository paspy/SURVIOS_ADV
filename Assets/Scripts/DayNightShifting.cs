using UnityEngine;
using System.Collections;

public class DayNightShifting : MonoBehaviour {

    public Transform Sun;
    public float MaxSunIntensity = 1.2f;
    public float MinSunIntensity = 0.0f;
    public float SunSetPoint = -0.1f;
    public Transform Moon;
    public float MaxMoonIntensity = 0.3f;
    public float MinMoonIntensity = 0.1f;
    public Transform Star;

    public float shiftingSpeed = 1;

    public HexGrid hexGrid;


    Light SunLight;
    Light MoonLight;
    void Awake() {
        SunLight = Sun.GetComponent<Light>();
        MoonLight = Moon.GetComponent<Light>();
    }

    void Start() {
        if (hexGrid.isActiveAndEnabled)
            RefreshStarPosition();
    }

    void Update() {
        Sun.transform.RotateAround(Vector3.zero, Vector3.right, shiftingSpeed * Time.deltaTime);
        Moon.transform.RotateAround(Vector3.zero, Vector3.right, shiftingSpeed * Time.deltaTime);
        Star.transform.Rotate(Vector3.right, shiftingSpeed * Time.deltaTime * 0.5f);

        float dot = Mathf.Clamp01((Vector3.Dot(Sun.forward, Vector3.down) - SunSetPoint) / (1 + SunSetPoint));
        SunLight.intensity = ((MaxSunIntensity - MinSunIntensity) * dot) + MinSunIntensity;
        dot = Mathf.Clamp01(Vector3.Dot(Moon.forward, Vector3.down));
        MoonLight.intensity = ((MaxMoonIntensity - MinMoonIntensity) * dot) + MinMoonIntensity;
    }

    public void RefreshStarPosition() {
        var cells = hexGrid.GetCells();
        var midpoint = cells[cells.Length / 2 + (int)(Mathf.Sqrt(cells.Length) / 2)];
        Star.transform.position = midpoint.Position;
    }
}