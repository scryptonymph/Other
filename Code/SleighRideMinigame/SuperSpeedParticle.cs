using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SleighRide {
    public class SuperSpeedParticle : MonoBehaviour {

        public ParticleSystem particle;
        public Rigidbody2D rb;
        
        // Use this for initialization
        void Start() {
            particle = GetComponentInChildren<ParticleSystem>();
            rb = GetComponentInParent<Rigidbody2D>();
            particle.Stop();
        }

        // Update is called once per frame
        void Update() {
            if(rb.velocity.x >= 25) {
                particle.Play();
            }
            if(rb.velocity.x < 25) {
                particle.Stop();
            }

        }
    }
}
