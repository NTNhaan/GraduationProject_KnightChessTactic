using UnityEngine;
using System;

public class DoorMover : MonoBehaviour
{
    public bool isLeftDoor;
    public float speed = 8f;

    public Action OnHitOtherDoor;

    private Rigidbody2D rb;
    private bool movingIn = false;
    private bool movingOut = false;
    private bool stopped = false;

    private Vector2 startPos;
    public float outSpeedMultiplier = 0.6f; 
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
    }

    private void FixedUpdate()
    {
        if (stopped) return;

        // Move In
        if (movingIn)
        {
            float dir = isLeftDoor ? 1f : -1f;
            rb.MovePosition(rb.position + Vector2.right * speed * dir * Time.fixedDeltaTime);
        }

        // Move Out
        if (movingOut)
        {
            float dir = isLeftDoor ? -1f : 1f;
            rb.MovePosition(rb.position + Vector2.right * speed * dir * Time.fixedDeltaTime);
        }
    }

    public void StartMoveIn()
    {
        stopped = false;
        movingIn = true;
        movingOut = false;
    }

    public void StartMoveOut()
    {
        stopped = false;
        movingIn = false;
        movingOut = true;
        speed *= outSpeedMultiplier;
    }

    public void Stop()
    {
        stopped = true;
        movingIn = false;
        movingOut = false;
        rb.linearVelocity = Vector2.zero;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Door")) return;
        if (stopped) return;

        Stop();
        OnHitOtherDoor?.Invoke();
    }

    public void ResetDoor()
    {
        stopped = false;
        movingIn = false;
        movingOut = false;

        transform.position = startPos;
        rb.linearVelocity = Vector2.zero;
    }
}