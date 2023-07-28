using System.Collections;
using UnityEngine;

// A level in the Hertan Ruskaretki remake where the player has to make 
// crepes for the characters, pouring the correct amount of batter 
// on to a skillet, flip the crepe by tilting the phone (using 
// the accelerator, with outcomes of too little, correct, too much) 
// and slide it onto a plate before it visually burns (using 
// the accelerometer and a timer connected to a gradual color change).
public class FlapjackLevel : MonoBehaviour {
    public static LevelManager Instance = null;

    private SoundManager _soundManager;

    private Camera _camera;

    [SerializeField] private Animator _sceneAnimator;
    [SerializeField] private GameObject _scene30_2;
    [SerializeField] private GameObject _butterGlob;
    [SerializeField] private GameObject _bottle;
    [SerializeField] private float _pourRotation;
    [SerializeField] private Transform[] _batters;
    [SerializeField] private float _correctBatterScale;
    [SerializeField] private float _correctBatterScaleOffset;
    [SerializeField] private float _overflowScale;
    [SerializeField] private float _correctThrowTime;
    [SerializeField] private float _correctThrowTimeOffset;
    [SerializeField] private GameObject[] _uiFlapjacks;
    [SerializeField] private GameObject _uiFlip;
    [SerializeField] private GameObject _uiTilt;
    [SerializeField] private GameObject _plate;
    [SerializeField] private float _waitTime;
    [SerializeField] private string _nextScene;

    private float _originalRot;

    private GameObject _bottleBatter;

    private Vector3 _originalScale;

    private SVGImage _readyFlapjack;

    private Coroutine _startRoutine;
    private bool _began = false;

    private float _time = 0f;

    private bool _levelEnd = false;
    private bool _overFlow = false;
    private bool _cooking = false;
    private bool _pour = false;
    private bool _flip = false;
    private bool _tilt = false;
        
    private Vector3 _touchPos;
    private Vector2 _touchPos2D;
    private GameObject _hitObject;
    private int _touchCount = 0;

    private Vector3 _rotationDirection = Vector3.zero;
    private float _rotationAngle = 0f;
    private float _pourAngle = 0f;

    private bool _wasPoured = false;

    private int _flapjackCount = 0;

    private float _startAcceleration = 0f;
    private float _peakAcceleration = 0f;
    private bool _reset = true;

