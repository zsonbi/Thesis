using DataTypes;
using UnityEngine;
using Utility;

namespace Game
{
    /// <summary>
    /// Handles the car's effects
    /// </summary>
    public class EffectScript : ThreadSafeMonoBehaviour
    {
        [SerializeField]
        private EffectType effectType;

        public EffectType EffectType => effectType;
    }
}