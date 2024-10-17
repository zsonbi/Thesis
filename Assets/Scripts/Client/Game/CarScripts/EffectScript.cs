using DataTypes;
using UnityEngine;
using Utility;

namespace Game
{
    public class EffectScript : ThreadSafeMonoBehaviour
    {
        [SerializeField]
        private EffectType effectType;

        public EffectType EffectType => effectType;
    }
}