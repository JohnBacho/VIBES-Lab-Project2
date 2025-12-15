using System.Collections;
using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    public TextMeshPro countdownText;
    public float startTime = 45f;

    private float currentTime;

    public void StartCountdown()
    {
        currentTime = startTime;
        StartCoroutine(TimerRoutine());
    }

    IEnumerator TimerRoutine()
    {
        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            countdownText.text = Mathf.CeilToInt(currentTime).ToString();

            yield return null;
        }

        countdownText.text = "0";
    }
}
