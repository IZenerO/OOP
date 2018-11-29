using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.SceneManagement;
using System;


public class Character : Unit {

    [SerializeField]
    private float speed = 3.0F;
    [SerializeField]
    private float lives = 5;
    [SerializeField]
    private float jumpForce = 100.0F;

    private int _prevTimeStamp = 0;
    
    [SerializeField]
    private Bullet bullet;

    new private Rigidbody2D rigidbody;
    private Animator animator;
    private SpriteRenderer sprite;

    private bool isGrounded = false;

    private CharState State
    {
        get { return (CharState)animator.GetInteger("State"); }
        set { animator.SetInteger("State", (int)value); }
    }
    private void FixedUpdate()
    {
     CheckGround();
    }
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (isGrounded) State = CharState.Idle;
        if (Input.GetButtonDown("Fire1")) Shoot();
        if (Input.GetButton("Horizontal")) Run();
        if (isGrounded && Input.GetButton("Jump")) Jump();
    }

   private void Run()
    {
        Vector3 direction = transform.right * Input.GetAxis("Horizontal");

        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);

        sprite.flipX = direction.x < 0.0F;

        if (isGrounded) State = CharState.Run;
    }

    private void Jump()
    {
        rigidbody.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);        
    }

    private void Shoot()
    {
        Vector3 position = transform.position; position.y += 0.8F; position.x += 0.5F * (sprite.flipX ? -1.0F : 1.0F);
        Bullet newBullet = Instantiate(bullet, position, bullet.transform.rotation) as Bullet;
        

        newBullet.Parent = gameObject;
        newBullet.Direction = newBullet.transform.right * (sprite.flipX ? -1.0F : 1.0F);
        
    }

    public override void ReceiveDamage()
    {
        var timeStamp = Environment.TickCount;
        _prevTimeStamp = _prevTimeStamp == 0 ? timeStamp : _prevTimeStamp;

        if (TimeSpan.FromTicks(timeStamp - _prevTimeStamp).TotalMilliseconds < 0.1F && _prevTimeStamp != timeStamp)
        {
            return;
        }
        
        _prevTimeStamp = timeStamp;

        lives--;
        Debug.Log(lives);
        
  
        rigidbody.velocity = Vector3.zero;
        rigidbody.AddForce(transform.up * 9.0F, ForceMode2D.Impulse);
        rigidbody.AddForce(transform.right * (sprite.flipX ? -1.0F : 1.0F) * speed * -9.0F, ForceMode2D.Impulse);

    }

    private void CheckGround()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.3F);

        isGrounded = colliders.Length > 1;
        
        if (!isGrounded) State = CharState.Jump;
        
    }
    
    private void OnTriggerEnter2D(Collider2D collider)
    {
        Unit unit = collider.gameObject.GetComponent<Unit>();
        if (unit) ReceiveDamage();
        if (lives == 0)
        {
            SceneManager.LoadScene("Start");
            // Application.LoadLevel(Application.loadedLevel);
        }
    }

}


public enum CharState
{
    Idle,
    Run,
    Jump,
}