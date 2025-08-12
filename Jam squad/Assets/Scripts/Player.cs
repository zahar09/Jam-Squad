using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Настройки смерти")]
    [SerializeField] private float _deathDuration = 1f;
    [SerializeField] private float _restartDelay = 1f;

    [Header("Звук смерти")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _deathSounds;

    private Vector3 _originalScale;
    private bool _isDead = false;

    private void Awake()
    {
        _originalScale = transform.localScale;
    }

    public void Die()
    {
        if (_isDead) return;
        _isDead = true;

        // Проигрываем случайный звук смерти
        PlayRandomDeathSound();

        // Анимация уменьшения
        transform.DOScale(Vector3.zero, _deathDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                StartCoroutine(RestartSceneWithDelay());
            });
    }

    private void PlayRandomDeathSound()
    {
        if (_audioSource == null || _deathSounds == null || _deathSounds.Length == 0)
            return;

        int randomIndex = Random.Range(0, _deathSounds.Length);
        _audioSource.PlayOneShot(_deathSounds[randomIndex]);
    }

    private IEnumerator RestartSceneWithDelay()
    {
        yield return new WaitForSeconds(_restartDelay);

        // Перезагрузка текущей сцены
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}