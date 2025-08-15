using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioMixerController : MonoBehaviour
{
    [System.Serializable]
    public class AudioChannel
    {
        public string name;
        public Slider slider;
        public string mixerParameter;
    }

    [Header("Настройки")]
    public AudioMixer mixer;
    public AudioChannel[] channels;

    private void Start()
    {
        InitializeSliders();
    }

    private void InitializeSliders()
    {
        foreach (var channel in channels)
        {
            if (channel.slider == null)
            {
                Debug.LogError($"Слайдер для {channel.name} не назначен!", this);
                continue;
            }

            // Загружаем текущее значение из микшера
            if (mixer.GetFloat(channel.mixerParameter, out float dbValue))
            {
                // Конвертируем dB в значение 0-1
                channel.slider.value = DBToLinear(dbValue);
            }
            else
            {
                Debug.LogError($"Не удалось получить параметр {channel.mixerParameter}", this);
                continue;
            }

            // Назначаем обработчик изменения
            channel.slider.onValueChanged.AddListener((value) =>
            {
                SetVolume(channel.mixerParameter, value);
            });
        }
    }

    private void SetVolume(string parameter, float linearValue)
    {
        // Конвертируем 0-1 в dB
        float dbValue = LinearToDB(linearValue);

        // Устанавливаем громкость
        if (!mixer.SetFloat(parameter, dbValue))
        {
            Debug.LogError($"Не удалось установить {parameter}", this);
        }

        Debug.Log($"{parameter} установлен на {dbValue}dB (input: {linearValue})");
    }

    private float LinearToDB(float linear)
    {
        return linear <= 0.0001f ? -80f : Mathf.Log10(linear) * 20f;
    }

    private float DBToLinear(float db)
    {
        return Mathf.Pow(10f, db / 20f);
    }

    // Метод для проверки подключения
    public void TestConnection()
    {
        foreach (var channel in channels)
        {
            if (mixer.GetFloat(channel.mixerParameter, out float value))
            {
                Debug.Log($"{channel.mixerParameter} = {value}dB");
            }
            else
            {
                Debug.LogError($"{channel.mixerParameter} не доступен!");
            }
        }
    }
}