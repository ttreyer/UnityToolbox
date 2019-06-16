using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

/* Setup a camera that is always showing the player what's in front of him!
 *
 * Add this component to a cinemachine virtual camera with a framing transposer.
 * This will flip the screenX and biasX of the transposer depending on the target velocity.
 */
public class FlipCamera : MonoBehaviour {
    [Range(0f, 20f)]
    public float flipSpeed = 5.0f;

    [Range(-1f, 1f)]
    private float flipT = 1.0f;

    private CinemachineFramingTransposer framing;
    private float initialScreenX, initialBiasX;

    private Rigidbody2D playerRB;

    // Start is called before the first frame update
    void Start() {
        CinemachineVirtualCamera vcam = GetComponent<CinemachineVirtualCamera>();
        framing = vcam.GetCinemachineComponent<CinemachineFramingTransposer>();

        // Save initial parameters
        initialScreenX = framing.m_ScreenX;
        initialBiasX = framing.m_BiasX;

        // Use player velocity to change where the camera is facing
        playerRB = framing.FollowTarget.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() {
        // If the player is moving, smoothly update internal camera orientation
        float vx = playerRB.velocity.x;
        if (Mathf.Abs(vx) > 0.001f)
            flipT = Mathf.Lerp(flipT, Mathf.Sign(vx), flipSpeed * Time.deltaTime);

        // Apply internal camera orientation to the cinemachine camera
        float clampedT = (flipT + 1f) / 2f;
        framing.m_ScreenX = Mathf.Lerp(1f - initialScreenX, initialScreenX, clampedT);
        framing.m_BiasX = Mathf.Lerp(-initialBiasX, initialBiasX, clampedT);
    }
}
