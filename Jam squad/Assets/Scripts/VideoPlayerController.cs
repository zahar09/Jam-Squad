using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerController : MonoBehaviour
{
    [SerializeField] private VideoClip videoClip; // ���������� ��������� ����
    [SerializeField] private RenderTexture renderTexture; // ����������� ��� 3D-��������

    private VideoPlayer videoPlayer;

    private void Start()
    {
        // ������� VideoPlayer ���������
        videoPlayer = gameObject.AddComponent<VideoPlayer>();

        // ��������� �����������
        videoPlayer.playOnAwake = false;
        videoPlayer.clip = videoClip;
        videoPlayer.renderMode = VideoRenderMode.MaterialOverride;
        videoPlayer.targetMaterialProperty = "_MainTex";
        PlayVideo();

        // ��� ��������������� �� UI
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