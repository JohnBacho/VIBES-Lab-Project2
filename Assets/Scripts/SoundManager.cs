using System;
using UnityEngine;
using UnityEngine.Audio;

public enum SoundType
{
    uiButton,
    winAudio,
    lossAudio,
    minigamePointSound,
    increaseButtonSound,
    decreaseButtonSound,
    handleSound,
}

[System.Serializable]
public class AudioClips
{
    public SoundType sounds;
    public AudioClip clip;
}

namespace SoundManager
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioClips[] audioClips;
        [SerializeField] private AudioSource audioPrefab;

        private static SoundManager instance = null;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                InitializeAudioSource();
            }
        }

        private void InitializeAudioSource()
        {
            for (int i = 0; i < audioClips.Length; i++)
            {
                AudioSource src = Instantiate(audioPrefab, transform);
                src.clip = audioClips[i].clip;
                src.gameObject.name = audioClips[i].sounds.ToString();
            }
        }

        public static void PlaySound3D(
            SoundType sound,
            Vector3 position,
            float volume = 1,
            float pitch = 1
        )
        {
            AudioSource src = instance.transform
                .Find(sound.ToString())
                .GetComponent<AudioSource>();

            src.transform.position = position;
            src.volume = volume;
            src.pitch = pitch;
            src.PlayOneShot(src.clip);
        }

        public static void PlaySound3DOnce(
            SoundType sound,
            Vector3 position,
            float volume = 1,
            float pitch = 1
        )
        {
            AudioSource src = instance.transform
                .Find(sound.ToString())
                .GetComponent<AudioSource>();

            src.transform.position = position;
            src.volume = volume;
            src.pitch = pitch;
            src.Play();
        }

        public static void StopSound3D(SoundType sound)
        {
            AudioSource src = instance.transform
                .Find(sound.ToString())
                .GetComponent<AudioSource>();
                src.Stop();
        }

    }
}
