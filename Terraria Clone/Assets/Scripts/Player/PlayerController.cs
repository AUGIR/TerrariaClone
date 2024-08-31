using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public TileClass selectedTile;
    public Vector2Int mousePos;
    public float moveSpeed;
    public float jumpForce;
    public bool onGround;
    public Vector2 spawnPos;
    public bool place;
    public bool hit;
    public int reachRange;
    public ProceduralGeneration proceduralGenerator;

    public Camera cam;
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

        hit = Input.GetMouseButton(0);
        place = Input.GetMouseButton(1);

        if (Vector2.Distance(transform.position, mousePos) <= reachRange)
        {
            if (hit)
            {
                proceduralGenerator.RemoveTile(mousePos.x, mousePos.y);
            }
            else if (place)
            {
                proceduralGenerator.CheckTile(selectedTile, mousePos.x, mousePos.y, false);
            }
        }


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
        mousePos.x = Mathf.RoundToInt(cam.ScreenToWorldPoint(Input.mousePosition).x - 0.5f);
        mousePos.y = Mathf.RoundToInt(cam.ScreenToWorldPoint(Input.mousePosition).y - 0.5f);
        anim.SetFloat("horizontal", horizontal);
        anim.SetBool("hit", hit || place);
    }

}
