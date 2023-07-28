using System.Collections;
using UnityEngine;

// A level in the Hertan Ruskaretki remake where the player has to move
// a compass needle to point to the correct random cardinal point cued 
// by audio.
public class CompassLevel : MonoBehaviour {
    public static LevelManager Instance = null;

    private SoundManager _soundManager;

    private Camera _camera;

    [SerializeField] private Animator _sceneAnimator;
    [SerializeField] private GameObject _scene24_1;
    [SerializeField] private Transform _compassNeedle;
    [SerializeField] private float _needleOffset;
    [SerializeField] private float _waitTime;

    private int _targetPoint = 0;
    private int _startingPoint = 0;

    private float _originalRotation = 0f;

    // EulerAngles are counterclockwise
    private float _northRotation = 0f;
    private float _eastRotation = 270f;
    private float _southRotation = 180f;
    private float _westRotation = 90f;

    private Coroutine _startRoutine;

    private Vector3 _touchPos;
    private Vector2 _touchPos2D;

    private bool _began;
    private bool _pressed;

    private Vector3 _rotationDirection = Vector3.zero;

    private float _rotationAngle = 0;

    private bool _isCorrect;

    private float _time = 0f;

    void Awake() {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    private void Start() {
        _soundManager = SoundManager.Instance;
            
        _camera = Camera.main;

        _targetPoint = Random.Range(1, 5);

        do {
            _startingPoint = Random.Range(1, 5);
        } while (_startingPoint == _targetPoint);

        switch (_startingPoint) {
            case 1:
                _originalRotation = _northRotation;
                break;
            case 2:
                _originalRotation = _eastRotation;
                break;
            case 3:
                _originalRotation = _southRotation;
                break;
            case 4:
                _originalRotation = _westRotation;
                break;
        }

        _compassNeedle.eulerAngles = new Vector3(0f, 0f, _originalRotation);

        _startRoutine = StartCoroutine(WaitForPlayerInput());
    }

    private void Update() {
        _time += Time.deltaTime;

        if (_scene24_1.activeInHierarchy && !_isCorrect) {
            GetTouchInput();
        }
    }

    private void GetTouchInput() {
#if UNITY_EDITOR
        if (Input.GetMouseButton(0)) {
            _touchPos = _camera.ScreenToWorldPoint(Input.mousePosition);

            _touchPos2D = new Vector2(_touchPos.x, _touchPos.y);

            RaycastHit2D hit = Physics2D.Raycast(_touchPos2D, _camera.transform.forward);

            if (hit.collider != null) {
                if (hit.collider.gameObject == _compassNeedle.gameObject) {
                    if (!_began) {
                        _began = true;

                        StopCoroutine(_startRoutine);
                    }

                    _pressed = true;
                }
            }
        }

        if (_pressed) {
            Rotate(_touchPos);
        }

        if (Input.GetMouseButtonUp(0) && _pressed) {
            _pressed = false;

            CheckPoint();
        }
#endif
        if (Input.touchCount > 0) {

            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved) {
                _touchPos = _camera.ScreenToWorldPoint(touch.position);

                _touchPos2D = new Vector2(_touchPos.x, _touchPos.y);

                RaycastHit2D hit = Physics2D.Raycast(_touchPos2D, _camera.transform.forward);

                if (hit.collider != null) {
                    if (hit.collider.gameObject == _compassNeedle.gameObject) {
                        if (!_began) {
                            _began = true;

                            StopCoroutine(_startRoutine);
                        }

                        _pressed = true;
                    }
                }
            }

            if (_pressed) {
                Rotate(_touchPos);
            }

            if (touch.phase == TouchPhase.Ended && _pressed) {
                _pressed = false;

                CheckPoint();
            }
        }
    }

    private void Rotate(Vector3 touchPos) {
        _rotationDirection = touchPos - _compassNeedle.position;

        _rotationAngle = Mathf.Atan2(_rotationDirection.x, _rotationDirection.y) * Mathf.Rad2Deg;

        // -angle to get the rotation to work clockwise
        _compassNeedle.eulerAngles = new Vector3(0f, 0f, -_rotationAngle);
    }

    private void CheckPoint() {
        if ((_compassNeedle.eulerAngles.z > 0f && _compassNeedle.eulerAngles.z < (_northRotation + _needleOffset)) ||
                (_compassNeedle.eulerAngles.z < 360f && _compassNeedle.eulerAngles.z > (360f - _needleOffset))) {

            _compassNeedle.eulerAngles = new Vector3(0f, 0f, _northRotation);

            if (_targetPoint == 1) {
                _isCorrect = true;
            }
        } else if (_compassNeedle.eulerAngles.z > (_eastRotation - _needleOffset) && _compassNeedle.eulerAngles.z < (_eastRotation + _needleOffset)) {

            _compassNeedle.eulerAngles = new Vector3(0f, 0f, _eastRotation);

            if (_targetPoint == 2) {
                _isCorrect = true;
            }
        } else if (_compassNeedle.eulerAngles.z > (_southRotation - _needleOffset) && _compassNeedle.eulerAngles.z < (_southRotation + _needleOffset)) {

            _compassNeedle.eulerAngles = new Vector3(0f, 0f, _southRotation);

            if (_targetPoint == 3) {
                _isCorrect = true;
            }
        } else if (_compassNeedle.eulerAngles.z > (_westRotation - _needleOffset) && _compassNeedle.eulerAngles.z < (_westRotation + _needleOffset)) {

            _compassNeedle.eulerAngles = new Vector3(0f, 0f, _westRotation);

            if (_targetPoint == 4) {
                _isCorrect = true;
            }
        } else {
            _isCorrect = false;
        }

        if (_isCorrect) {
            StartCoroutine(EndTransition());
        } else if (_time > 1f) {
            _time = 0f;

            _soundManager.PlaySound(8);
        }
    }

    private IEnumerator WaitForPlayerInput() {
        while (!_scene24_1.activeInHierarchy) {
            yield return new WaitForSeconds(1f);
        }

        _soundManager.PlaySound(_targetPoint + 1);

        while (!_began) {
            yield return new WaitForSeconds(_waitTime);

            _soundManager.PlaySound(6);

            yield return new WaitForSeconds(_waitTime);
        }
    }

    private IEnumerator EndTransition() {
        _soundManager.PlaySound(7);

        yield return new WaitForSeconds(_waitTime);

        _sceneAnimator.SetTrigger("EndTransition");

        _soundManager.PlaySound(9);
    }
}
