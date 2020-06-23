using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// A level in the Hertan Ruskaretki remake where the player tilts their device
// (using the accelerometer) to guide the character's canoe safely down the river
// as the background and its hazards scrolls by.
public class CanoeingLevel : MonoBehaviour {
    public static LevelManager27 Instance = null;

    private SoundManager27 _soundManager;

    private Camera _camera;

    [SerializeField] private Animator _sceneAnimator;
    [SerializeField] private ScrollRect _scrollView;
    [SerializeField] private GameObject _player;
    [SerializeField] private float _scrollSpeedMax;
    [SerializeField] private float _scrollSpeedMin;
    [SerializeField] private float _scrollChangeLerpSpeed;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _invulnerableTime;
    [SerializeField] private float _waitTime;

    private Transform _rotatingKayak;
    private Transform _rotatingTollo;

    private Transform _content;
    private float _height = 10000f;

    private float _scrollSpeed;

    private bool _levelEnd = false;
    private bool _began = false;

    private float _playerPositionX;

    private Vector3 _originalPos;

    private CollisionDetection _playerCollision;

    private Coroutine _backgroundRoutine;

    private float _time = 0f;
    private float _lerpTime = 0f;

    private bool _isInvulnerable = false;
    private bool _isSpinning = false;
    private bool _rotateBack = false;
    private bool _firstCollision = true;

    void Awake() {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    private void Start() {
        _soundManager = SoundManager27.Instance;
            
        _camera = Camera.main;

        _rotatingKayak = _player.transform.GetChild(0);
        _rotatingTollo = _rotatingKayak.GetChild(0);

        _content = _scrollView.content;

        _scrollSpeed = _scrollSpeedMin;

        _playerPositionX = _player.transform.localPosition.x;

        _originalPos = _rotatingKayak.position;

        _playerCollision = _player.GetComponent<CollisionDetection>();

        _backgroundRoutine = StartCoroutine(SoundRoutine());
    }

    private void Update() {
        _time += Time.deltaTime;

        if (_began) {
            if (_lerpTime < 1) {
                _lerpTime += _scrollChangeLerpSpeed * Time.deltaTime;

                _scrollSpeed = Mathf.Lerp(_scrollSpeedMin, _scrollSpeedMax, _lerpTime);
            }

            ScrollScene();

            if (!_isInvulnerable && !_levelEnd) {
                GetTiltInput();
            }
        }
    }

    private void FixedUpdate() {
        if (_playerCollision.Collided) {
            StartCoroutine(BecomeInvulnerable());
        }
    }

    private void ScrollScene() {
        _content.localPosition = new Vector3(_content.localPosition.x, _content.localPosition.y - _scrollSpeed * 10f * Time.deltaTime, 0f);

        if (!_isSpinning) {
            _player.transform.localPosition = new Vector3(_playerPositionX, _player.transform.localPosition.y + _scrollSpeed * 10f * Time.deltaTime, 0f);
        } else {
            Spin();
        }

        if (_player.transform.localPosition.y > 9000f && !_levelEnd) {
            EndGame();
        }
    }

    private void GetTiltInput() {
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.LeftArrow)) {
            _playerPositionX = _player.transform.localPosition.x - 1f;

            Rotate(Vector3.right);
        }

        if (Input.GetKey(KeyCode.RightArrow)) {
            _playerPositionX = _player.transform.localPosition.x + 1f;

            Rotate(Vector3.left);
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow)) {
            _rotateBack = true;
        }
            
#endif
        if (Input.acceleration.x < -0.1f) {
            _playerPositionX += Input.acceleration.x * _movementSpeed;

            Rotate(Vector3.right);
        }

        if (Input.acceleration.x > 0.1f) {
            _playerPositionX += Input.acceleration.x * _movementSpeed;

            Rotate(Vector3.left);
        }

        if (Input.acceleration.x > -0.1f && Input.acceleration.x < 0.1f) {
            _rotateBack = true;
        }

        if (_rotateBack) {
            _rotatingKayak.rotation = Quaternion.Slerp(_rotatingKayak.rotation, Quaternion.identity, _movementSpeed * Time.deltaTime);
            _rotatingTollo.rotation = Quaternion.Slerp(_rotatingTollo.rotation, Quaternion.identity, _movementSpeed * Time.deltaTime);

            if (_rotatingKayak.rotation == Quaternion.identity) {
                _rotateBack = false;
            }
        }
    }

    private void Rotate(Vector3 direction) {
        Vector3 targetDirection = direction - _originalPos;

        float angle = Mathf.Atan2(targetDirection.x, targetDirection.y) * Mathf.Rad2Deg;

        Quaternion rotation = Quaternion.AngleAxis(angle * 2f, Vector3.forward);
        Quaternion reverseRotation = Quaternion.AngleAxis(-angle, Vector3.forward);

        _rotatingKayak.rotation = Quaternion.Slerp(_rotatingKayak.rotation, rotation, _movementSpeed * Time.deltaTime);
        _rotatingTollo.rotation = Quaternion.Slerp(_rotatingTollo.rotation, reverseRotation, _movementSpeed * Time.deltaTime);
    }

    private void Spin() {
        _rotatingKayak.Rotate(Vector3.forward * _movementSpeed * 60f * Time.deltaTime);
    }

    private IEnumerator BecomeInvulnerable() {
        if (_firstCollision) {
            _firstCollision = false;

            _soundManager.PlaySound(3);
        } else {
            _soundManager.PlaySound(5);
        }

        _isInvulnerable = true;

        _playerCollision.Collided = false;

        _player.GetComponent<Collider2D>().enabled = false;

        Vector3 resetPosition = new Vector3(0f, _player.transform.position.y, 0f);

        _isSpinning = true;

        SwitchScrollSpeed();

        _time = 0f;

        while (_time < _invulnerableTime) {
            yield return new WaitForSeconds(0.1f);
        }

        _player.transform.position = resetPosition;
        _playerPositionX = 0f;
        _rotatingKayak.rotation = Quaternion.identity;
        _rotatingTollo.rotation = Quaternion.identity;

        _isSpinning = false;

        SwitchScrollSpeed();

        _time = 0f;

        while (_time < _invulnerableTime) {
            yield return new WaitForSeconds(0.1f);

            _rotatingKayak.GetComponent<SVGImage>().enabled = false;

            yield return new WaitForSeconds(0.1f);

            _rotatingKayak.GetComponent<SVGImage>().enabled = true;
        }

        _player.GetComponent<Collider2D>().enabled = true;

        _isInvulnerable = false;
    }

    private void SwitchScrollSpeed() {
        float temp = _scrollSpeedMax;

        _scrollSpeedMax = _scrollSpeedMin;

        _scrollSpeedMin = temp;

        _lerpTime = 0f;
    }

    private IEnumerator SoundRoutine() {
        _soundManager.PlaySound(2);

        yield return new WaitForSeconds(_waitTime);

        _began = true;

        while (!_levelEnd) {
            yield return new WaitForSeconds(_waitTime * 4f);

            _soundManager.PlaySound(4);
        }
    }

    private void EndGame() {
        _levelEnd = true;

        StopCoroutine(_backgroundRoutine);

        _soundManager.PlaySound(4);

        _scrollSpeedMin = 0f;

        SwitchScrollSpeed();

        _sceneAnimator.SetTrigger("End");
    }
}
