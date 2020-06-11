using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Simple floating motion for an object.
public class Float : MonoBehaviour {
    [SerializeField] private float _amplitude = 0.3f;
    [SerializeField] private float _frequency = 1f;
    [SerializeField] private float _waveIntensity = 3f;

    private float _sineWave;
    
    void Update() {
        // sine wave y = Sin(2 * pi * frequency * time + phase constant), but phase constant can be 0
        _sineWave = Mathf.Sin(2 * Mathf.PI * _frequency * Time.fixedTime) * _amplitude;

        transform.position += new Vector3(0, _sineWave, 0f);

        // _sineWave used to match up the rotation with the wave, wave intensity used as a modifier
        transform.rotation = Quaternion.Euler(0, 0, _sineWave * _waveIntensity);
    }
}

