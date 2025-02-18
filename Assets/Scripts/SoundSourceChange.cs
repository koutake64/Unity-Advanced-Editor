using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class SoundSourceChange : MonoBehaviour
{
    [System.Serializable]
    public class SoundLayer
    {
        public string layerName;
        public AudioClip clip;
    }

    public List<SoundLayer> soundLayers = new List<SoundLayer>();
    private AudioSource audioSource;
    private Coroutine fadeCoroutine;
    private static SoundSourceChange currentPlaying;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[SoundSourceChange] {other.gameObject.name} がエリアに入りました (タグ: {other.tag})");

        foreach (var soundLayer in soundLayers)
        {
            Debug.Log($"[SoundSourceChange] 比較中: {soundLayer.layerName} と {other.tag}");

            if (other.CompareTag(soundLayer.layerName))
            {
                Debug.Log($"[SoundSourceChange] {soundLayer.layerName} の音源を再生");

                if (currentPlaying != null && currentPlaying != this)
                {
                    currentPlaying.StopAudio();
                }

                currentPlaying = this;
                ChangeAudioClip(soundLayer.clip);
                break;
            }
        }
    }

    private void ChangeAudioClip(AudioClip newClip)
    {
        if (newClip == null)
        {
            Debug.LogWarning("[SoundSourceChange] 新しい音源がnullです");
            return;
        }

        if (audioSource == null)
        {
            Debug.LogError("[SoundSourceChange] AudioSourceがアタッチされていません");
            return;
        }

        if (audioSource.clip == newClip)
        {
            Debug.Log("[SoundSourceChange] すでに再生中の音源です");
            return;
        }

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeAudio(newClip));
    }

    private IEnumerator FadeAudio(AudioClip newClip)
    {
        float fadeDuration = 1.5f;
        float startVolume = audioSource.volume;

        // フェードアウト
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        audioSource.clip = newClip;
        audioSource.Play();

        // フェードイン
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0, startVolume, t / fadeDuration);
            yield return null;
        }
    }

    private void StopAudio()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeOutAndStop());
    }

    private IEnumerator FadeOutAndStop()
    {
        float fadeDuration = 1.5f;
        float startVolume = audioSource.volume;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        audioSource.Stop();
    }
}
