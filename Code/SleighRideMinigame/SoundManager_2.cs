using UnityEngine;

namespace Rekiviskaus {
    public class SoundManager_2 : MonoBehaviour {
        [SerializeField] private AudioSource[] _audioSources;
        [SerializeField] private AudioClip _music;
        [SerializeField] private AudioClip[] _rocketBoosts;
        [SerializeField] private AudioClip _rocketPickup;
        [SerializeField] private AudioClip _slingShot;
        [SerializeField] private AudioClip[] _trampoline;
        [SerializeField] private AudioClip _sledHitsRock;
        [SerializeField] private AudioClip[] _sledBouncesOnGround;
        [SerializeField] private AudioClip _letsGo;
        [SerializeField] private AudioClip _sledStopping;
        [SerializeField] private AudioClip _endFanfare;
        [SerializeField] private AudioClip _endBearCongrats;

        public static SoundManager_2 Instance = null;

        void Awake() {
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
                Destroy(gameObject);
        }

        private void Start() {
            PlaySound(1);
        }
        
        public void PlaySound(int i) {
            switch (i) {
                case 1:
                    _audioSources[1].clip = _music;
                    _audioSources[1].Play();
                    Debug.Log("Music Start");
                    break;
                case 2:
                    _audioSources[0].clip = _rocketBoosts[Random.Range(0, 3)];
                    _audioSources[0].Play();
                    Debug.Log("Rocket Boost");
                    break;
                case 3:
                    _audioSources[0].clip = _rocketPickup;
                    _audioSources[0].Play();
                    Debug.Log("Picked up rocket");
                    break;
                case 4:
                    _audioSources[0].clip = _slingShot;
                    _audioSources[0].Play();
                    Debug.Log("Slingshot");
                    break;
                case 5:
                    _audioSources[0].clip = _trampoline[Random.Range(0, 2)];
                    _audioSources[0].Play();
                    Debug.Log("Bounced on trampoline");
                    break;
                case 6:
                    _audioSources[0].clip = _sledHitsRock;
                    _audioSources[0].Play();
                    Debug.Log("Sled hit rock");
                    break;
                case 7:
                    _audioSources[0].clip = _sledBouncesOnGround[Random.Range(0, 2)];
                    _audioSources[0].Play();
                    Debug.Log("Sled bounces on the ground");
                    break;
                case 8:
                    _audioSources[0].clip = _letsGo;
                    _audioSources[0].Play();
                    Debug.Log("Let's go!");
                    break;
                case 9:
                    _audioSources[0].clip = _sledStopping;
                    _audioSources[0].Play();
                    Debug.Log("Game over, sled stopped");
                    break;
                case 10:
                    _audioSources[0].clip = _endFanfare;
                    _audioSources[0].Play();
                    Debug.Log("Game over, fanfare");
                    break;
                case 11:
                    _audioSources[0].clip = _endBearCongrats;
                    _audioSources[0].Play();
                    Debug.Log("Game over, bear congratulates");
                    break;
            }
        }
    }
}
