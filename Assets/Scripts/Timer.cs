using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.UI;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public GameObject levelCompleteUI;
    public TextMeshProUGUI finalTimeText;
    public TextMeshProUGUI leaderboardText;

    public float startTime;
    private bool isRunning = true;
    private List<float> bestTimes = new List<float>();

    void Start()
    {
        startTime = Time.time;
        LoadTimes();
    }

    void Update()
    {
        if (isRunning)
        {
            float currentTime = Time.time - startTime;
            timerText.text = currentTime.ToString("0.000");
        }
    }

    public void CompleteLevel() 
    {
        isRunning = false;
        float finalTime = Time.time - startTime;
        finalTimeText.text = "Completed in: " + finalTime.ToString("F2");

        SaveTime(finalTime);
        ShowLeaderboard();

        levelCompleteUI.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    void SaveTime(float time) 
    {
        bestTimes.Add(time);
        bestTimes = bestTimes.OrderBy(t => t).Take(10).ToList();
        PlayerPrefs.SetString("BestTimes", string.Join(",", bestTimes));
        PlayerPrefs.Save();
    }

    void LoadTimes() 
    {
        if (PlayerPrefs.HasKey("BestTimes")) {
            string savedTimes = PlayerPrefs.GetString("BestTimes");
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
