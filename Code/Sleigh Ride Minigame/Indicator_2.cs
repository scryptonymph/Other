using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rekiviskaus {
    public class Indicator_2 : MonoBehaviour {

        [SerializeField] private Transform _player;

        [SerializeField] private GameObject _indicator;

        [SerializeField] private float _edge = 6f;

        [SerializeField] private float _offsetY = 1.5f;

        [SerializeField] private float _offsetX = 1f;

        private bool _goingUp = true;

        // Use this for initialization
        void Start() {
            _indicator.SetActive(false);
        }

        private void Update() {
            if (_goingUp && _player.position.y > _edge) {
                _indicator.transform.position = new Vector2(_player.position.x - _offsetX, _edge - _offsetY);

                _indicator.SetActive(true);

                _goingUp = false;
            }

            if (!_goingUp && _player.position.y < _edge) {
                _indicator.SetActive(false);

                _goingUp = true;
            }
        }
    }
}
