using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SleighRide {
    public class BGScroller : MonoBehaviour {

        [SerializeField] private float _speed;

        [SerializeField] private List<Transform> _background;

        [SerializeField] private Transform _player;

        private Rigidbody2D _playerRigidbody;

        private void Start() {
            _playerRigidbody = _player.gameObject.GetComponent<Rigidbody2D>();
        }

        private void Update() {
            if (_player.position.x > 0 && (_playerRigidbody.velocity.x > 5)) {
                transform.Translate(Vector2.left * _speed * Time.deltaTime);
            }

            float distance = _background[3].position.x - _player.position.x;

            // Abs just in case
            if (Mathf.Abs(distance) > 5f) {
                return;
            } else {
                MovePlatform();
            }
        }

        private void MovePlatform() {
            Vector2 position = _background[3].position;

            float offset = Vector2.Distance(_background[2].position, _background[3].position);

            position.x = _background[3].position.x + offset;

            _background[0].position = position;

            List<Transform> _temp = new List<Transform> {
                _background[1],
                _background[2],
                _background[3],
                _background[0]
            };

            _background = _temp;
        }
    }
}
