using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SleighRide {
    public class FireworkSpawner : MonoBehaviour {

        [SerializeField] private Transform _player;

        [SerializeField] private List<Transform> _objects;

        private Vector2 _previousPosition;

        [SerializeField] private float _spawnDistance;
        [SerializeField] private float _spawnAheadOffset;
        [SerializeField] private float _spawnHeight;

        private bool _spawn;

        private Sled _playerScript;

        // Use this for initialization
        void Start() {
            _previousPosition = _player.position;

            _playerScript = _player.gameObject.GetComponent<Sled>();
        }

        // Update is called once per frame
        void Update() {
            if (_playerScript.FireworkCurrentCount < _playerScript.FireworkMaxCount) {
                _spawn = true;
            } else {
                return;
            }

            if ((_player.position.x - _previousPosition.x) > _spawnDistance) {
                Spawn();

                _previousPosition = _player.position;
            }
        }

        private void Spawn() {
            int spawnIndex = Random.Range(0, _objects.Count);

            Transform objectToSpawn = Instantiate(_objects[spawnIndex], new Vector2(_player.position.x + _spawnAheadOffset, _spawnHeight), Quaternion.identity);
        }
    }
}
