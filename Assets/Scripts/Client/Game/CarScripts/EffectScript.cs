using DataTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class EffectScript : MonoBehaviour
{
    private ParticleSystem particleSystem;

    [SerializeField]
    private EffectType effectType;

    public EffectType EffectType => effectType;

    // Runs when the sccript is loaded
    void Awake()
    {
        this.particleSystem = GetComponent<ParticleSystem>();
    }

    
}