    void Awake() {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    private void Start() {
        _soundManager = SoundManager.Instance;

        _camera = Camera.main;

        _originalRot = _bottle.transform.eulerAngles.z;

        _bottleBatter = _bottle.transform.GetChild(0).GetChild(0).gameObject;

        _originalScale = _batters[0].localScale;

        _readyFlapjack = _batters[2].GetComponent<SVGImage>();

        _startRoutine = StartCoroutine(WaitForPlayerInput());
    }

    private void Update() {
        _time += Time.deltaTime;

        if (_scene30_2.activeInHierarchy && !_levelEnd && !_butterGlob.activeInHierarchy && !_overFlow && !_cooking) {
            GetTouchInput();
        }

        if (_pour) {
            Pour();
        }

        if (_overFlow) {
            Overflow();
        }

        if (_flip || _tilt) {
            GetTiltInput();
        }

        if (!_flip && _tilt && _cooking) {
            Brown();
        }
    }

    private void GetTouchInput() {
#if UNITY_EDITOR
        if (Input.GetMouseButton(0)) {
            _touchPos = _camera.ScreenToWorldPoint(Input.mousePosition);

            _touchPos2D = new Vector2(_touchPos.x, _touchPos.y);
                
            RaycastHit2D hit = Physics2D.Raycast(_touchPos2D, _camera.transform.forward);

            if (hit.collider != null) {
                if (hit.collider.gameObject == _bottle) {
                    if (!_began) {
                        _began = true;

                        StopCoroutine(_startRoutine);
                    }

                    Rotate(_touchPos);
                }
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            if (!_pour) {

                if (_batters[0].localScale.z < _correctBatterScale + _correctBatterScaleOffset * 2 && _batters[0].localScale.z > _correctBatterScale - _correctBatterScaleOffset) {
                    _soundManager.StopSound();

                    _soundManager.PlaySound(6);

                    _uiFlip.SetActive(true);
                        
                    _bottle.transform.eulerAngles = new Vector3(0f, 0f, _originalRot);

                    _bottle.SetActive(false);

                    _cooking = true;

                    _flip = true;

                    _sceneAnimator.SetTrigger("Dry");
                } else if (_batters[0].localScale.z < _correctBatterScale - _correctBatterScaleOffset && _wasPoured) {
                    _wasPoured = false;

                    _soundManager.StopSound();

                    _soundManager.PlaySound(3);
                }
            }
        }
#endif
        if (Input.touchCount > 0) {

            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved) {
                _touchPos = _camera.ScreenToWorldPoint(touch.position);

                _touchPos2D = new Vector2(_touchPos.x, _touchPos.y);

                RaycastHit2D hit = Physics2D.Raycast(_touchPos2D, _camera.transform.forward);

                if (hit.collider != null) {
                    if (hit.collider.gameObject == _bottle) {
                        if (!_began) {
                            _began = true;

                            StopCoroutine(_startRoutine);
                        }
                    }

                    Rotate(_touchPos);
                }
            }

            if (touch.phase == TouchPhase.Ended) {
                if (!_pour) {

                    if (_batters[0].localScale.z < _correctBatterScale + _correctBatterScaleOffset && _batters[0].localScale.z > _correctBatterScale - _correctBatterScaleOffset) {
                        _soundManager.StopSound();

                        _soundManager.PlaySound(6);

                        _bottle.transform.eulerAngles = new Vector3(0f, 0f, _originalRot);

                        _bottle.SetActive(false);

                        _cooking = true;

                        _flip = true;

                        _sceneAnimator.SetTrigger("Dry");
                    } else if (_batters[0].localScale.z < _correctBatterScale - _correctBatterScaleOffset && _wasPoured) {
                        _wasPoured = false;

                        _soundManager.StopSound();

                        _soundManager.PlaySound(3);
                    }
                }
            }
        }
    }

    float _distance;

    private void GetTiltInput() {
#if UNITY_EDITOR
        if (_flip) {
            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                _time = 0f;

                _uiFlip.SetActive(false);
            }

            if (Input.GetKeyUp(KeyCode.UpArrow)) {
                if (_time < _correctThrowTime && _time > _correctThrowTime - _correctThrowTimeOffset) {
                    StartCoroutine(Correct());
                } else if (_time > _correctThrowTime) {
                    StartCoroutine(Long());
                } else {
                    StartCoroutine(Short());
                }
            }
        }

        if (_tilt) {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                StartCoroutine(Tilt());
            }
        }

#endif
        if (_flip) {
            Debug.Log("start " + _startAcceleration);
            Debug.Log("peak " + _peakAcceleration);
            Debug.Log("input  " + Input.acceleration.y);


            if (_startAcceleration == 0) {
                _startAcceleration = Input.acceleration.y;

                if (_startAcceleration < -0.8) {    // If phone is held at too steep an angle
                    _startAcceleration = -0.8f;
                }
            } else {
                if (Input.acceleration.y < _startAcceleration - 0.2f) {     // If phone has been tilted enough

                    if (_peakAcceleration < Input.acceleration.y) {     // Player is tilting the phone back to neutral
                        _distance = Mathf.Abs(_peakAcceleration - _startAcceleration);

                        if (_distance > 0.35f && _distance < 0.7f) {
                            StartCoroutine(Correct());
                        } else if (_distance > 0.7f) {
                            StartCoroutine(Long());
                        } else if (_distance < 0.35f) {
                            StartCoroutine(Short());
                        }

                        _peakAcceleration = 0f;
                    } else {
                        _peakAcceleration = Input.acceleration.y;
                    }
                }
            }
        }

