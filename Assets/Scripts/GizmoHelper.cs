using UnityEngine;
using System.Collections;
 
namespace ProjectCloud
{
    public static class GizmoHelper
    {
        public static void DrawArrow(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength, float arrowHeadAngle)
        {
            Gizmos.color = color;
            Gizmos.DrawRay(pos, direction);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
        }

        public static void DrawArrow(Vector3 pos, Vector3 direction, Color? color = null, float scale = 1f)
        {
            var arrowHeadAngle = 20.0f;
            var arrowHeadLength = 0.25f * scale;

            Gizmos.color = color.HasValue ? color.Value : Color.red;
            Gizmos.DrawRay(pos, direction * scale);

            Vector3 right = Quaternion.LookRotation(direction * scale) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction * scale) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(pos + (direction * scale), right * arrowHeadLength);
            Gizmos.DrawRay(pos + (direction * scale), left * arrowHeadLength);
        }
    }
}