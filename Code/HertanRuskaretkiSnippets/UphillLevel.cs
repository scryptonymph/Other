using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

// A level in the Hertan Ruskaretki remake where the player has to pull down 
// two sliders continuously one after the other to make characters walk up 
// a hill. If the player pauses, the characters begin to slide down.
public class UphillLevel : MonoBehaviour {
    public static LevelManager17 Instance = null;

    private SoundManager17 _soundManager;

    private Camera _camera;

    [SerializeField] private List<GameObject> _characters;
    [SerializeField, HideInInspector] private List<Animator> _animators;
    [SerializeField] private Animator _sceneAnimator;
    [SerializeField] private PlayableDirector _playableDirector;
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _midPoint;
    [SerializeField] private Transform _endPoint;
    [SerializeField] private GameObject _UI;
    [SerializeField] private Slider _leftSlider;
    [SerializeField] private Slider _rightSlider;
    [SerializeField, Range(1, 5)] private float _sliderSpeed;
    [SerializeField] private float _waitTime;

    private Coroutine _waitRoutine;

    private Transform _player;

    private Vector3 _playerScreenPos;

    private float _timeSinceTouch = 0f;
    private float _timeOfTouch = 0f;

    private bool _levelBegan = false;
    private bool _touchedLeft = false;
    private bool _touchedRight = false;

    private bool _playerIsMoving = false;

    public bool PlayerIsMoving {
        get {
            return _playerIsMoving;
        }
    }

    private bool _playerIsMovingBack = false;

    public bool PlayerIsMovingBack {
        get {
            return _playerIsMovingBack;
        }
    }

    private bool _levelEnd = false;
    private bool _levelMid = false;
    private bool _levelStart = false;

    void Awake() {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    private void Start() {
        _soundManager = SoundManager17.Instance;

        _camera = Camera.main;

        for (int i = 0; i < _characters.Count; i++) {
            _animators.Add(_characters[i].GetComponent<Animator>());
        }

        _player = _characters[0].transform;

        _playerScreenPos = _camera.WorldToScreenPoint(new Vector3(_player.position.x, 0.0f, 0.0f));

        _sceneAnimator.enabled = false;

        ToggleSlider(_rightSlider, false);
        ToggleSlider(_leftSlider, false);

        _waitRoutine = StartCoroutine(WaitForPlayerInput());
    }

    private void Update() {
        _timeSinceTouch = Time.time - _timeOfTouch;

        if (_levelEnd) {
            _UI.SetActive(false);

            ChangeAnimation("Climb", "Idle");

            _playerIsMoving = false;

            _sceneAnimator.enabled = true;

            _soundManager.StopSound();

            _playableDirector.Play();
        } else if (_levelBegan) {
            if (_levelStart) {
                _playerIsMovingBack = false;
            } else if (_timeSinceTouch > 2f && !PlayerIsMovingBack) {
                ChangeAnimation("Climb", "Slide");

                _playerIsMoving = false;

                _playerIsMovingBack = true;

                _soundManager.StopSound();
                _soundManager.PlaySound(3);
            } else {
                if (_touchedLeft) {
                    ResetSlider(_leftSlider);
                }

                if (_touchedRight) {
                    ResetSlider(_rightSlider);
                }
            }

            if (_playerScreenPos.x < _camera.WorldToScreenPoint(_startPoint.position).x && PlayerIsMovingBack) {
                _playerIsMovingBack = false;
                _levelMid = false;
                _levelStart = true;

                StartCoroutine(Fall());
            }

            if (_playerScreenPos.x > _camera.WorldToScreenPoint(_midPoint.position).x && !_levelMid) {
                _levelMid = true;

                _soundManager.StopSound();
                _soundManager.PlaySound(5);
            }

            if (_playerScreenPos.x > _camera.WorldToScreenPoint(_endPoint.position).x) {
                _levelEnd = true;
            }
        }
    }

    private void ToggleSlider(Slider slider, bool toggle) {
        slider.interactable = toggle;
        slider.gameObject.GetComponentInChildren<SVGImage>().raycastTarget = toggle;
    }

    private IEnumerator WaitForPlayerInput() {
        _soundManager.PlaySound(1);

        ToggleSlider(_rightSlider, false);
        ToggleSlider(_leftSlider, false);

        yield return new WaitForSeconds(_waitTime);

        ToggleSlider(_rightSlider, true);
        ToggleSlider(_leftSlider, true);

        while (!_levelBegan) {
            _soundManager.PlaySound(2);

            yield return new WaitForSeconds(_waitTime * 1.5f);
        }
    }

    public void Move(Slider slider) {
        _timeOfTouch = Time.time;

        _timeSinceTouch = 0f;

        if (_animators[0].GetBool("Idle")) {
            ChangeAnimation("Idle", "Climb");
        } else {
            ChangeAnimation("Slide", "Climb");
        }

        _playerIsMoving = true;
        _playerIsMovingBack = false;

        if (slider.gameObject == _rightSlider.gameObject) {
            ToggleSlider(_rightSlider, false);

            _touchedRight = true;
        }

        if (slider.gameObject == _leftSlider.gameObject) {
            ToggleSlider(_leftSlider, false);

            _touchedLeft = true;
        }

        if (_levelStart) {
            _levelStart = false;
        }

        if (!_levelBegan) {
            StopCoroutine(_waitRoutine);
            _levelBegan = true;
        }
    }

    private void ResetSlider(Slider _slider) {
        if (_slider.value > _slider.minValue) {
            _slider.value -= _sliderSpeed * Time.deltaTime;
        }

        if (_slider.value == _slider.minValue) {
            if (_slider.gameObject == _leftSlider.gameObject) {
                ToggleSlider(_rightSlider, true);

                _touchedLeft = false;
            }

            if (_slider.gameObject == _rightSlider.gameObject) {
                ToggleSlider(_leftSlider, true);

                _touchedRight = false;
            }
        }
    }

    private void ChangeAnimation(string oldAnimation, string newAnimation) {
        for (int i = 0; i < _animators.Count; i++) {
            _animators[i].SetBool(oldAnimation, false);
            _animators[i].SetBool(newAnimation, true);
        }
    }

    private IEnumerator Fall() {
        ToggleSlider(_leftSlider, false);
        ToggleSlider(_rightSlider, false);

        ChangeAnimation("Slide", "Fall");

        yield return new WaitForSeconds(2f);

        ToggleSlider(_leftSlider, true);
        ToggleSlider(_rightSlider, true);
    }
}
