using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterJump : MonoBehaviour
{

    private Rigidbody2D _rb;
    [SerializeField]private float fallSpeed;
    [SerializeField]private float lowJumpHeight;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (_rb.velocity.y < 0)
        {
            _rb.velocity += Vector2.up * Physics.gravity.y * (fallSpeed - 1) * Time.deltaTime;
        }
        else if (_rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            _rb.velocity += Vector2.up * Physics.gravity.y * (lowJumpHeight - 1) * Time.deltaTime;
        }
        
    }
}
