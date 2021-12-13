using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Movement : MonoBehaviour
{
    public float Speed = 100f;
    public float JumpHeight = 10f;

    private Rigidbody2D rigidBody;
    private PlayerInput playerInput;
    private float inputMovement;

    public void OnMovement(InputAction.CallbackContext value)
    {
        inputMovement = value.ReadValue<float>();
    }
    private void Move()
    {
        rigidBody.velocity = new Vector2(inputMovement * Speed * Time.deltaTime, rigidBody.velocity.y);

    }
    public void OnJump(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            rigidBody.AddForce(new Vector2(0f, JumpHeight), ForceMode2D.Impulse);
        }
    }
    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        if (rigidBody == null)
        {
            gameObject.AddComponent<Rigidbody2D>();
        }
        if (playerInput == null)
        {
            playerInput = gameObject.AddComponent<PlayerInput>();
        }
    }
    private void FixedUpdate()
    {
        Move();
    }
}
