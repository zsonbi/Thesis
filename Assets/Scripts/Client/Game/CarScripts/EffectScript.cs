using DataTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectScript : ThreadSafeMonoBehaviour
{
    [SerializeField]
    private EffectType effectType;

    public EffectType EffectType => effectType;
}