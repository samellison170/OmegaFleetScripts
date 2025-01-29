using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class CinemachineSwitchScript : MonoBehaviour
{
    [SerializeField] private Animator stateDrivenAnimator; // Reference to the Animator controlling the SDC
    [SerializeField] private string stateOneName = "Overview State"; // First camera state
    [SerializeField] private string stateTwoName = "Level Select State"; // Second camera state
    [SerializeField] private CinemachineVirtualCamera vCam1;//Overview camera
    [SerializeField] private CinemachineVirtualCamera vCam2;//Level select camera
    private bool isInStateOne = true;  // Track the current state
    private float tapTimeMax = 0.3f;  // Maximum time interval for a double tap
    private float lastTapTime = 0f;   // Time of the last tap

    void Update()
    {
        // Check if there is a touch input
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == UnityEngine.TouchPhase.Ended)
            {
                float currentTime = Time.time;

                // Check for double tap
                if (currentTime - lastTapTime <= tapTimeMax)
                {
                    ToggleCameraState();
                    //Debug.Log("double tap detected");
                }

                lastTapTime = currentTime; // Update the last tap time
            }
        }
    }

    private void ToggleCameraState()
    {
        if (stateDrivenAnimator != null)
        {
            // Toggle between the two states
            if (isInStateOne)
            {
                //Debug.Log("switch to state 1");
                stateDrivenAnimator.Play(stateTwoName);
                //vCam1.Priority = 1; vCam2.Priority = 0;
            }
            else
            {
                Debug.Log("switch to state 2");
                stateDrivenAnimator.Play(stateOneName);
                //vCam1.Priority = 0; vCam2.Priority = 1;
            }

            isInStateOne = !isInStateOne; // Flip the state tracker
        }
        else
        {
            Debug.LogWarning("Animator is not set!");
        }
    }
}