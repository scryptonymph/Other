using System.Collections;
using UnityEngine;
using Coffee.UIExtensions;

// A level in the Hertan Ruskaretki remake where the player has to press 
// a character an appropriate time visualized by a meter to make the character 
// jump across a swamp.
public class SwampJumpLevel : MonoBehaviour {
    public static LevelManager22_1 Instance = null;

    private SoundManager22_1 _soundManager;

    private Camera _camera;

    [SerializeField] private Animator _sceneAnimator;
    [SerializeField] private GameObject _tollo;
    [SerializeField] private GameObject _meter;
    [SerializeField] private float _meterSpeed;
    [SerializeField] private float _waitTime;
    [SerializeField] private float _minPressTime;

    private RectTransform _meterLevel;
    private float _meterMin;
    private float _meterMax = 0f;

    private float _timeSinceTouch = 0f;
    private float _timeOfTouch = 0f;

    private int _jumpCount = 0;

    private Vector3 _startPosition;

    private bool _touched = false;

    private Vector3 _touchPos;
    private Vector2 _touchPos2D;
    private GameObject _hitObject;

    private bool _began = false;
    private bool _isPressed = false;
    private bool _isJumping = false;

    void Awake() {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    private void Start() {
        _soundManager = SoundManager22_1.Instance;

        _camera = Camera.main;

        _meterLevel = _meter.GetComponentInChildren<Unmask>().gameObject.GetComponent<RectTransform>();

        _meterMin = _meterLevel.sizeDelta.y;

        _sceneAnimator.SetTrigger("Idle");

        StartCoroutine(WaitForPlayerInput());
    }

    private void Update() {
        _timeSinceTouch = Time.time - _timeOfTouch;

        if (!_isJumping) {
            GetTouchInput();
        }
    }

    private void GetTouchInput() {
#if UNITY_EDITOR
        if (Input.GetMouseButton(0) && !_isPressed) {
            _touchPos = _camera.ScreenToWorldPoint(Input.mousePosition);

            _touchPos2D = new Vector2(_touchPos.x, _touchPos.y);

            RaycastHit2D hit = Physics2D.Raycast(_touchPos2D, _camera.transform.forward);

            if (hit.collider != null) {
                if (hit.collider.gameObject == _tollo) {
                    if (!_began) {
                        _began = true;
                    }

                    _isPressed = true;

                    _meter.transform.position = new Vector3(_tollo.transform.position.x, _tollo.transform.position.y + 3.5f, _tollo.transform.position.z);

                    _meter.SetActive(true);

                    switch (_jumpCount) {
                        case 0:
                            _sceneAnimator.SetTrigger("Pressed 1");
                            break;
                        case 1:
                            _sceneAnimator.SetTrigger("Pressed 2");
                            break;
                        case 2:
                            _sceneAnimator.SetTrigger("Pressed 3");
                            break;
                        case 3:
                            _sceneAnimator.SetTrigger("Pressed 4");
                            break;
                    }

                    _timeOfTouch = Time.time;

                    _timeSinceTouch = 0f;
                }
            }
        }

        if (_isPressed) {
            FillMeter();
        }

        if (Input.GetMouseButtonUp(0) && _isPressed) {
            _isJumping = true;

            _isPressed = false;

            ResetMeter();

            if (_timeSinceTouch > _minPressTime) {
                _sceneAnimator.SetTrigger("Jump");

                _jumpCount++;

                _soundManager.PlaySound(6);
            } else {
                _sceneAnimator.SetTrigger("Swamp");

                switch (_jumpCount) {
                    case 0:
                    case 2:
                        _soundManager.PlaySound(4);
                        break;
                    case 1:
                    case 3:
                        _soundManager.PlaySound(5);
                        break;
                }
            }

            StartCoroutine(WaitForAnimationEnd());
        }
#endif
        if (Input.touchCount > 0) {

            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began) {
                _touchPos = _camera.ScreenToWorldPoint(touch.position);

                _touchPos2D = new Vector2(_touchPos.x, _touchPos.y);

                RaycastHit2D hit = Physics2D.Raycast(_touchPos2D, _camera.transform.forward);

                if (hit.collider != null) {
                    if (hit.collider.gameObject == _tollo) {
                        if (!_began) {
                            _began = true;
                        }

                        _isPressed = true;

                        _meter.transform.position = new Vector3(_tollo.transform.position.x, _tollo.transform.position.y + 3.5f, _tollo.transform.position.z);

                        _meter.SetActive(true);

                        switch (_jumpCount) {
                            case 0:
                                _sceneAnimator.SetTrigger("Pressed 1");
                                break;
                            case 1:
                                _sceneAnimator.SetTrigger("Pressed 2");
                                break;
                            case 2:
                                _sceneAnimator.SetTrigger("Pressed 3");
                                break;
                            case 3:
                                _sceneAnimator.SetTrigger("Pressed 4");
                                break;
                        }

                        _timeOfTouch = Time.time;

                        _timeSinceTouch = 0f;
                    }
                }
            }

            if (_isPressed) {
                FillMeter();
            }

            if (touch.phase == TouchPhase.Ended && _isPressed) {
                _isJumping = true;

                _isPressed = false;

                ResetMeter();

                if (_timeSinceTouch > _minPressTime) {
                    _sceneAnimator.SetTrigger("Jump");

                    _jumpCount++;

                    _soundManager.PlaySound(6);
                } else {
                    _sceneAnimator.SetTrigger("Swamp");

                    switch (_jumpCount) {
                        case 0:
                        case 2:
                            _soundManager.PlaySound(4);
                            break;
                        case 1:
                        case 3:
                            _soundManager.PlaySound(5);
                            break;
                    }
                }

                StartCoroutine(WaitForAnimationEnd());
            }
        }
    }

    private IEnumerator WaitForPlayerInput() {
        while (!_began) {
            _soundManager.PlaySound(3);

            yield return new WaitForSeconds(_waitTime);
        }
    }

    private void FillMeter() {
        if (_meterLevel.sizeDelta.y < _meterMax) {
            return;
        } else {
            _meterLevel.sizeDelta = new Vector2(_meterLevel.sizeDelta.x, _meterLevel.sizeDelta.y - _meterSpeed * 10f * Time.deltaTime);
        }
    }

    private void ResetMeter() {
        _meterLevel.sizeDelta = new Vector2(_meterLevel.sizeDelta.x, _meterMin);

        _meter.SetActive(false);
    }

    private IEnumerator WaitForAnimationEnd() {
        yield return new WaitForSeconds(1.5f);

        _isJumping = false;
    }
}
