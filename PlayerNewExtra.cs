using UnityEngine;

namespace SmoothPlayer
{
    public struct RayRange {
        public RayRange(float x1, float y1, float x2, float y2, Vector2 dir) {
            Start = new Vector2(x1, y1);
            End = new Vector2(x2, y2);
            Dir = dir;
        }

        public readonly Vector2 Start, End, Dir;
    }

     public struct PlayerFrameInput {
        public float X,Y;
        public bool JumpDown;
        public bool JumpUp;
    }
}