using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI _timerText;
    public GameObject _levelCompleteUI;
    public TextMeshProUGUI _finalTimeText;
    public TextMeshProUGUI _timeDifferenceText;
    public TextMeshProUGUI _leaderboardText;

    [Header("Colours")]
    private Color betterTime = Color.green;
    private Color worseTime = Color.red;

    [Header("Variables")]
    public float fStartTime;
    private bool bIsRunning = true;
    private List<float> lBestTimes = new List<float>();
    private string sLevelKey;

    void Start()
    {
        // set variable to time
        fStartTime = Time.time;

        // Create level key for saving
        sLevelKey = "BestTimes_" + SceneManager.GetActiveScene().name;

        // load times function
        LoadTimes();
    }

    void Update()
    {
        if (bIsRunning)
        {
            // set ui text to time
            float currentTime = Time.time - fStartTime;
            _timerText.text = currentTime.ToString("0.000");
        }
    }

    public void CompleteLevel() 
    {
        // stop time when done
        bIsRunning = false;

        // set finaltime ui and display
        float finalTime = Time.time - fStartTime;
        _finalTimeText.text = finalTime.ToString("F2");

        // get your best time and compare to time you just got
        float bestTime = lBestTimes.Count > 0 ? lBestTimes.Min() : finalTime;
        ShowTimeDifference(finalTime, bestTime);

        SaveTime(finalTime);
        ShowLeaderboard();

        // get and find the gameobjects
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        if (player != null) {
            // get components
            PlayerController movementScript = player.GetComponent<PlayerController>();
            WeaponSystem weaponScript = player.GetComponent<WeaponSystem>();
            CameraController cameraScript = camera.GetComponent<CameraController>();
            // make it so the player cant do anything when the level is done
            movementScript.enabled = false;
            weaponScript.enabled = false;
            cameraScript.enabled = false;
        }

        _levelCompleteUI.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    void ShowTimeDifference(float finalTime, float bestTime) 
    {
        float difference = finalTime - bestTime;
        if (difference == 0) {
            _timeDifferenceText.text = "New Best Time!";
            _timeDifferenceText.color = betterTime;
        }
        else if (difference < 0) {
            _timeDifferenceText.text = $"-{Mathf.Abs(difference):F2}s";
            _timeDifferenceText.color = betterTime;
        }
        else {
            _timeDifferenceText.text = $"+{difference:F2}s";
            _timeDifferenceText.color = worseTime;
        }
    }

    void SaveTime(float time) 
    {
        lBestTimes.Add(time);
        lBestTimes = lBestTimes.OrderBy(t => t).Take(10).ToList();
        PlayerPrefs.SetString(sLevelKey, string.Join(",", lBestTimes));
        PlayerPrefs.Save();
    }

    void LoadTimes() 
    {
        if (PlayerPrefs.HasKey(sLevelKey)) {
            string savedTimes = PlayerPrefs.GetString(sLevelKey);
            lBestTimes = savedTimes.Split(',').Select(float.Parse).ToList();
        }
    }

    void ShowLeaderboard() 
    {
        _leaderboardText.text = "Best Times:\n";
        for (int i = 0; i < lBestTimes.Count; i++) {
            _leaderboardText.text += (i + 1) + ". " + lBestTimes[i].ToString("F2") + "s\n";
        }
    }
}