        if (_tilt) {
            if ((Input.acceleration.x < -0.1f)) {
                StartCoroutine(Tilt());
            }
        }
    }


    private IEnumerator WaitForPlayerInput() {
        while (!_scene30_2.activeInHierarchy) {
            yield return null;
        }

        _sceneAnimator.SetTrigger("Butter");

        while (!_began) {
            yield return new WaitForSeconds(_waitTime);

            _soundManager.PlaySound(2);
            _sceneAnimator.SetTrigger("Bottle");
        }
    }

    private void Rotate(Vector3 touchPos) {
        _rotationDirection = touchPos - _bottle.transform.position;

        _rotationAngle = Mathf.Atan2(_rotationDirection.x, _rotationDirection.y) * Mathf.Rad2Deg;

        Debug.Log(_bottle.transform.eulerAngles.z);

        if ((-_rotationAngle > _originalRot && -_rotationAngle < (_pourRotation + 10f)) || (_rotationAngle > _pourAngle && _bottle.transform.eulerAngles.z < _pourAngle + 10f && _pour)) {
            // -angle to get the rotation to work clockwise
            _bottle.transform.eulerAngles = new Vector3(0f, 0f, -_rotationAngle);
        }

        if (!_pour && _bottle.transform.eulerAngles.z < _pourRotation + 10f && _bottle.transform.eulerAngles.z > _pourRotation - 2f) {
            _bottleBatter.SetActive(true);

            _batters[0].gameObject.SetActive(true);
            _pourAngle = _rotationAngle;

            _wasPoured = true;

            _pour = true;
        } else if (_pour && _bottle.transform.eulerAngles.z < _pourRotation -2f) {
            _bottleBatter.SetActive(false);

            _pour = false;
        }
    }

    private void Pour() {
        _batters[0].localScale = new Vector3(_batters[0].localScale.x + Time.deltaTime, _batters[0].localScale.z + Time.deltaTime, _batters[0].localScale.z + Time.deltaTime);

        if (_batters[0].localScale.z > _correctBatterScale + _correctBatterScaleOffset) {
            _bottleBatter.SetActive(false);

            _bottle.transform.eulerAngles = new Vector3(0f, 0f, _originalRot);

            _overFlow = true;

            _pour = false;

            _sceneAnimator.SetTrigger("Dry");

            _soundManager.PlaySound(4);
        }
    }

    private void Overflow() {
        _batters[0].localScale = new Vector3(_batters[0].localScale.x + Time.deltaTime, _batters[0].localScale.z + Time.deltaTime, _batters[0].localScale.z + Time.deltaTime);

        if (_batters[0].localScale.z > _overflowScale + 0.5f) {
            _overFlow = false;

            _batters[1].gameObject.SetActive(false);

            _batters[0].localScale = _originalScale;

            _sceneAnimator.SetTrigger("Butter");

            _wasPoured = false;
        } else if (_batters[0].localScale.z > _overflowScale) {
            _batters[0].gameObject.SetActive(false);
            _batters[1].gameObject.SetActive(true);
        }
    }

    private IEnumerator Correct() {
        _batters[0].gameObject.SetActive(false);

        _flip = false;

        _sceneAnimator.SetTrigger("Correct");

        yield return new WaitForSeconds(1f);

        _soundManager.StopSound();

        _soundManager.PlaySound(8);

        yield return new WaitForSeconds(_waitTime / 3f);

        _batters[0].localScale = _originalScale;

        _tilt = true;

        _plate.SetActive(true);

        _soundManager.PlaySound(7);

        _startAcceleration = 0f;
    }

    private IEnumerator Long() {
        _batters[0].gameObject.SetActive(false);

        _flip = false;

        _sceneAnimator.SetTrigger("Long");

        yield return new WaitForSeconds(1f);

        _soundManager.StopSound();

        _soundManager.PlaySound(9);

        yield return new WaitForSeconds(_waitTime / 3f);

        _cooking = false;

        _sceneAnimator.SetTrigger("Butter");

        _batters[0].localScale = _originalScale;

        _bottle.SetActive(true);

        _startAcceleration = 0f;
    }

    private IEnumerator Short() {
        _batters[0].gameObject.SetActive(false);

        _flip = false;

        _sceneAnimator.SetTrigger("Short");

        yield return new WaitForSeconds(1f);

        _soundManager.StopSound();

        _soundManager.PlaySound(9);

        _flip = true;

        _startAcceleration = 0f;
    }

    private IEnumerator Tilt() {
        _tilt = false;

        _sceneAnimator.SetTrigger("Tilt");

        _soundManager.StopSound();

        _soundManager.PlaySound(8);

        _uiFlapjacks[_flapjackCount].SetActive(true);
            
        _flapjackCount++;

        yield return new WaitForSeconds(_waitTime / 4f);

        _plate.SetActive(false);

        if (_flapjackCount < _uiFlapjacks.Length) {
            _cooking = false;

            _batters[2].gameObject.SetActive(false);

            _sceneAnimator.SetTrigger("Butter");

            _wasPoured = false;

            _bottle.SetActive(true);

            yield return new WaitForSeconds(2f);
                
            _readyFlapjack.color = Color.white;
        } else {
            _sceneAnimator.GetComponent<SceneLoader>().LoadInterimScene(_nextScene);
        }
    }

    private void Brown() {
        if (_readyFlapjack.color.r > 0.5f) {
            _readyFlapjack.color = new Color(_readyFlapjack.color.r - (0.1f * Time.deltaTime), _readyFlapjack.color.g - (0.1f * Time.deltaTime), _readyFlapjack.color.b - (0.1f * Time.deltaTime));
        } else {
            _tilt = false;

            _soundManager.PlaySound(5);

            _batters[2].gameObject.SetActive(false);

            _plate.SetActive(false);

            _sceneAnimator.SetTrigger("Burnt");

            _cooking = false;

            _readyFlapjack.color = Color.white;

            _bottle.SetActive(true);

            _sceneAnimator.SetTrigger("Butter");

            _wasPoured = false;
        }
    }
}
