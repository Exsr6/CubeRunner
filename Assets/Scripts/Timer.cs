using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.UI;
using TMPro;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timerText;

    public float currentTime;
    public bool TimerRunning;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (TimerRunning)
        {
            currentTime = currentTime += Time.deltaTime;
            timerText.text = currentTime.ToString("0.000");
        }
        else
        {
            timerText.text = currentTime.ToString("0.000");
        }
    }
}
