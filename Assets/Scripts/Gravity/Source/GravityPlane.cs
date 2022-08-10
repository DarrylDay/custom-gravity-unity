using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectCloud
{
    public class GravityPlane : GravitySource
    {
        private Mesh _boxMesh;

        protected override void DrawGizmos()
        {
            InitMesh();

            Gizmos.color = GizmoColor;

            Gizmos.DrawMesh(
                _boxMesh,
                transform.position - (transform.forward * (_fullStrengthDistance / 2f)),
                transform.rotation,
                new Vector3(transform.localScale.x, transform.localScale.y, _fullStrengthDistance));

            Gizmos.DrawWireMesh(
                _boxMesh,
                transform.position - (transform.forward * (_cutoffDistance / 2f)),
                transform.rotation,
                new Vector3(transform.localScale.x, transform.localScale.y, _cutoffDistance));

            if (!_inverse)
            {
                GizmoHelper.DrawArrow(transform.position - (transform.forward * (_cutoffDistance / 2f)), transform.forward, GizmoColor, _cutoffDistance / 2f);
            }
            else
            {
                GizmoHelper.DrawArrow(transform.position, -transform.forward, GizmoColor, _cutoffDistance / 2f);
            }
        }

        protected override void Awake()
        {
            base.Awake();

            InitMesh();
        }

        private void InitMesh()
        {
            if (_boxMesh == null) _boxMesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            _baseSize = _boxMesh.bounds.size;
        }

        protected override Vector3 CalculateTransformVector(Transform obj)
        {
            var vector = transform.position - obj.position;
            var dir = transform.forward;
            var distance = Vector3.Dot(vector, transform.forward);

            if (distance < 0) dir *= -1f;

            return dir * distance;
        }

        protected override void ConfigureTriggerBoxCollider(BoxCollider collider)
        {
            collider.center = new Vector3(0f, 0f, -_cutoffDistance / 2f / transform.lossyScale.z);
            collider.size = new Vector3(1f, 1f, _cutoffDistance / transform.lossyScale.z);
        }
    }
}