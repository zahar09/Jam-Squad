using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CollectableObj : MonoBehaviour
{
    [Header("звук появления нейрона")]
    [SerializeField] private AudioSource _neuronRiseSource;
    [SerializeField] private AudioClip[] _neuronRiseSounds;
    public string type;

    private void OnEnable()
    {
        PlayRandomSound(_neuronRiseSounds, _neuronRiseSource);
    }
    public void PlayRandomSound(AudioClip[] clips, AudioSource audioSource)
    {
        if (audioSource == null || clips == null || clips.Length == 0) return;

        int randomIndex = Random.Range(0, clips.Length);
        audioSource.PlayOneShot(clips[randomIndex]);
    }
}
