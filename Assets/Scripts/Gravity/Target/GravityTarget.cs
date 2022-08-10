using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace ProjectCloud
{
    [ExecuteInEditMode]
    public abstract class GravityTarget : MonoBehaviour
    {
        public enum DrawType
        {
            None,
            Gizmo,
            GizmoSelected,
            Debug
        }

        [SerializeField] private float _minGravityAngleChange = 0.1f;
        public UnityEvent<Vector3> OnGravityAngleChange;

        [Header("Editor")]
        [SerializeField] private bool _updateInEditor = true;
        [SerializeField] private DrawType _drawGravity = DrawType.GizmoSelected;

        public Vector3 CurrentGravity { get; protected set; }
        public Vector3 UpVector => CurrentGravity * -1f;

        private List<GravitySource> _gravitySources = new List<GravitySource>();
        private List<Vector3> _currentGravityVectors = new List<Vector3>();
        private Vector3 _prevPos;

        protected virtual void Awake()
        {
            _prevPos = transform.position;
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && _updateInEditor)
            {
                if (_prevPos != transform.position)
                {
                    _gravitySources = FindObjectsOfType<GravitySource>().Where(x => x.IsPointEffected(transform.position)).ToList();

                    FixedUpdate();

                    _prevPos = transform.position;
                }
            }
#endif
        }

        protected void FixedUpdate()
        {
            var gravitySources = _gravitySources.ToList();

            // Add global sources
            gravitySources.AddRange(GravitySource.GlobalGravities);

            // Remove null sources
            gravitySources = gravitySources.Where(x => x != null).ToList();

            // Check for any master sources
            var masterSources = gravitySources.Where(x => x.IsMasterSource).ToList();

            gravitySources = masterSources.Count > 0 ? new List<GravitySource>() { masterSources.First() } : gravitySources;

            // Calculate vectors
            _currentGravityVectors.Clear();
            gravitySources.ForEach(x =>
            {
                var vector = x.CalculateGravity(transform);

                // Get non zero vectors
                if (vector != Vector3.zero)
                    _currentGravityVectors.Add(vector);
            });

            var resultVector = Vector3.zero;

            if (_currentGravityVectors.Count == 0)
            {
                if (GravityManager.Instance != null && GravityManager.Instance.GlobalGravityEffect == GravityManager.GlobalGravityEffectType.NoGravitySources)
                    resultVector = GravityManager.Instance.GlobalGravity;
            }
            else
            {
                // Sum results
                _currentGravityVectors.ForEach(x => resultVector += x);

                if (GravityManager.Instance != null && GravityManager.Instance.GlobalGravityEffect == GravityManager.GlobalGravityEffectType.Always)
                    resultVector += GravityManager.Instance.GlobalGravity;
            }

            if (Application.isPlaying)
                HandleGravityResult(resultVector);
            else
                transform.rotation = Quaternion.FromToRotation(transform.up, resultVector.normalized * -1f) * transform.rotation;

            if (Vector3.Angle(CurrentGravity, resultVector) > _minGravityAngleChange)
            {
                OnGravityAngleChange?.Invoke(CurrentGravity);
            }

            CurrentGravity = resultVector;

            if (_drawGravity == DrawType.Debug)
                DebugHelper.DrawArrow(transform.position, CurrentGravity.normalized);
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.isTrigger && other.TryGetComponent(out GravitySource gravitySource))
            {
                if (!_gravitySources.Contains(gravitySource))
                    _gravitySources.Add(gravitySource);
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (other.isTrigger && other.TryGetComponent(out GravitySource gravitySource))
            {
                if (_gravitySources.Contains(gravitySource))
                    _gravitySources.Remove(gravitySource);
            }
        }

        protected abstract void HandleGravityResult(Vector3 resultVector);

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_drawGravity == DrawType.Gizmo)
                DrawGizmoGravity();
        }

        private void OnDrawGizmosSelected()
        {
            if (_drawGravity == DrawType.GizmoSelected)
                DrawGizmoGravity();
        }

        private void DrawGizmoGravity()
        {
            if (_currentGravityVectors.Count > 1)
            {
                foreach (var gravity in _currentGravityVectors)
                {
                    GizmoHelper.DrawArrow(transform.position, gravity.normalized, Color.blue, 1f);
                    UnityEditor.Handles.Label(transform.position + gravity.normalized, gravity.magnitude.ToString());
                }
            }

            if (CurrentGravity != Vector3.zero)
            {
                GizmoHelper.DrawArrow(transform.position, CurrentGravity.normalized * 2f, Color.red, 1f);
                UnityEditor.Handles.Label(transform.position + (CurrentGravity.normalized * 2f), CurrentGravity.magnitude.ToString());
            }
        }
#endif

    }
}