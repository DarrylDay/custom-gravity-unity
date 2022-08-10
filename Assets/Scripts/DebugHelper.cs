using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectCloud
{
    public static class DebugHelper
    {
        public static void DrawArrow(Vector3 pos, Vector3 direction, Color? color = null, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            var debugColor = color.HasValue ? color.Value : Color.red;
            Debug.DrawRay(pos, direction, debugColor);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Debug.DrawRay(pos + direction, right * arrowHeadLength, debugColor);
            Debug.DrawRay(pos + direction, left * arrowHeadLength, debugColor);
        }
    }
}