using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerController : MonoBehaviour
{
    [SerializeField] private VideoClip videoClip; // Перетащите видеофайл сюда
    [SerializeField] private RenderTexture renderTexture; // Опционально для 3D-объектов

    private VideoPlayer videoPlayer;

    private void Start()
    {
        // Создаем VideoPlayer компонент
        videoPlayer = gameObject.AddComponent<VideoPlayer>();

        // Настройки видеоплеера
        videoPlayer.playOnAwake = false;
        videoPlayer.clip = videoClip;
        videoPlayer.renderMode = VideoRenderMode.MaterialOverride;
        videoPlayer.targetMaterialProperty = "_MainTex";
        PlayVideo();

        // Для воспроизведения на UI
        // videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        // videoPlayer.targetTexture = renderTexture;
    }

    public void PlayVideo()
    {
        videoPlayer.Play();
    }

    public void StopVideo()
    {
        videoPlayer.Stop();
    }
}