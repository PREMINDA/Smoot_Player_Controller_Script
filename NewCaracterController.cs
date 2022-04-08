using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SmoothPlayer
{
    public class NewCaracterController : MonoBehaviour
    {
        [SerializeField] private Bounds playerBounds;
        [SerializeField] [Range(0.1f, 0.3f)] private float rayBuffer = 0.1f;
        
        private RayRange _raysUp, _raysRight, _raysDown, _raysLeft;

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            CalculateRayRanged();
            
            Debug.DrawRay(_raysUp.Start,_raysUp.Dir,Color.green);
            Debug.DrawRay(_raysUp.End,_raysUp.Dir,Color.green);
            
            Debug.DrawRay(_raysDown.Start,_raysDown.Dir,Color.blue);
            Debug.DrawRay(_raysDown.End,_raysDown.Dir,Color.blue);
            
            Debug.DrawRay(_raysLeft.Start,_raysLeft.Dir,Color.magenta);
            Debug.DrawRay(_raysLeft.End,_raysLeft.Dir,Color.magenta);
            
            Debug.DrawRay(_raysRight.Start,_raysRight.Dir,Color.yellow);
            Debug.DrawRay(_raysRight.End,_raysRight.Dir,Color.yellow);
            
        }

        private void CalculateRayRanged()
        {
            var bound = new Bounds(transform.position, playerBounds.size);

            _raysDown = new RayRange(bound.min.x + rayBuffer, bound.min.y, bound.max.x - rayBuffer, bound.min.y, Vector2.down);
            _raysUp = new RayRange(bound.min.x + rayBuffer, bound.max.y, bound.max.x - rayBuffer, bound.max.y, Vector2.up);
            _raysLeft = new RayRange(bound.min.x, bound.min.y + rayBuffer, bound.min.x, bound.max.y - rayBuffer, Vector2.left);
            _raysRight = new RayRange(bound.max.x, bound.min.y + rayBuffer, bound.max.x, bound.max.y - rayBuffer, Vector2.right);

        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + playerBounds.center, playerBounds.size);
        }
    }
}
