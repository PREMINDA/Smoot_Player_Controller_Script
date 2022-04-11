using System.Collections.Generic;
using TarodevController;
using UnityEngine;

public class MixPlayerController : MonoBehaviour
{
    [SerializeField]private float speed;
    [SerializeField]private float jumpForce;

    private BoxCollider2D _bx;
    private Rigidbody2D _rb;
    private Collision _collision;
    
    public float slideSpeed = 5;
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collision = GetComponent<Collision>();
    }

    private void Update()
    {
        float movX = Input.GetAxis("Horizontal");
        float movY = Input.GetAxis("Vertical");
        Walk(new Vector2(movX,movY));

        if (WallGrabe())
        {
            _rb.gravityScale = 0f;
            Debug.Log(_rb.velocity);
            _rb.velocity = new Vector2(_rb.velocity.x, movY * slideSpeed);
        }
        else
        {
            _rb.gravityScale = 1.7f;
        }

        if (_collision.onWall && !_collision.onGround && !Input.GetKey(KeyCode.LeftShift))
        {
            WallSlide();
        }
        

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }
    private bool WallGrabe()
    {
        return _collision.onWall && Input.GetKey(KeyCode.LeftShift);
    }
    private void WallSlide()
    {
        
        _rb.velocity = new Vector2(_rb.velocity.x, -slideSpeed);
    }
    void Walk(Vector2 dir)
    {
        _rb.velocity = new Vector2(dir.x*speed*Time.deltaTime+dir.x*speed,_rb.velocity.y);
    }
    void Jump()
    {
        var velocity = _rb.velocity;
        velocity = new Vector2(velocity.x, 0);
        velocity += Vector2.up * jumpForce;
        _rb.velocity = velocity;
    }
    
}
