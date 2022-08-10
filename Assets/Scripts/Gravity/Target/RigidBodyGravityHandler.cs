using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectCloud
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class RigidBodyGravityHandler : GravityTarget
    {
        private Rigidbody _rigidbody;

        protected override void Awake()
        {
            base.Awake();

            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.useGravity = false;
        }

        protected override void HandleGravityResult(Vector3 resultVector)
        {
            _rigidbody.AddForce(resultVector, ForceMode.Acceleration);
        }
    }
}