using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectCloud
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(Collider))]
    public class GravityMesh : GravitySource
    {
        [SerializeField] private bool _drawSurfaceLines = true;

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private Collider _collider;

        private Mesh _cutoffMesh;
        private Mesh _fullStrengthMesh;

        private Mesh _prevMesh;
        private float _prevCutoff = -1f;
        private float _prevFullStrength = -1f;
        private Vector3 _prevScale;

        protected override void DrawGizmos()
        {
            InitMesh();

            if (_drawSurfaceLines)
            {
                Gizmos.color = GizmoColor;
                Gizmos.DrawMesh(_cutoffMesh, transform.position, transform.rotation, Vector3.one);

                Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
                Gizmos.DrawMesh(_fullStrengthMesh, transform.position, transform.rotation, Vector3.one);
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            Awake();
        }

        protected override void Awake()
        {
            base.Awake();

            if (_meshFilter == null) _meshFilter = GetComponent<MeshFilter>();
            if (_meshRenderer == null) _meshRenderer = GetComponent<MeshRenderer>();

            if (_collider == null)
            {
                var colliders = GetComponents<Collider>();
                foreach (var collider in colliders)
                {
                    if (!collider.isTrigger) _collider = collider;
                }
                if (_collider == null) Debug.LogError("Missing Non Trigger Collider! Please attach one.");
            }

            InitMesh();
        }

        protected override Vector3 CalculateTransformVector(Transform obj)
        {
            return _collider.ClosestPoint(obj.position) - obj.position;
        }

        private void InitMesh()
        {
            if (_cutoffMesh == null) _cutoffMesh = new Mesh();
            if (_fullStrengthMesh == null) _fullStrengthMesh = new Mesh();

            if (_meshFilter != null && _meshFilter.sharedMesh != null)
            {
                var mesh = _meshFilter.sharedMesh;

                _baseSize = Vector3.one;
                _minSize = mesh.bounds.size;
                _scale = 2f;

                if (_prevMesh != mesh || _prevCutoff != _cutoffDistance || _prevScale != transform.lossyScale)
                {
                    UpdateLinesMesh(_cutoffMesh, _cutoffDistance);
                }

                if (_prevMesh != mesh || _prevFullStrength != _fullStrengthDistance || _prevScale != transform.lossyScale)
                {
                    UpdateLinesMesh(_fullStrengthMesh, _fullStrengthDistance);
                }

                _prevMesh = mesh;
                _prevScale = transform.lossyScale;
            }
        }

        private void UpdateLinesMesh(Mesh linesMesh, float offset)
        {
            var mesh = _meshFilter.sharedMesh;
            var normals = mesh.normals;
            var verts = mesh.vertices;
            var tris = mesh.triangles;

            var indiciesCount = 0;
            var linesIndicies = new List<int>();
            var linesVerts = new List<Vector3>();
            var linesNormals = new List<Vector3>();

            for (int i = 0; i < tris.Count(); i += 3)
            {
                var center = (verts[tris[i]] + verts[tris[i + 1]] + verts[tris[i + 2]]) / 3f;
                var normal = (normals[tris[i]] + normals[tris[i + 1]] + normals[tris[i + 2]]) / 3f;
                var scaledCenter = Vector3.Scale(center, transform.lossyScale);
                linesVerts.Add(scaledCenter);
                linesVerts.Add(scaledCenter + (normal * offset));
                linesNormals.Add(normal);
                linesNormals.Add(normal);
                linesIndicies.Add(indiciesCount++);
                linesIndicies.Add(indiciesCount++);
            }

            linesMesh.vertices = linesVerts.ToArray();
            linesMesh.SetIndices(linesIndicies.ToArray(), MeshTopology.Lines, 0);
            linesMesh.normals = linesNormals.ToArray();
        }
    }

}