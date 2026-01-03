using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayOffMusic : MonoBehaviour
{
    [SerializeField] private AudioSource playOffAudioSource;
    [SerializeField] private GameObject EffortTask;
    private int offMusicToken = 0;
    private Coroutine playOffMusicCoroutine;
    

    public void StartPlayOffMusic(float delay, int token)
    {
        offMusicToken = token;
        playOffMusicCoroutine = StartCoroutine(RunPlayOffMusic(delay, token));
    }
    IEnumerator RunPlayOffMusic(float delay, int token)
    {
        Debug.Log("Attempting to play off music");
        yield return new WaitForSeconds(delay);
        if (token != offMusicToken)
            yield break;

        if (EffortTask.activeSelf)
            yield break;

        playOffAudioSource.Play();
    }



    public void CancelOffMusic()
    {
        if (playOffMusicCoroutine != null)
        {
            StopCoroutine(playOffMusicCoroutine);
            playOffMusicCoroutine = null;
        }

        if (playOffAudioSource.isPlaying)
            playOffAudioSource.Stop();
    }
}
