using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class PlayerBehavior : MonoBehaviour {
    public enum PlayerType { SinglePlayer = 0, Player_A, Player_B }
    public PlayerType playerType = PlayerType.SinglePlayer;

    public Scrollbar HPIndicator;
    public Scrollbar ManaIndicator;
    public Scrollbar BaconIndicator;
    public Scrollbar throwingIndicator;
    public Scrollbar castingIndicator;
    public Image damageIndicator;
    public Text GameOverText;

    public Transform firebolt;
    public Transform magicLight;
    public GameObject grabbedObject;

    [Range(0, 100)]
    public int HP = 100;

    [Range(0, 100)]
    public int Mana = 100;
    private float manaBuffer = 0;

    [Range(0, 10)]
    public int Bacon = 3;

    public bool IsCasting = false;

    public bool IsGrabbing = false;

    public bool attachToCenterOfMass;

    FirstPersonController fpctrl;
    Camera eyes;
    Vector3 screenPosV3;
    Ray rayFromEye;
    RaycastHit hitInfo;

    float maxInterativeDistance = 8.0f;
    float castingTime;
    float maxDamageFading = 150;
    float curDamageFading = 0;
    Color damageColor;
    float throwingPower = 0;
    bool isLightOn;

    GamePadState prevGamePadStateOne;
    GamePadState currGamePadStateOne;

    GamePadState prevGamePadStateTwo;
    GamePadState currGamePadStateTwo;

    bool castingKeyPress = false;
    bool grabbingKeyPress = false;
    bool lightKeyPress = false;

    private void Awake() {
        eyes = GetComponentInChildren<Camera>();
        fpctrl = GetComponent<FirstPersonController>();
    }

    private void Start() {
        switch (playerType) {
            case PlayerType.SinglePlayer:
                screenPosV3 = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                break;
            case PlayerType.Player_A:
                screenPosV3 = new Vector3(Screen.width / 2, Screen.height / 4, 0);
                eyes.rect = new Rect(0, 0, 1.0f, 0.5f);
                break;
            case PlayerType.Player_B:
                screenPosV3 = new Vector3(Screen.width / 2, Screen.height - (Screen.height / 4), 0);
                eyes.rect = new Rect(0, 0.5f, 1.0f, 0.5f);
                break;
            default:
                break;
        }

        if (playerType != PlayerType.SinglePlayer) {
            var rectTransform = HPIndicator.transform.parent.GetComponent<RectTransform>();
            /*Left X*/
            //rectTransform.offsetMin.x;
            /*Right Width*/
            //rectTransform.offsetMax.x;
            /*Top Y*/
            //rectTransform.offsetMax.y;
            /*Bottom Height*/
            //rectTransform.offsetMin.y;
            var newBottom = (rectTransform.offsetMin.y - 130) / 2;
            rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, newBottom);
            rectTransform = throwingIndicator.GetComponent<RectTransform>();
            rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y / 2, rectTransform.localPosition.z);
            rectTransform = castingIndicator.GetComponent<RectTransform>();
            rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y / 2, rectTransform.localPosition.z);

        }

        grabbedObject = null;
        isLightOn = false;
        damageColor = damageIndicator.color;
        castingTime = 0;
    }

    void UpdateHUD() {
        ManaIndicator.size = Mana * 0.01f;
        ManaIndicator.GetComponentInChildren<Text>().text = Mana.ToString();

        HPIndicator.size = HP * 0.01f;
        HPIndicator.GetComponentInChildren<Text>().text = HP.ToString();

        BaconIndicator.size = Bacon * 0.1f;
        BaconIndicator.GetComponentInChildren<Text>().text = Bacon.ToString();

        if (HP > 100) HP = 100;
        if (Mana > 100) Mana = 100;
        if (Bacon > 10) Bacon = 10;

    }

    void UpdateInput() {
        prevGamePadStateOne = currGamePadStateOne;
        prevGamePadStateTwo = currGamePadStateTwo;
        currGamePadStateOne = GamePad.GetState(PlayerIndex.One);
        currGamePadStateTwo = GamePad.GetState(PlayerIndex.Two);

        switch (playerType) {
            default:
            case PlayerType.SinglePlayer:
                lightKeyPress = Input.GetKeyDown(KeyCode.F);
                grabbingKeyPress = Input.GetMouseButton(0);
                castingKeyPress = Input.GetMouseButtonDown(1);

                break;
            case PlayerType.Player_A:
                if (prevGamePadStateOne.IsConnected) {
                    lightKeyPress =
                        prevGamePadStateOne.Buttons.Y == ButtonState.Released &&
                        currGamePadStateOne.Buttons.Y == ButtonState.Pressed;
                    grabbingKeyPress =
                        prevGamePadStateOne.Buttons.LeftShoulder == ButtonState.Pressed &&
                        currGamePadStateOne.Buttons.LeftShoulder == ButtonState.Pressed;
                    castingKeyPress =
                        prevGamePadStateOne.Buttons.B == ButtonState.Released &&
                        currGamePadStateOne.Buttons.B == ButtonState.Pressed;
                }
                break;
            case PlayerType.Player_B:
                if (prevGamePadStateTwo.IsConnected) {
                    lightKeyPress =
                        prevGamePadStateTwo.Buttons.Y == ButtonState.Released &&
                        currGamePadStateTwo.Buttons.Y == ButtonState.Pressed;
                    grabbingKeyPress =
                        prevGamePadStateTwo.Buttons.LeftShoulder == ButtonState.Pressed &&
                        currGamePadStateTwo.Buttons.LeftShoulder == ButtonState.Pressed;
                    castingKeyPress =
                        prevGamePadStateTwo.Buttons.B == ButtonState.Released &&
                        currGamePadStateTwo.Buttons.B == ButtonState.Pressed;

                }
                break;
        }


    }

    private void Update() {

        if (curDamageFading > 0) {
            curDamageFading -= Time.deltaTime;
            damageColor.a = curDamageFading <= 0 ? 0 : curDamageFading;
            damageIndicator.color = damageColor;
        }

        rayFromEye = eyes.ScreenPointToRay(screenPosV3);

        UpdateInput();

        if (!fpctrl.IsJump && !fpctrl.IsJumping && fpctrl.IsWalking) {

            if (grabbingKeyPress && !IsCasting) {

                if (Physics.Raycast(rayFromEye, out hitInfo, maxInterativeDistance) && !IsGrabbing) {
                    Debug.DrawLine(rayFromEye.origin, hitInfo.point, Color.yellow);
                    if (hitInfo.transform.tag != "Grabbable") return;
                    var stone = hitInfo.transform.GetComponent<StoneBehavior>();
                    if (stone != null) {
                        stone.AssignToGrabber(transform);
                        grabbedObject = stone.gameObject;
                    }
                    var crate = hitInfo.transform.GetComponent<CrateBehavior>();
                    if (crate!= null) {
                        crate.SpawnSupply(transform);
                        return;
                    }
                    IsGrabbing = true;
                    throwingIndicator.gameObject.SetActive(IsGrabbing);

                }
                throwingPower = Mathf.Abs(Mathf.Cos(Time.time * 2.0f));
                throwingIndicator.size = throwingPower;
                var cb = throwingIndicator.colors;
                cb.normalColor = Color.Lerp(Color.yellow, Color.red, throwingPower);
                throwingIndicator.colors = cb;

            } else if (!grabbingKeyPress && IsGrabbing) {
                grabbedObject.GetComponent<StoneBehavior>().ThrowStone(rayFromEye.direction * throwingPower * 100.0f);
                grabbedObject = null;
                IsGrabbing = false;
                throwingIndicator.gameObject.SetActive(IsGrabbing);
                throwingPower = 0.0f;
            }

            if (castingKeyPress && !IsGrabbing && !IsCasting && Mana >= 15) {
                IsCasting = true;
                castingIndicator.gameObject.SetActive(IsCasting);
            }
            if (IsCasting) {
                castingIndicator.size = castingTime;
                var cb = castingIndicator.colors;
                cb.normalColor = Color.Lerp(Color.red, Color.green, castingTime);
                castingIndicator.colors = cb;
                CastingFirebolt();
            }

            if (lightKeyPress) {
                isLightOn = !isLightOn;
                magicLight.gameObject.SetActive(isLightOn);
            }

        }

        if (fpctrl.IsJump || fpctrl.IsJumping || !fpctrl.IsWalking) {
            ResetCasting();
            ResetGrabbing();
        }

        if (Mana < 100) {
            if ((manaBuffer += Time.deltaTime) >= 2) {
                manaBuffer = 0;
                Mana += 1;
            }
        }
        UpdateHUD();
    }

    private void CastingFirebolt() {
        if ((castingTime += Time.deltaTime) >= 1.5f) {
            var projectile = Instantiate(firebolt, rayFromEye.origin, Quaternion.identity);
            projectile.gameObject.GetComponent<FireboltBehavior>().movingDirection = rayFromEye.direction;
            projectile.gameObject.GetComponent<FireboltBehavior>().owner = transform;
            ResetCasting();
            Mana -= 15;
        }
    }

    private void ResetCasting() {
        IsCasting = false;
        castingIndicator.gameObject.SetActive(IsCasting);
        castingTime = 0;
        castingIndicator.size = 0;
    }

    private void ResetGrabbing() {
        if (grabbedObject != null) {
            grabbedObject.GetComponent<StoneBehavior>().ReleaseStone();
            grabbedObject = null;
            IsGrabbing = false;
            throwingIndicator.gameObject.SetActive(IsGrabbing);
            throwingPower = 0.0f;
        }
    }


    public void ApplyDamage(int amount) {
        curDamageFading = maxDamageFading / 255.0f;
        HP += amount;
    }

}
