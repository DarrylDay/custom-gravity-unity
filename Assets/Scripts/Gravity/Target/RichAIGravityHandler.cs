#if AstarPathfinding
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Pathfinding.Util;

namespace ProjectCloud {

    [RequireComponent(typeof(RichAI))]
    public class RichAIGravityHandler : GravityTarget
    {
        public bool AdjustRotation = true;

        private RichAI _richAI;

        private List<Vector3> _pathPositions = new List<Vector3>();

        protected override void Awake()
        {
            base.Awake();

            _richAI = GetComponent<RichAI>();
            _richAI.gravity = Vector3.zero;
            _richAI.updateRotation = true;
        }

        protected override void HandleGravityResult(Vector3 resultVector)
        {
            _richAI.gravity = resultVector;

            if (!_richAI.isStopped)
            {
                _richAI.GetRemainingPath(_pathPositions, out bool stale);

                var forward = transform.forward;
                var upward = -resultVector.normalized;

                if (!stale && _pathPositions.Count >= 2)
                {
                    forward = (_pathPositions[1] - _pathPositions[0]).normalized;
                }


                // Very close, goes crazy when entering bottom half of sphere
                _richAI.movementPlane = new GraphTransform(Matrix4x4.TRS(Vector3.zero, Quaternion.LookRotation(forward, upward), Vector3.one));


                //transform.rotation = Quaternion.LookRotation(forward, upward);
            }
        }
    }

}
#endif