using UnityEngine;

public class MixPlayerController : MonoBehaviour
{
    [SerializeField]private float speed;
    [SerializeField]private float jumpForce;

    private BoxCollider2D _bx;
    private Rigidbody2D _rb;
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float movX = Input.GetAxis("Horizontal");
        float movY = Input.GetAxis("Vertical");
        Walk(new Vector2(movX,movY));
        
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
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
