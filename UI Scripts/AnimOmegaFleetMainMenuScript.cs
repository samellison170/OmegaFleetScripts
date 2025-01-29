using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimOmegaFleetMainMenuScript : MonoBehaviour
{
    public Transform pointA;    // Starting point
    public Transform pointB;    // Target point
    public float duration = 2f; // Duration for each movement
    public Button moveButton;   // Button to trigger movement
    public float hoverDistance = 0.2f; // Distance for hover effect
    public float hoverSpeed = 1f; // Speed for hover effect

    //private bool movingToB = true; // Controls the movement direction

    void Start()
    {
        // Set up the button click event to trigger the first movement
        moveButton.onClick.AddListener(OnButtonPress);

        // Start at pointA with easing and hover effect
        MoveToPointA();
    }

    void MoveToPointA()
    {
        // Move to pointA with an ease-in effect
        LeanTween.move(gameObject, pointA.position, duration).setEase(LeanTweenType.easeInOutCubic).setOnComplete(StartHover);
    }

    void MoveToPointB()
    {
        // Move to pointB when button is pressed
        LeanTween.move(gameObject, pointB.position, duration).setEase(LeanTweenType.easeInOutCubic);
    }

    void OnButtonPress()
    {
        // Stop hover when moving away from pointA
        LeanTween.cancel(gameObject);
        MoveToPointB();
    }

    void StartHover()
    {
        // Apply a continuous hover effect at pointA
        LeanTween.moveY(gameObject, pointA.position.y + hoverDistance, hoverSpeed).setEase(LeanTweenType.easeInOutSine).setLoopPingPong();
    }
}
