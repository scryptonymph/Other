using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rekiviskaus {
    public class SuperSpeedParticle_2 : MonoBehaviour {

        public ParticleSystem particle;
        Rigidbody2D rb;
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
