using DataTypes;
using UnityEngine;

namespace Game
{
    public class EffectScript : ThreadSafeMonoBehaviour
    {
        [SerializeField]
        private EffectType effectType;

        public EffectType EffectType => effectType;
    }
}