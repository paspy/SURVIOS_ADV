using System;
using UnityEngine;
using XInputDotNetPure;

[Serializable]
public class MouseLook {
    public float XSensitivity = 2f;
    public float YSensitivity = 2f;
    public bool clampVerticalRotation = true;
    public float MinimumX = -90F;
    public float MaximumX = 90F;
    public bool smooth;
    public float smoothTime = 5f;
    public bool lockCursor = true;

    private Quaternion m_CharacterTargetRot;
    private Quaternion m_CameraTargetRot;
    public Quaternion CharacterTargetRot {
        get { return m_CharacterTargetRot; }
    }
    public Quaternion CameraTargetRot {
        get { return m_CameraTargetRot; }
    }

    private bool m_cursorIsLocked = true;
    GamePadState gamePadStateOne;
    GamePadState gamePadStateTwo;
    Transform player;

    public void Init(Transform character, Transform camera) {
        m_CharacterTargetRot = character.localRotation;
        m_CameraTargetRot = camera.localRotation;
        player = character;
    }


    public void LookRotation(Transform character, Transform camera) {
        var p = player.GetComponent<PlayerBehavior>();
        gamePadStateOne = GamePad.GetState(PlayerIndex.One);
        gamePadStateTwo = GamePad.GetState(PlayerIndex.Two);

        float yRot = 0, xRot = 0;
        switch (p.playerType) {
            default:
            case PlayerBehavior.PlayerType.SinglePlayer:
                yRot = Input.GetAxis("Mouse X") * XSensitivity;
                xRot = Input.GetAxis("Mouse Y") * YSensitivity;
                break;
            case PlayerBehavior.PlayerType.Player_A:
                if (gamePadStateOne.IsConnected) {
                    yRot = gamePadStateOne.ThumbSticks.Right.X * 5.0f;
                    xRot = gamePadStateOne.ThumbSticks.Right.Y * 5.0f;
                }
                break;
            case PlayerBehavior.PlayerType.Player_B:
                if (gamePadStateTwo.IsConnected) {
                    yRot = gamePadStateTwo.ThumbSticks.Right.X * 5.0f;
                    xRot = gamePadStateTwo.ThumbSticks.Right.Y * 5.0f;
                }
                break;
        }
        
        

        m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
        m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

        if (clampVerticalRotation)
            m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

        if (smooth) {
            character.localRotation = Quaternion.Slerp(character.localRotation, m_CharacterTargetRot,
                smoothTime * Time.deltaTime);
            camera.localRotation = Quaternion.Slerp(camera.localRotation, m_CameraTargetRot,
                smoothTime * Time.deltaTime);
        } else {
            character.localRotation = m_CharacterTargetRot;
            camera.localRotation = m_CameraTargetRot;
        }

        //UpdateCursorLock();
    }

    public void SetCursorLock(bool value) {
        lockCursor = value;
        if (!lockCursor) {//we force unlock the cursor if the user disable the cursor locking helper
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void UpdateCursorLock() {
        //if the user set "lockCursor" we check & properly lock the cursors
        if (lockCursor)
            InternalLockUpdate();
    }

    private void InternalLockUpdate() {
        if (Input.GetKeyUp(KeyCode.Escape)) {
            m_cursorIsLocked = false;
        } else if (Input.GetMouseButtonUp(0)) {
            m_cursorIsLocked = true;
        }

        if (m_cursorIsLocked) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        } else if (!m_cursorIsLocked) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    Quaternion ClampRotationAroundXAxis(Quaternion q) {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }

}
