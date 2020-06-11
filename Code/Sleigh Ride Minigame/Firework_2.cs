using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rekiviskaus {
    public class Firework_2 : MonoBehaviour {

        private GameManager_2 _gameManager;
        private SoundManager_2 _soundManager;

        [SerializeField] private Transform _player;

        [SerializeField] private float _followDistance;

        [SerializeField] private float _followSpeed;

        private ParticleSystem _particle;

        private float _distance;
        private float _multiplierX;
        private float _multiplierY;

        // Use this for initialization
        void Start() {
            _gameManager = GameManager_2.Instance;
            _soundManager = SoundManager_2.Instance;

            _particle = GetComponentInParent<ParticleSystem>();

            _particle.Stop();

            if (_player == null) {
                _player = GameObject.Find("Sled").transform;
            }
        }

        private void Update() {
            _distance = Vector2.Distance(_player.position, this.transform.position);

            if (_distance < _followDistance) {
                if (_player.position.y < this.transform.position.y) {
                    _multiplierY = 0f;
                } else {
                    _multiplierY = 1f;
                }

                if (_player.position.x < this.transform.position.x) {
                    _multiplierX = -1f;
                } else {
                    _multiplierX = 1f;
                }

                transform.Translate(new Vector3(_multiplierX, _multiplierY, 0) * _followSpeed * Time.deltaTime);
            }
        }

        private void OnTriggerEnter2D(Collider2D collider) {
            if (collider.gameObject.CompareTag("Player")) {
                Sled_2 player = collider.gameObject.GetComponent<Sled_2>();

                if (player.FireworkCurrentCount < player.FireworkMaxCount) {
                    _particle.Emit(20);

                    _soundManager.PlaySound(3);

                    player.FireworkCurrentCount++;

                    _gameManager.UpdateFireworks(player.FireworkCurrentCount);

                    Destroy(this.gameObject);
                }
            }
        }
    }
}
