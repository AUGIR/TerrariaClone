using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public float jumpForce;
    public bool onGround;
    public Vector2 spawnPos;
    private float horizontal;
    private Rigidbody2D rb;
    private Animator anim;

    public void Spawn()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        GetComponent<Transform>().position = spawnPos;
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Ground"))
        {
            onGround = true;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Ground"))
        {
            onGround = false;
        }
    }

    private void FixedUpdate()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        float jump = Input.GetAxisRaw("Jump");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(horizontal * moveSpeed, rb.velocity.y);
        if (horizontal < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (horizontal > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        if (vertical > 0.1f || jump > 0.1f)
        {
            if (onGround)
            {
                movement.y = jumpForce;
            }

        }

        rb.velocity = movement;
    }

    private void Update()
    {
        anim.SetFloat("horizontal", horizontal);
    }
}
