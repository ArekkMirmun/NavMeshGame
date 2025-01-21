using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Character : MonoBehaviour
{
    public float speed = 5f;                // Base walking speed
    public float sprintMultiplier = 1.5f;  // Multiplier for sprinting speed
    public float acceleration = 5f;        // Rate of acceleration
    [SerializeField] private Rigidbody rb; // Reference to the player's Rigidbody
    [SerializeField] private Animator animator; // Reference to the Animator
    [SerializeField] private bool isSprinting = false; // Whether the player is sprinting

    private float _targetSpeed = 0f;      // Desired speed based on input
    private float _currentSpeed = 0f;    // Current speed (accelerates toward _targetSpeed)
    private float _xAxis = 0f;
    private float _zAxis = 0f;

    void Start()
    {
        _targetSpeed = 0f;
    }

    void FixedUpdate()
    {
        // Calculate movement input magnitude
        bool isMoving = _xAxis != 0 || _zAxis != 0;

        // Determine the target speed based on input and sprinting state
        _targetSpeed = isMoving ? (isSprinting ? speed * sprintMultiplier : speed) : 0f;

        // Gradually adjust the current speed toward the target speed
        _currentSpeed = Mathf.MoveTowards(_currentSpeed, _targetSpeed, acceleration * Time.fixedDeltaTime);

        // Calculate the movement vector
        Vector3 movement = new Vector3(_xAxis, 0, _zAxis).normalized * _currentSpeed;

        // Move the Rigidbody
        rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);

        // Update the Animator with the current speed
        animator.SetFloat("velocity", _currentSpeed);

        // Rotate the player to face the direction of movement
        if (isMoving)
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(_xAxis, 0, _zAxis));
        }
    }

    #region Movement

    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        _xAxis = input.x;
        _zAxis = input.y;
    }

    public void OnSprint(InputValue value)
    {
        isSprinting = value.isPressed;
    }

    #endregion
}
