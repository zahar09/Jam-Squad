using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Настройки смерти")]
    [SerializeField] private float _deathDuration = 1f;
    [SerializeField] private float _restartDelay = 1f;

    [Header("Звук смерти")]
    [SerializeField] private AudioSource _playerDeadSource;
    [SerializeField] private AudioClip[] _playerDeadSounds;

    [Header("Звук появления")]
    [SerializeField] private AudioSource _plyaerRiseSource;
    [SerializeField] private AudioClip[] _plyaerRiseSounds;

    private Vector3 _originalScale;
    private bool _isDead = false;

    private void OnEnable()
    {
        PlayRandomSound(_plyaerRiseSource, _plyaerRiseSounds);
    }

    private void Awake()
    {
        _originalScale = transform.localScale;
    }

    public void Die()
    {
        if (_isDead) return;
        _isDead = true;

        PlayRandomSound(_playerDeadSource, _playerDeadSounds);

        transform.DOScale(Vector3.zero, _deathDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                StartCoroutine(RestartSceneWithDelay());
            });
    }

    private void PlayRandomSound(AudioSource audioSource, AudioClip[] audioClips)
    {
        if (audioSource == null || audioClips == null || audioClips.Length == 0)
            return;

        int randomIndex = Random.Range(0, audioClips.Length);
        audioSource.PlayOneShot(audioClips[randomIndex]);
    }

    private IEnumerator RestartSceneWithDelay()
    {
        yield return new WaitForSeconds(_restartDelay);

        // Перезагрузка текущей сцены
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}