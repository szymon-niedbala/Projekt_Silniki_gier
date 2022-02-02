using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Movement : MonoBehaviour
{
    [SerializeField] float Acceleration = 100f;
    [SerializeField] float MaxSpeed = 20;
    [SerializeField] float AirControlModifier = .5f;
    [SerializeField] float ConvinienceDelay = .2f;
    [SerializeField] float FallingSpeedTreshold = -1f;
    [SerializeField] float JumpHeight = 10f;
    [SerializeField] float jumpColdown = .5f;
    [SerializeField] float radious = .5f;
    [SerializeField] float CircleCastOffset = 0f;
    [SerializeField] LayerMask groundCheck;
    [SerializeField] float dashDistance = 2000f;
    [SerializeField] float dashCooldown = 2f;
    [SerializeField] float fullGravity = 5f;

    private Rigidbody2D rigidBody;
    private PlayerInput playerInput;
    private float inputMovement;
    private float lastDashTime = 0;
    private float lastJumpTime = 0f;
    private float lastTimeGroundTouch = 0;
    private bool isRight = true;

    public void OnMovement(InputAction.CallbackContext value)
    {
        inputMovement = value.ReadValue<float>();
    }
    private void Move(float _speed, float _maxSpeed)
    {
        var collider = Physics2D.OverlapCircleAll(rigidBody.position + new Vector2(0f, CircleCastOffset), radious, groundCheck);
        //Ograniczenie zmian szybkoœci na gracza kiedy jest w powietrzu
        _speed = collider.Length == 0
            ? Acceleration * AirControlModifier
            : Acceleration;
        rigidBody.velocity = new Vector2(Mathf.Clamp(rigidBody.velocity.x, -_maxSpeed, _maxSpeed), rigidBody.velocity.y);
            //Jeœli chce zmieniæ kierunek ruchu dodajemy odwrotn¹ prêdkoœæ do gracza ¿eby j¹ wyzerowaæ i wtedy pozosta³a prêdkoœæ jest dodana jak w przypadku kiedy zaczêliœmy stacjonarnie
            if (inputMovement > 0 && rigidBody.velocity.x < 0)
            {
                rigidBody.velocity += new Vector2(inputMovement * _speed * Time.deltaTime - rigidBody.velocity.x, 0f);
            }
            else if (inputMovement < 0 && rigidBody.velocity.x > 0)
            {
                rigidBody.velocity += new Vector2(inputMovement * _speed * Time.deltaTime, 0f);
            }
            else
            {
                rigidBody.velocity += new Vector2(inputMovement * _speed * Time.deltaTime, 0f);
            }
    }
    public void Flip()  
    {
        if (rigidBody.velocity.x < -.1 && isRight)
        {
            transform.Rotate(0, 180, 0);
            isRight = false;
        }
        if (rigidBody.velocity.x > .1f && !isRight)
        {
            transform.Rotate(0, 180, 0);
            isRight = true;
        }
    }

    public void OnJump(InputAction.CallbackContext value)
    {
        var collider = Physics2D.OverlapCircleAll(rigidBody.position + new Vector2(0f, CircleCastOffset), radious, groundCheck);
        if (value.started)
        {
            if(lastTimeGroundTouch + ConvinienceDelay > Time.time)
            {
                if (collider.Length > 0 || lastJumpTime + jumpColdown < Time.time)
                {
                    lastJumpTime = Time.time;
                    if (rigidBody.velocity.y < FallingSpeedTreshold)
                    {
                        rigidBody.AddForce(new Vector2(0f, JumpHeight), ForceMode2D.Impulse);
                    }
                    else
                    {
                        rigidBody.AddForce(new Vector2(0f, JumpHeight), ForceMode2D.Impulse);
                    }
                }
            }
        }
    }
    public void OnDash(InputAction.CallbackContext value)
    {
        var collider = Physics2D.OverlapCircleAll(rigidBody.position + new Vector2(0f, CircleCastOffset), radious, groundCheck);
        if (value.performed)
        {
            if (lastTimeGroundTouch + ConvinienceDelay > Time.time)
            {
                if (collider.Length > 0 || lastDashTime + dashCooldown < Time.time)
                {
                    lastDashTime = Time.time;
                    rigidBody.AddForce(new Vector2(inputMovement * dashDistance, 0f), ForceMode2D.Impulse);
                }
            }
        }
    }
    public void StickToSurface()
    {
        var collider = Physics2D.OverlapCircleAll(rigidBody.position + new Vector2(0f, CircleCastOffset), radious, groundCheck);
        if(collider.Length>0)
        {
            lastTimeGroundTouch = Time.time;
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, Mathf.Clamp(rigidBody.velocity.y, 0, JumpHeight));
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
        Move(Acceleration, MaxSpeed);
        Flip();
        StickToSurface();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(0f, CircleCastOffset), radious);
    }
}
