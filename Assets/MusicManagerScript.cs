using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManagerScript : MonoBehaviour {
    [Header("References")]
    public AudioClip _normalMusic;
    public AudioClip _finalLevelMusic;
    private AudioSource _audioSource;

    private static MusicManagerScript instance;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
            return;
        }
    }

    // Start is called before the first frame update
    void Start() {
        _audioSource = GetComponent<AudioSource>();

        PlayMusic(SceneManager.GetActiveScene());

        // Listen for scene changes
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        PlayMusic(scene);
    }

    void PlayMusic(Scene scene) {
        if (scene.name == "finalLevel")
            SwitchToFinalLevelMusic();
        else
            SwitchToNormalMusic();
}
    void SwitchToFinalLevelMusic() {
        if (_audioSource.clip != _finalLevelMusic) {
            _audioSource.Stop();
            _audioSource.clip = _finalLevelMusic;
            _audioSource.loop = true;
            _audioSource.Play();
        }
    }

    void SwitchToNormalMusic() {
        if (_audioSource.clip != _normalMusic) {
            _audioSource.Stop();
            _audioSource.clip = _normalMusic;
            _audioSource.loop = true;
            _audioSource.Play();
        }
    }
}
