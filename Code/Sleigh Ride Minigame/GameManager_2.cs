using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Rekiviskaus {
    public class GameManager_2 : MonoBehaviour {

        public static GameManager_2 Instance = null;
        private SoundManager_2 _soundManager;

        [SerializeField] private GameObject _canvas;
        private Animator _canvasUiAnim;
        [SerializeField] private Camera _camera;
        private Animator _cameraAnim;

        private bool _gameOver;

        public bool GameOver {
            get {
                return _gameOver;
            }
            set {
                _gameOver = value;
            }
        }

        [SerializeField] private Transform _player;

        [SerializeField] private List<GameObject> _fireworks;

        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _endScoreText;

        [SerializeField] private GameObject _endScreen;

        private int _score;

        private GameObject _fireworkCanvas;

        private void Awake() {
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
                Destroy(gameObject);
        }

        // Use this for initialization
        void Start() {
            _soundManager = SoundManager_2.Instance;

            _canvasUiAnim = _canvas.GetComponent<Animator>();
            _camera = Camera.main;
            _cameraAnim = _camera.GetComponent<Animator>();

            _fireworkCanvas = GameObject.Find("FireworkCanvas").gameObject;

            _endScreen.SetActive(false);

            _fireworkCanvas.SetActive(false);
        }

        // Update is called once per frame
        void Update() {
            if(_player.position.x > 0) {
                _score = System.Convert.ToInt32(_player.position.x);
            }

            if(!_gameOver) {
                _scoreText.text = _score.ToString() + " m";
            } else if (_gameOver && !_endScreen.activeInHierarchy){
                _endScreen.SetActive(true);

                _canvasUiAnim.SetBool("GameOver", true);
                _cameraAnim.SetBool("GameOver", true);

                Invoke("EnableFireWorks", 1f);

                Invoke("BearCongratulates", 2f);

                _endScoreText.text = _score.ToString() + " m";
            }
        }

        public void UpdateFireworks(int fireworks) {
            switch(fireworks) {
                case 0:
                    _fireworks[0].SetActive(false);
                    _fireworks[1].SetActive(false);
                    _fireworks[2].SetActive(false);
                    break;
                case 1:
                    _fireworks[0].SetActive(true);
                    _fireworks[1].SetActive(false);
                    _fireworks[2].SetActive(false);
                    break;
                case 2:
                    _fireworks[0].SetActive(true);
                    _fireworks[1].SetActive(true);
                    _fireworks[2].SetActive(false);
                    break;
                case 3:
                    _fireworks[0].SetActive(true);
                    _fireworks[1].SetActive(true);
                    _fireworks[2].SetActive(true);
                    break;
            }
        }

        private void EnableFireWorks() {
            _fireworkCanvas.SetActive(true);

            _soundManager.PlaySound(10);
        }

        private void BearCongratulates() {
            _soundManager.PlaySound(11);
        }
    }
}
