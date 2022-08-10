using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectCloud
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class GravityTargetRelay : MonoBehaviour
    {
        [SerializeField] private GravityTarget _relayTarget;

        public void OnTriggerEnter(Collider other)
        {
            _relayTarget.OnTriggerEnter(other);
        }

        public void OnTriggerExit(Collider other)
        {
            _relayTarget.OnTriggerExit(other);
        }
    }
}

