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
        // make a singleton of the music manager
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            // if an instance already exists, destroy this one
            Destroy(gameObject);
            return;
        }
    }

    void Start() {
        // Get audio component
        _audioSource = GetComponent<AudioSource>();

        // Call Play Music Function
        PlayMusic(SceneManager.GetActiveScene());

        // Listen for scene changes
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        // Play music dependinding on the scene
        PlayMusic(scene);
    }

    void PlayMusic(Scene scene) {
        // check if the scene is the final level
        if (scene.name == "finalLevel")
            SwitchToFinalLevelMusic();
        // else play normal music
        else
            SwitchToNormalMusic();
}
    // Final Level Music playing
    void SwitchToFinalLevelMusic() {
        if (_audioSource.clip != _finalLevelMusic) {
            _audioSource.Stop();
            _audioSource.clip = _finalLevelMusic;
            _audioSource.loop = true;
            _audioSource.Play();
        }
    }
    // Normal Music playing
    void SwitchToNormalMusic() {
        if (_audioSource.clip != _normalMusic) {
            _audioSource.Stop();
            _audioSource.clip = _normalMusic;
            _audioSource.loop = true;
            _audioSource.Play();
        }
    }
}
