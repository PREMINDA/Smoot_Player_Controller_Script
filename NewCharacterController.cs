using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace SmoothPlayer
{
    public class NewCharacterController : MonoBehaviour
    {
        [SerializeField] private Bounds playerBounds;
        [SerializeField] [Range(0.1f, 0.3f)] private float rayBuffer = 0.1f;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float detectionRayLength = 0.1f;
        [SerializeField] private int detectorCount = 3;

        public bool landingThisFrame;
        
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

        private void CollisionCheck()
        {
            CalculateRayRanged();

            landingThisFrame = false;
            
            
        }
        
        private bool RunDetection(RayRange range) {
            return EvaluateRayPositions(range).Any(point => Physics2D.Raycast(point, range.Dir, detectionRayLength, groundLayer));
        }
        
        private IEnumerable<Vector2> EvaluateRayPositions(RayRange range) {
            for (var i = 0; i < detectorCount; i++) {
                var t = (float)i / (detectorCount - 1);
                yield return Vector2.Lerp(range.Start, range.End, t);
            }
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
