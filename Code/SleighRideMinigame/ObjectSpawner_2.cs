using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rekiviskaus {
    public class ObjectSpawner_2 : MonoBehaviour {

        private GameManager_2 _gameManager;

        [SerializeField] private Transform _player;

        [SerializeField] private List<Transform> _objects;

        private Vector2 _previousPosition;

        [SerializeField] private float _spawnDistance;
        [SerializeField] private float _spawnAheadOffset;
        [SerializeField] private float _spawnHeight;

        private List<Transform> _objectPool;

        [SerializeField] private int _maxObjectPoolSize;

        private bool _poolEmpty = false;

        // Use this for initialization
        void Start() {
            _gameManager = GameManager_2.Instance;

            _previousPosition = _player.position;

            _objectPool = new List<Transform>();
        }

        // Update is called once per frame
        void Update() {
            if ((_player.position.x - _previousPosition.x) > _spawnDistance) {
                Spawn();

                _previousPosition = _player.position;
            }
        }

        private void Spawn() {
            int spawnIndex = Random.Range(0, _objects.Count);

            Transform objectToSpawn = Instantiate(_objects[spawnIndex], new Vector2(_player.position.x + _spawnAheadOffset, _spawnHeight), Quaternion.identity);

            _objectPool.Add(objectToSpawn);

            if (_objectPool.Count > _maxObjectPoolSize) {
                Destroy(_objectPool[0].gameObject);

                _objectPool.RemoveAt(0);
            }
        }
    }
}
