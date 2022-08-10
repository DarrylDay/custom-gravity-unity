using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectCloud
{
    public class GravityPoint : GravitySource
    {
        private Mesh _sphereMesh;

        protected override void DrawGizmos()
        {
            InitMesh();

            Gizmos.color = GizmoColor;

            Gizmos.DrawMesh(
                _sphereMesh, 
                transform.position, 
                transform.rotation, 
                Vector3.one * _fullStrengthDistance);

            Gizmos.DrawWireMesh(
                _sphereMesh, 
                transform.position, 
                transform.rotation,
                Vector3.one * _cutoffDistance);

            if (!_inverse)
            {
                GizmoHelper.DrawArrow(transform.position + (transform.up * (_cutoffDistance + (_cutoffDistance / 4f))), -transform.up, GizmoColor, _cutoffDistance / 4f);
                GizmoHelper.DrawArrow(transform.position + (transform.forward * (_cutoffDistance + (_cutoffDistance / 4f))), -transform.forward, GizmoColor, _cutoffDistance / 4f);
                GizmoHelper.DrawArrow(transform.position + (transform.right * (_cutoffDistance + (_cutoffDistance / 4f))), -transform.right, GizmoColor, _cutoffDistance / 4f);
                GizmoHelper.DrawArrow(transform.position + (-transform.up * (_cutoffDistance + (_cutoffDistance / 4f))), transform.up, GizmoColor, _cutoffDistance / 4f);
                GizmoHelper.DrawArrow(transform.position + (-transform.forward * (_cutoffDistance + (_cutoffDistance / 4f))), transform.forward, GizmoColor, _cutoffDistance / 4f);
                GizmoHelper.DrawArrow(transform.position + (-transform.right * (_cutoffDistance + (_cutoffDistance / 4f))), transform.right, GizmoColor, _cutoffDistance / 4f);
            } 
            else
            {
                GizmoHelper.DrawArrow(transform.position + (transform.up * _cutoffDistance), transform.up, GizmoColor, _cutoffDistance / 4f);
                GizmoHelper.DrawArrow(transform.position + (transform.forward * _cutoffDistance), transform.forward, GizmoColor, _cutoffDistance / 4f);
                GizmoHelper.DrawArrow(transform.position + (transform.right * _cutoffDistance), transform.right, GizmoColor, _cutoffDistance / 4f);
                GizmoHelper.DrawArrow(transform.position + (-transform.up * _cutoffDistance), -transform.up, GizmoColor, _cutoffDistance / 4f);
                GizmoHelper.DrawArrow(transform.position + (-transform.forward * _cutoffDistance), -transform.forward, GizmoColor, _cutoffDistance / 4f);
                GizmoHelper.DrawArrow(transform.position + (-transform.right * _cutoffDistance), -transform.right, GizmoColor, _cutoffDistance / 4f);
            }
                
        }

        protected override void Awake()
        {
            base.Awake();

            InitMesh();
        }

        private void InitMesh()
        {
            if (_sphereMesh == null) _sphereMesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
            _baseSize = _sphereMesh.bounds.size;
        }

        protected override Vector3 CalculateTransformVector(Transform obj)
        {
            return transform.position - obj.position;
        }
    }
}