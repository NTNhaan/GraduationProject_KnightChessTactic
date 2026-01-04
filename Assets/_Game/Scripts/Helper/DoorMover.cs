using UnityEngine;
using System;

public class DoorMover : MonoBehaviour
{
    public bool isLeftDoor;
    public float speed = 8f;

    public Action OnHitOtherDoor;

    private Rigidbody2D rb;
    private Collider2D doorCollider; // Cache collider để enable/disable
    private bool movingIn = false;
    private bool movingOut = false;
    private bool stopped = false;

    private Vector2 startPos;
    public Vector2 StartPos => startPos; // Expose startPos để LoadingFade có thể dùng
    public float outSpeedMultiplier = 0.6f;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        doorCollider = GetComponent<Collider2D>();
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
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void SetKinematic(bool isKinematic)
    {
        if (rb != null)
        {
            rb.bodyType = isKinematic ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;
        }
    }

    public void SetColliderEnabled(bool enabled)
    {
        if (doorCollider != null)
        {
            doorCollider.enabled = enabled;
        }
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