using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public GameObject levelCompleteUI;
    public TextMeshProUGUI finalTimeText;
    public TextMeshProUGUI timeDifferenceText;
    public TextMeshProUGUI leaderboardText;
    private Color betterTime = Color.green;
    private Color worseTime = Color.red;

    public float startTime;
    private bool isRunning = true;
    private List<float> bestTimes = new List<float>();
    private string levelKey;

    void Start()
    {
        // set variable to time
        startTime = Time.time;

        // Create level key for saving
        levelKey = "BestTimes_" + SceneManager.GetActiveScene().name;

        // load times function
        LoadTimes();
    }

    void Update()
    {
        if (isRunning)
        {
            // set ui text to time
            float currentTime = Time.time - startTime;
            timerText.text = currentTime.ToString("0.000");
        }
    }

    public void CompleteLevel() 
    {
        // stop time when done
        isRunning = false;

        // set finaltime ui and display
        float finalTime = Time.time - startTime;
        finalTimeText.text = finalTime.ToString("F2");

        // get your best time and compare to time you just got
        float bestTime = bestTimes.Count > 0 ? bestTimes.Min() : finalTime;
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

        levelCompleteUI.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    void ShowTimeDifference(float finalTime, float bestTime) 
    {
        float difference = finalTime - bestTime;
        if (difference == 0) {
            timeDifferenceText.text = "New Best Time!";
            timeDifferenceText.color = betterTime;
        }
        else if (difference < 0) {
            timeDifferenceText.text = $"-{Mathf.Abs(difference):F2}s";
            timeDifferenceText.color = betterTime;
        }
        else {
            timeDifferenceText.text = $"+{difference:F2}s";
            timeDifferenceText.color = worseTime;
        }
    }

    void SaveTime(float time) 
    {
        bestTimes.Add(time);
        bestTimes = bestTimes.OrderBy(t => t).Take(10).ToList();
        PlayerPrefs.SetString(levelKey, string.Join(",", bestTimes));
        PlayerPrefs.Save();
    }

    void LoadTimes() 
    {
        if (PlayerPrefs.HasKey(levelKey)) {
            string savedTimes = PlayerPrefs.GetString(levelKey);
            bestTimes = savedTimes.Split(',').Select(float.Parse).ToList();
        }
    }

    void ShowLeaderboard() 
    {
        leaderboardText.text = "Best Times:\n";
        for (int i = 0; i < bestTimes.Count; i++) {
            leaderboardText.text += (i + 1) + ". " + bestTimes[i].ToString("F2") + "s\n";
        }
    }
}
