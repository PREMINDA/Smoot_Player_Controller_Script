using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
    [Header("Layers")]
    public LayerMask groundLayer;
    [SerializeField] private Bounds playerBounds;

    [Space]

    public bool onGround;
    public bool onWall;
    public bool onRightWall;
    public bool onLeftWall;
    public int wallSide;

    [Space]

    [Header("Collision")]

    public float collisionRadius = 0.25f;
    public Vector2 bottomOffset, rightOffset, leftOffset;
    private Color _debugCollisionColor = Color.red;
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        GetCorn();
        var position = transform.position;
        onGround = Physics2D.OverlapCircle( bottomOffset, collisionRadius, groundLayer);
        onWall = Physics2D.OverlapCircle(rightOffset, collisionRadius, groundLayer) 
                 || Physics2D.OverlapCircle(leftOffset, collisionRadius, groundLayer);

        onRightWall = Physics2D.OverlapCircle(rightOffset, collisionRadius, groundLayer);
        onLeftWall = Physics2D.OverlapCircle(leftOffset, collisionRadius, groundLayer);

        wallSide = onRightWall ? -1 : 1;
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        var position = transform.position;
        Gizmos.DrawWireSphere(bottomOffset, collisionRadius);
        Gizmos.DrawWireSphere(rightOffset, collisionRadius);
        Gizmos.DrawWireSphere(leftOffset, collisionRadius);
        
        Gizmos.DrawWireCube(position + playerBounds.center, playerBounds.size);
    }

    private void GetCorn()
    {
        var bound = new Bounds(transform.position, playerBounds.size);

        bottomOffset = new Vector2((bound.max.x+bound.min.x)/2,bound.min.y);
        rightOffset = new Vector2(bound.max.x, (bound.min.y+bound.max.y)/2);
        leftOffset = new Vector2(bound.min.x,(bound.min.y+bound.max.y)/2);

    }
}
