using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SleighRide {
    public class CameraFollow : MonoBehaviour {

        [SerializeField] private Transform _player;

        private Vector3 _offset;

        [SerializeField] private float _minFollowDistance;
        [SerializeField] private float _maxFollowDistance;

        private void LateUpdate() {
            if (_player.position.x < _minFollowDistance || _player.position.x > _maxFollowDistance) {
                return;
            }

            Vector3 position = transform.position;

            position.x = _player.position.x;

            transform.position = position;
        }
    }
}
