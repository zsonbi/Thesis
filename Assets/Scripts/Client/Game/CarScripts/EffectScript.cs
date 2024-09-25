using DataTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectScript : MonoBehaviour
{
    private ParticleSystem particleSystem;

    [SerializeField]
    private EffectType effectType;

    public EffectType EffectType => effectType;

    // Runs when the sccript is loaded
    private void Awake()
    {
        TryGetComponent<ParticleSystem>(out particleSystem);
    }
}