using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireworkSound : MonoBehaviour
{
    private ParticleSystem  _parentParticleSystem;

    private int _currentNumberOfParticles = 0;

    public AudioClip[] BornSounds;
    public AudioClip[] DieSounds;


    // Start is called before the first frame update
    void Start()
    {
        _parentParticleSystem = this.GetComponent<ParticleSystem>();
        if(_parentParticleSystem == null)
            Debug.LogError("Missing ParticleSystem!", this);
        
    }

    // Update is called once per frame
    void Update()
    {
        var amount = Mathf.Abs(_currentNumberOfParticles - _parentParticleSystem.particleCount);

        // if (_parentParticleSystem.particleCount < _currentNumberOfParticles) 
        // { 
        //     StartCoroutine(PlaySound(DieSounds[Random.Range(0, DieSounds.Length)], amount));
        // } 

        if (_parentParticleSystem.particleCount > _currentNumberOfParticles) 
        { 
            GameManager.Instance.AudioManager.Instantplay(SoundName.Firework, Vector3.zero);
        } 

        _currentNumberOfParticles = _parentParticleSystem.particleCount;
    }
}
