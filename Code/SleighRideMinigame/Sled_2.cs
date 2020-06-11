using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rekiviskaus {
    public class Sled_2 : MonoBehaviour {

        private GameManager_2 _gameManager;
        private SoundManager_2 _soundManager;

        private GameObject _rocketButton;
        [SerializeField] private Animator _canvasAnim;

        [SerializeField] private float _maxStretch = 3f;

        [SerializeField] private LineRenderer _slingshotLineFront;
        [SerializeField] private LineRenderer _slingshotLineBack;
        [SerializeField] private Transform _holdPoint;

        private SpringJoint2D _spring;
        private Rigidbody2D _projectileRigidbody;

        private bool _isDragged = false;

        private Ray _raySlingshotToTouch;

        private Transform _slingshot;

        private Vector2 _previousFrameVelocity;

        [SerializeField] private Vector2 _trampolineForce;
        [SerializeField] private Vector2 _superTrampolineForce;
        [SerializeField] private Vector2 _fireworkForce;
        [SerializeField] private int _fireworkMaxCount;

        public int FireworkMaxCount {
            get {
                return _fireworkMaxCount;
            }
        }

        private int _fireworkCurrentCount;

        public int FireworkCurrentCount {
            get {
                return _fireworkCurrentCount;
            }

            set {
                _fireworkCurrentCount = value;
            }
        }

        public int FireworksUsed {
            get {
                return _fireworkMaxCount - _fireworkCurrentCount;
            }
        }

        private bool _hitRock = false;
        private bool _instructPlayer = true;
        private bool _enableButton = true;

        private ParticleSystem _fireworkEffect;

        private void Awake() {
            _rocketButton = GameObject.Find("RocketButton");

            _rocketButton.SetActive(false);
        }
        
        private void Start() {
            _gameManager = GameManager_2.Instance;
            _soundManager = SoundManager_2.Instance;

            _spring = GetComponent<SpringJoint2D>();
            _projectileRigidbody = GetComponent<Rigidbody2D>();

            _slingshot = _spring.connectedBody.transform;

            LineRendererSetup();

            _raySlingshotToTouch = new Ray(_slingshot.position, Vector3.zero);

            _fireworkCurrentCount = _fireworkMaxCount;

            _fireworkEffect = transform.Find("FireworkEffect").GetComponent<ParticleSystem>();
        }
        
        private void Update() {

            if (_spring != null) {

                if (Input.GetMouseButtonDown(0)) {
                    _spring.enabled = false;

                    _isDragged = true;
                }

                if (Input.GetMouseButtonUp(0)) {
                    _spring.enabled = true;

                    _projectileRigidbody.simulated = true;

                    _isDragged = false;

                    _soundManager.PlaySound(4);
                }

                if (_isDragged) {
                    DragSled();
                }

                if (_projectileRigidbody.simulated && (_previousFrameVelocity.sqrMagnitude > _projectileRigidbody.velocity.sqrMagnitude)) {
                    Destroy(_spring);

                    _projectileRigidbody.velocity = _previousFrameVelocity;
                }

                if (!_isDragged) {
                    _previousFrameVelocity = _projectileRigidbody.velocity;
                }

                LineRendererUpdate();
            } else {
                _slingshotLineBack.enabled = false;
                _slingshotLineFront.enabled = false;

                if (_enableButton) {
                    _rocketButton.SetActive(true);

                    _enableButton = false;
                }
            }

            if (_spring == null && FireworksUsed < _fireworkMaxCount && Input.GetMouseButtonDown(0)) {
                PropelPlayer();
            }

            if (_fireworkCurrentCount == 0 && _projectileRigidbody.velocity == Vector2.zero && !_gameManager.GameOver) {
                _soundManager.PlaySound(9);

                _gameManager.GameOver = true;

            } else if (_fireworkCurrentCount > 0 && _projectileRigidbody.velocity == Vector2.zero && _instructPlayer) {
                StartCoroutine(InstructPlayer());
            }

            if (_projectileRigidbody.velocity.x < 0) {
                _previousFrameVelocity = _projectileRigidbody.velocity;

                _projectileRigidbody.velocity = new Vector2(-_previousFrameVelocity.x, _previousFrameVelocity.y);
            }

            if (_fireworkCurrentCount > 0 && _spring == null) {
                _canvasAnim.SetBool("ShowButton", true);
            } else if (_fireworkCurrentCount == 0) {
                _canvasAnim.SetBool("ShowButton", false);
            }
        }

        private void LineRendererSetup() {
            _slingshotLineBack.SetPosition(0, _slingshotLineBack.transform.position);
            _slingshotLineFront.SetPosition(0, _slingshotLineFront.transform.position);

            _slingshotLineBack.sortingLayerName = "Front Distant";
            _slingshotLineFront.sortingLayerName = "Front Near";

            _slingshotLineBack.sortingOrder = 1;
            _slingshotLineFront.sortingOrder = 13;
        }

        private void LineRendererUpdate() {

            Vector3 holdPoint = new Vector3(_holdPoint.position.x, _holdPoint.position.y);

            _slingshotLineBack.SetPosition(1, holdPoint);
            _slingshotLineFront.SetPosition(1, holdPoint);
        }

        private void DragSled() {
            Vector3 touchWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 slingshotToTouch = touchWorldPoint - _slingshot.position;

            // Square magnitude is quicker to calculate than just magnitude!
            if (slingshotToTouch.sqrMagnitude > (_maxStretch * _maxStretch)) {
                _raySlingshotToTouch.direction = slingshotToTouch;

                touchWorldPoint = _raySlingshotToTouch.GetPoint(_maxStretch);
            }

            touchWorldPoint.z = 0f;

            transform.position = touchWorldPoint;
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            if (collision.gameObject.CompareTag("Trampoline")) {
                _projectileRigidbody.AddForce(_trampolineForce, ForceMode2D.Impulse);

                _soundManager.PlaySound(5);
            }

            if (collision.gameObject.CompareTag("SuperTrampoline")) {
                _projectileRigidbody.AddForce(_superTrampolineForce, ForceMode2D.Impulse);

                _soundManager.PlaySound(5);
            }

            if (collision.gameObject.CompareTag("Floor")) {
                _soundManager.PlaySound(7);
            }

            if (collision.gameObject.CompareTag("Obstacle") && !_hitRock) {
                StartCoroutine(DontReactToRock());

                _soundManager.PlaySound(6);
            }
        }

        private IEnumerator InstructPlayer() {
            _instructPlayer = false;

            yield return new WaitForSeconds(5);
            
            if (_projectileRigidbody.velocity == Vector2.zero) {
                _soundManager.PlaySound(8);
            }

            _instructPlayer = true;
        }

        private IEnumerator DontReactToRock() {
            _hitRock = true;

            yield return new WaitForSeconds(2);

            _hitRock = false;
        }

        private void PropelPlayer() {
            _fireworkEffect.Play();

            _soundManager.PlaySound(2);

            _projectileRigidbody.AddForce(_fireworkForce, ForceMode2D.Impulse);

            _fireworkCurrentCount--;

            _gameManager.UpdateFireworks(_fireworkCurrentCount);
        }
    }
}
