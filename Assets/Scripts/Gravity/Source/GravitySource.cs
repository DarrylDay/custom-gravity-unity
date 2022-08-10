using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectCloud
{
    public abstract class GravitySource : MonoBehaviour
    {
        public enum DrawType
        {
            None,
            Gizmo,
            GizmoSelected
        }

        public static IReadOnlyList<GravitySource> GlobalGravities => _globalGravities;
        private static List<GravitySource> _globalGravities = new List<GravitySource>();

        public enum TriggerType
        {
            BoxCollider,
            Global
        }

        [SerializeField] private DrawType _drawGravity = DrawType.GizmoSelected;
        [SerializeField] protected TriggerType _triggerMethod = TriggerType.BoxCollider;
        [SerializeField] protected float _strength = 5.22f;
        [SerializeField] protected bool _inverse = false;
        [SerializeField] protected float _cutoffDistance = 1f;
        [SerializeField] protected float _fullStrengthDistance = 0.5f;
        [SerializeField] protected bool _masterSource;

        public bool IsMasterSource => _masterSource;

        protected Color GizmoColor => _inverse ? new Color(0f, 0f, 1f, 0.5f) : new Color(1f, 0f, 0f, 0.5f);

        protected Vector3 _baseSize = Vector3.one;
        protected Vector3 _minSize = Vector3.zero;
        protected float _scale = 1f;
        protected BoxCollider _triggerBoxCollider;


#if UNITY_EDITOR
        protected virtual void OnDrawGizmos()
        {
            if (_drawGravity == DrawType.Gizmo)
            {
                InitTriggerMesh();
                DrawGizmos();
            }
        }

        protected virtual void OnDrawGizmosSelected()
        {
            if (_drawGravity == DrawType.GizmoSelected)
            {
                InitTriggerMesh();
                DrawGizmos();
            }
        }
#endif

        protected virtual void Awake()
        {
            if (_triggerBoxCollider == null) 
                _triggerBoxCollider = GetComponents<BoxCollider>().FirstOrDefault(x => x.isTrigger);

            if (_triggerMethod == TriggerType.Global)
                _globalGravities.Add(this);
        }

        protected virtual void OnDestroy()
        {
            if (_triggerMethod == TriggerType.Global)
                _globalGravities.Remove(this);
        }

        protected virtual void OnValidate()
        {
            Awake();

            if (_strength < 0f) _strength = 0f;
            if (_cutoffDistance < 0f) _cutoffDistance = 0f;

            if (_fullStrengthDistance > _cutoffDistance) _fullStrengthDistance = _cutoffDistance;
            else if (_fullStrengthDistance < 0) _fullStrengthDistance = 0f;

            if (_triggerMethod == TriggerType.BoxCollider)
            {
                if (_triggerBoxCollider == null) 
                    _triggerBoxCollider = gameObject.AddComponent<BoxCollider>();

                InitTriggerMesh();
            }
            else
            {
                if (_triggerBoxCollider != null)
                    DestroyImmediate(_triggerBoxCollider);
            }
        }

        public Vector3 CalculateGravity(Transform obj)
        {
            var vector = CalculateTransformVector(obj);
            var dir = vector.normalized;
            var distance = vector.magnitude;

            var strength = _strength;
            if (distance > _cutoffDistance)
            {
                return Vector3.zero;
            }
            else if (distance > _fullStrengthDistance)
            {
                strength *= distance.Remap(_fullStrengthDistance, _cutoffDistance, 1f, 0f);
            }

            if (_inverse) dir *= -1f;

            return dir * strength;
        }

        public bool IsPointEffected(Vector3 pos)
        {
            return _triggerMethod == TriggerType.BoxCollider ? _triggerBoxCollider.bounds.Contains(pos) : true;
        }

        protected virtual void DrawGizmos() { }

        protected abstract Vector3 CalculateTransformVector(Transform obj);

        protected virtual void ConfigureTriggerBoxCollider(BoxCollider collider)
        {
            collider.size = _minSize + new Vector3(
                    _baseSize.x * _cutoffDistance * _scale / transform.lossyScale.x,
                    _baseSize.y * _cutoffDistance * _scale / transform.lossyScale.y,
                    _baseSize.z * _cutoffDistance * _scale / transform.lossyScale.z);
        }


        private void InitTriggerMesh()
        {
            if (_triggerMethod == TriggerType.BoxCollider && _triggerBoxCollider != null)
            {
                _triggerBoxCollider.hideFlags = HideFlags.HideInInspector;
                _triggerBoxCollider.isTrigger = true;
                ConfigureTriggerBoxCollider(_triggerBoxCollider);
            }
        }

    }
}