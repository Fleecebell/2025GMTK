using UnityEngine;
using System.Collections;

/// <summary>
/// 音乐播放管理器 - 用于管理游戏背景音乐
/// </summary>
public class MusicManager : MonoBehaviour
{
    [Header("音乐设置")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] bgmClips;
    [SerializeField] [Range(0f, 1f)] private float volume = 0.5f;
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private bool loop = true;

    // 单例
    public static MusicManager Instance { get; private set; }

    // 属性
    public bool IsPlaying => audioSource != null && audioSource.isPlaying;
    public float Volume 
    { 
        get => volume; 
        set 
        { 
            volume = Mathf.Clamp01(value);
            if (audioSource != null) audioSource.volume = volume;
        } 
    }

    private void Awake()
    {
        // 单例模式
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSource();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (playOnStart && bgmClips.Length > 0)
        {
            PlayBGM(0);
        }
    }

    /// <summary>
    /// 初始化音频源
    /// </summary>
    private void InitializeAudioSource()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.playOnAwake = false;
    }

    /// <summary>
    /// 播放BGM（通过索引）
    /// </summary>
    /// <param name="index">BGM索引</param>
    public void PlayBGM(int index)
    {
        if (index < 0 || index >= bgmClips.Length)
        {
            Debug.LogWarning($"BGM索引超出范围: {index}");
            return;
        }

        PlayBGM(bgmClips[index]);
    }

    /// <summary>
    /// 播放BGM（通过AudioClip）
    /// </summary>
    /// <param name="clip">音频片段</param>
    public void PlayBGM(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("BGM音频片段为空");
            return;
        }

        if (audioSource == null) InitializeAudioSource();

        audioSource.clip = clip;
        audioSource.Play();
        
        Debug.Log($"播放BGM: {clip.name}");
    }

    /// <summary>
    /// 暂停BGM
    /// </summary>
    public void PauseBGM()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Pause();
            Debug.Log("BGM已暂停");
        }
    }

    /// <summary>
    /// 恢复BGM
    /// </summary>
    public void ResumeBGM()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.UnPause();
            Debug.Log("BGM已恢复");
        }
    }

    /// <summary>
    /// 停止BGM
    /// </summary>
    public void StopBGM()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            Debug.Log("BGM已停止");
        }
    }

    /// <summary>
    /// 淡入播放BGM
    /// </summary>
    /// <param name="clip">音频片段</param>
    /// <param name="fadeTime">淡入时间</param>
    public void FadeInBGM(AudioClip clip, float fadeTime = 1f)
    {
        if (clip == null) return;

        StartCoroutine(FadeInCoroutine(clip, fadeTime));
    }

    /// <summary>
    /// 淡出BGM
    /// </summary>
    /// <param name="fadeTime">淡出时间</param>
    public void FadeOutBGM(float fadeTime = 1f)
    {
        StartCoroutine(FadeOutCoroutine(fadeTime));
    }

    /// <summary>
    /// 切换BGM（带淡入淡出效果）
    /// </summary>
    /// <param name="newClip">新的音频片段</param>
    /// <param name="fadeTime">切换时间</param>
    public void SwitchBGM(AudioClip newClip, float fadeTime = 1f)
    {
        StartCoroutine(SwitchBGMCoroutine(newClip, fadeTime));
    }

    /// <summary>
    /// 静音/取消静音
    /// </summary>
    /// <param name="mute">是否静音</param>
    public void SetMute(bool mute)
    {
        if (audioSource != null)
        {
            audioSource.mute = mute;
            Debug.Log(mute ? "BGM已静音" : "BGM取消静音");
        }
    }

    /// <summary>
    /// 设置循环播放
    /// </summary>
    /// <param name="shouldLoop">是否循环</param>
    public void SetLoop(bool shouldLoop)
    {
        loop = shouldLoop;
        if (audioSource != null)
        {
            audioSource.loop = shouldLoop;
        }
    }

    #region 协程方法

    /// <summary>
    /// 淡入协程
    /// </summary>
    private IEnumerator FadeInCoroutine(AudioClip clip, float fadeTime)
    {
        if (audioSource == null) InitializeAudioSource();

        audioSource.clip = clip;
        audioSource.volume = 0f;
        audioSource.Play();

        float currentTime = 0f;
        while (currentTime < fadeTime)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, volume, currentTime / fadeTime);
            yield return null;
        }

        audioSource.volume = volume;
    }

    /// <summary>
    /// 淡出协程
    /// </summary>
    private IEnumerator FadeOutCoroutine(float fadeTime)
    {
        if (audioSource == null || !audioSource.isPlaying) yield break;

        float startVolume = audioSource.volume;
        float currentTime = 0f;

        while (currentTime < fadeTime)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, currentTime / fadeTime);
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
        audioSource.volume = volume; // 恢复原音量
    }

    /// <summary>
    /// 切换BGM协程
    /// </summary>
    private IEnumerator SwitchBGMCoroutine(AudioClip newClip, float fadeTime)
    {
        // 淡出当前BGM
        if (audioSource != null && audioSource.isPlaying)
        {
            yield return StartCoroutine(FadeOutCoroutine(fadeTime * 0.5f));
        }

        // 淡入新BGM
        yield return StartCoroutine(FadeInCoroutine(newClip, fadeTime * 0.5f));
    }

    #endregion
}