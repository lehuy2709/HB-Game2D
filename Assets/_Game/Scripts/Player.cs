using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [SerializeField] private Rigidbody2D rb;
    
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float speed = 5;
    [SerializeField] private float jumpForce = 350;
    [SerializeField] private Kunai kunaiPrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private GameObject attackArea;




    private bool isGrouned = true;
    private bool isJumping = false;
    private bool isAttack = false;
    private bool isDeath = false;

    private float horizontal;
    private int coin = 0;

    private Vector3 savePoint;


    private void Awake()
    {
        coin = PlayerPrefs.GetInt("coin", 0);
    }


    // Update is called once per frame
    void Update()
    {

        if (IsDead)
        {
            return;
        }
        isGrouned = CheckGrounded();
        //horizontal = Input.GetAxisRaw("Horizontal");

        if (isAttack)
        {
            return;
        }

        if (isGrouned)
        {
            if (isJumping)
            {
                rb.velocity = Vector2.zero;
                return;
            }
            // jump
            if (Input.GetKeyDown(KeyCode.Space) && isGrouned)
            {
                Jump();
            }

            // change anim run
            if (Mathf.Abs(horizontal) > 0.1f)
            {
                ChangeAnim("run");

            }

            // attack
            if (Input.GetKeyDown(KeyCode.C) && isGrouned)
            {
                Attack();
            }
            // throw
            if (Input.GetKeyDown(KeyCode.V) && isGrouned)
            {
                Throw();
            }
        }
            // check falling 
        if (!isGrouned && rb.velocity.y < 0)
        {
            ChangeAnim("fall");
            isJumping = false;
        }


        // Moving
        if (Mathf.Abs(horizontal) > 0.1f)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
            transform.rotation = Quaternion.Euler(new Vector3(0,horizontal > 0 ? 0 : 180,0));
            //transform.localScale = new Vector3(horizontal,1,1);

        }
        // idle
        else if (isGrouned)
        {
            ChangeAnim("idle");
            rb.velocity = Vector2.zero;
        }

    }

    public override void OnInit()
    {
        base.OnInit();
        isAttack = false;

        transform.position = savePoint;
        ChangeAnim("idle");
        DeActiveAttack();

        SavePoint();
        UIManager.instance.SetCoin(coin);
    }

    public override void Ondespawn()
    {
        base.Ondespawn();
        OnInit();
    }

    protected override void OnDeath()
    {
        base.OnDeath();
      
    }

    private bool CheckGrounded()
    {
        Debug.DrawLine(transform.position, transform.position + Vector3.down * 1.1f, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundLayer);

        return hit.collider != null;
    }

    public void Attack()
    {
        ChangeAnim("attack");
        isAttack = true;
        Invoke(nameof(ResetAttack), 0.5f);
        ActiveAttack();
        Invoke(nameof(DeActiveAttack), 0.5f);



    }
    public void Throw()
    {
        //rb.velocity = Vector2.zero;
        ChangeAnim("throw");
        isAttack = true;
        Invoke(nameof(ResetAttack), 0.5f);

        Instantiate(kunaiPrefab,throwPoint.position,throwPoint.rotation);
    }
    
    private void ResetAttack()
    {
        ChangeAnim("ilde");
        isAttack = false;
    }
    public void Jump()
    {
        isJumping = true;
        ChangeAnim("jump");
        rb.AddForce(jumpForce * Vector2.up);
    }

 
    internal void SavePoint()
    {
        savePoint = transform.position;

    }

    private void ActiveAttack()
    {
        attackArea.SetActive(true);
    }
    private void DeActiveAttack()
    {

        attackArea.SetActive(false);
    }

    public void SetMove(float horizontal)
    {
        this.horizontal = horizontal;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Coin") 
        {
            coin++;
            PlayerPrefs.SetInt("coin", coin);
            UIManager.instance.SetCoin(coin);
            Destroy(collision.gameObject);
        }

        if (collision.tag == "DeathZone")
        {
            ChangeAnim("die");

            Invoke(nameof(OnInit), 1f);
        }
    }

  
}
