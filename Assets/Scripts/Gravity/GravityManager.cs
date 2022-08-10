using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectCloud
{
    public class GravityManager : Singleton<GravityManager>
    {
        public enum GlobalGravityEffectType
        {
            Always,
            NoGravitySources
        }

        public Vector3 GlobalGravity = Vector3.zero;
        public GlobalGravityEffectType GlobalGravityEffect = GlobalGravityEffectType.NoGravitySources;
    }
}