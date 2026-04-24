using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using UnityEngine.UI;
using UnityEngine.Video;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

/// <summary>
/// 视觉小说场景播放器 - 支持图片和视频混合
/// </summary>
public class VisualNovelPlayer : MonoBehaviour
{
    [Header("场景列表")]
    [Tooltip("按顺序添加所有场景（图片或视频）")]
    public List<SceneData> scenes = new List<SceneData>();

    [Header("UI组件")]
    [Tooltip("用于显示图片场景的Image组件")]
    public Image sceneImage;

    [Tooltip("用于显示视频场景的RawImage组件")]
    public RawImage videoImage;

    [Tooltip("背景Image（用于填充空白区域）")]
    public Image backgroundImage;

    [Header("视频播放器")]
    [Tooltip("VideoPlayer组件（会自动创建）")]
    public VideoPlayer videoPlayer;

    [Header("显示模式")]
    [Tooltip("图片填充模式")]
    public ImageFillMode fillMode = ImageFillMode.Fill;

    [Tooltip("背景颜色")]
    public Color backgroundColor = Color.black;

    [Header("淡入淡出设置")]
    [Tooltip("淡入淡出持续时间（秒）")]
    public float fadeDuration = 0.5f;

    [Tooltip("是否使用淡入淡出效果")]
    public bool useFadeEffect = true;

    [Header("输入设置")]
    [Tooltip("是否启用鼠标点击")]
    public bool enableMouseClick = true;

    [Tooltip("是否启用键盘空格键")]
    public bool enableSpaceKey = true;

    [Tooltip("是否启用触摸输入（移动端）")]
    public bool enableTouch = true;

    private int currentSceneIndex = 0;
    private bool isTransitioning = false;
    private bool isPlayingVideo = false;
    private CanvasGroup imageCanvasGroup;
    private CanvasGroup videoCanvasGroup;

    public enum ImageFillMode
    {
        Fit,            // 完整显示，保持比例
        Fill,           // 填满屏幕，保持比例
        Stretch         // 拉伸填满
    }

    void Start()
    {
        // 设置背景颜色
        if (backgroundImage != null)
        {
            backgroundImage.color = backgroundColor;
        }
        else if (Camera.main != null)
        {
            Camera.main.backgroundColor = backgroundColor;
        }

        // 初始化VideoPlayer
        SetupVideoPlayer();

        // 初始化CanvasGroup用于淡入淡出
        if (useFadeEffect)
        {
            if (sceneImage != null)
            {
                imageCanvasGroup = sceneImage.GetComponent<CanvasGroup>();
                if (imageCanvasGroup == null)
                {
                    imageCanvasGroup = sceneImage.gameObject.AddComponent<CanvasGroup>();
                }
            }

            if (videoImage != null)
            {
                videoCanvasGroup = videoImage.GetComponent<CanvasGroup>();
                if (videoCanvasGroup == null)
                {
                    videoCanvasGroup = videoImage.gameObject.AddComponent<CanvasGroup>();
                }
            }
        }

        // 应用填充模式
        ApplyFillMode();

        // 初始状态：隐藏视频，显示图片
        if (videoImage != null) videoImage.gameObject.SetActive(false);
        if (sceneImage != null) sceneImage.gameObject.SetActive(true);

        // 启用增强触摸支持（新输入系统）
        if (enableTouch)
        {
            EnhancedTouchSupport.Enable();
        }

        // 显示第一个场景
        if (scenes.Count > 0)
        {
            ShowScene(0);
        }
        else
        {
            Debug.LogWarning("场景列表为空！请在Inspector中添加场景。");
        }
    }

    /// <summary>
    /// 设置VideoPlayer组件
    /// </summary>
    void SetupVideoPlayer()
    {
        if (videoPlayer == null)
        {
            videoPlayer = gameObject.GetComponent<VideoPlayer>();
            if (videoPlayer == null)
            {
                videoPlayer = gameObject.AddComponent<VideoPlayer>();
            }
        }

        // 配置VideoPlayer
        videoPlayer.playOnAwake = false;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.aspectRatio = VideoAspectRatio.FitInside;

        // 创建RenderTexture
        if (videoPlayer.targetTexture == null)
        {
            RenderTexture rt = new RenderTexture(1920, 1080, 0);
            videoPlayer.targetTexture = rt;
            if (videoImage != null)
            {
                videoImage.texture = rt;
            }
        }

        // 监听视频播放完成事件
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    /// <summary>
    /// 应用图片填充模式
    /// </summary>
    private void ApplyFillMode()
    {
        // 为图片Image设置填充模式
        if (sceneImage != null)
        {
            ApplyFillModeToImage(sceneImage);
        }

        // 为视频RawImage设置填充模式
        if (videoImage != null)
        {
            ApplyFillModeToRawImage(videoImage);
        }
    }

    private void ApplyFillModeToImage(Image img)
    {
        switch (fillMode)
        {
            case ImageFillMode.Fit:
                img.preserveAspect = true;
                var fitterFit = img.GetComponent<AspectRatioFitter>();
                if (fitterFit == null)
                    fitterFit = img.gameObject.AddComponent<AspectRatioFitter>();
                fitterFit.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
                break;

            case ImageFillMode.Fill:
                img.preserveAspect = true;
                var fitterFill = img.GetComponent<AspectRatioFitter>();
                if (fitterFill == null)
                    fitterFill = img.gameObject.AddComponent<AspectRatioFitter>();
                fitterFill.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
                break;

            case ImageFillMode.Stretch:
                img.preserveAspect = false;
                var fitterStretch = img.GetComponent<AspectRatioFitter>();
                if (fitterStretch != null)
                    Destroy(fitterStretch);
                break;
        }
    }

    private void ApplyFillModeToRawImage(RawImage rawImg)
    {
        switch (fillMode)
        {
            case ImageFillMode.Fit:
                var fitterFit = rawImg.GetComponent<AspectRatioFitter>();
                if (fitterFit == null)
                    fitterFit = rawImg.gameObject.AddComponent<AspectRatioFitter>();
                fitterFit.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
                break;

            case ImageFillMode.Fill:
                var fitterFill = rawImg.GetComponent<AspectRatioFitter>();
                if (fitterFill == null)
                    fitterFill = rawImg.gameObject.AddComponent<AspectRatioFitter>();
                fitterFill.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
                break;

            case ImageFillMode.Stretch:
                var fitterStretch = rawImg.GetComponent<AspectRatioFitter>();
                if (fitterStretch != null)
                    Destroy(fitterStretch);
                break;
        }
    }

    void Update()
    {
        // 如果正在过渡中，不接受输入
        if (isTransitioning) return;

        // 检测输入
        bool inputDetected = false;

        if (enableMouseClick && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            inputDetected = true;
        }

        // 空格键
        if (enableSpaceKey && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            inputDetected = true;
        }

        // 触摸输入（移动端）- 需要启用 EnhancedTouchSupport
        if (enableTouch && Touch.activeTouches.Count > 0 && Touch.activeTouches[0].phase == UnityEngine.InputSystem.TouchPhase.Began)
        {
            inputDetected = true;
        }

        // 如果检测到输入，播放下一个场景
        if (inputDetected)
        {
            NextScene();
        }
    }

    /// <summary>
    /// 显示指定索引的场景
    /// </summary>
    public void ShowScene(int index)
    {
        if (index < 0 || index >= scenes.Count)
        {
            Debug.LogWarning($"场景索引 {index} 超出范围！");
            return;
        }

        currentSceneIndex = index;
        SceneData sceneData = scenes[index];

        if (sceneData.IsImage())
        {
            ShowImageScene(sceneData);
        }
        else if (sceneData.IsVideo())
        {
            ShowVideoScene(sceneData);
        }
    }

    /// <summary>
    /// 显示图片场景
    /// </summary>
    void ShowImageScene(SceneData sceneData)
    {
        if (useFadeEffect)
        {
            StartCoroutine(FadeToImageScene(sceneData.sceneSprite));
        }
        else
        {
            // 停止视频
            StopVideo();

            // 切换显示
            if (videoImage != null) videoImage.gameObject.SetActive(false);
            if (sceneImage != null)
            {
                sceneImage.gameObject.SetActive(true);
                sceneImage.sprite = sceneData.sceneSprite;
            }
        }
    }

    /// <summary>
    /// 显示视频场景
    /// </summary>
    void ShowVideoScene(SceneData sceneData)
    {
        if (useFadeEffect)
        {
            StartCoroutine(FadeToVideoScene(sceneData));
        }
        else
        {
            // 切换显示
            if (sceneImage != null) sceneImage.gameObject.SetActive(false);
            if (videoImage != null) videoImage.gameObject.SetActive(true);

            // 播放视频
            PlayVideo(sceneData);
        }
    }

    /// <summary>
    /// 淡入淡出切换到图片场景
    /// </summary>
    IEnumerator FadeToImageScene(Sprite sprite)
    {
        isTransitioning = true;

        // 淡出当前内容
        CanvasGroup currentGroup = isPlayingVideo ? videoCanvasGroup : imageCanvasGroup;
        if (currentGroup != null)
        {
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                currentGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                yield return null;
            }
            currentGroup.alpha = 0f;
        }

        // 停止视频
        StopVideo();

        // 切换显示
        if (videoImage != null) videoImage.gameObject.SetActive(false);
        if (sceneImage != null)
        {
            sceneImage.gameObject.SetActive(true);
            sceneImage.sprite = sprite;
        }

        yield return new WaitForSeconds(0.1f);

        // 淡入图片
        if (imageCanvasGroup != null)
        {
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                imageCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
                yield return null;
            }
            imageCanvasGroup.alpha = 1f;
        }

        isTransitioning = false;
    }

    /// <summary>
    /// 淡入淡出切换到视频场景
    /// </summary>
    IEnumerator FadeToVideoScene(SceneData sceneData)
    {
        isTransitioning = true;

        // 淡出当前内容
        CanvasGroup currentGroup = isPlayingVideo ? videoCanvasGroup : imageCanvasGroup;
        if (currentGroup != null)
        {
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                currentGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                yield return null;
            }
            currentGroup.alpha = 0f;
        }

        // 切换显示
        if (sceneImage != null) sceneImage.gameObject.SetActive(false);
        if (videoImage != null) videoImage.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        // 开始播放视频
        PlayVideo(sceneData);

        // 淡入视频
        if (videoCanvasGroup != null)
        {
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                videoCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
                yield return null;
            }
            videoCanvasGroup.alpha = 1f;
        }

        isTransitioning = false;
    }

    /// <summary>
    /// 播放视频
    /// </summary>
    void PlayVideo(SceneData sceneData)
    {
        if (videoPlayer == null || sceneData.videoClip == null) return;

        videoPlayer.clip = sceneData.videoClip;
        videoPlayer.isLooping = sceneData.loopVideo;
        videoPlayer.Play();
        isPlayingVideo = true;

        Debug.Log($"开始播放视频: {sceneData.sceneName}");
    }

    /// <summary>
    /// 停止视频
    /// </summary>
    void StopVideo()
    {
        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
        }
        isPlayingVideo = false;
    }

    /// <summary>
    /// 视频播放完成回调
    /// </summary>
    void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("视频播放完成");

        SceneData currentScene = scenes[currentSceneIndex];

        // 如果设置了自动切换下一个场景
        if (currentScene.autoNextAfterVideo)
        {
            isPlayingVideo = false;
            NextScene();
        }
        else
        {
            isPlayingVideo = false;
        }
    }

    /// <summary>
    /// 播放下一个场景
    /// </summary>
    public void NextScene()
    {
        if (currentSceneIndex < scenes.Count - 1)
        {
            ShowScene(currentSceneIndex + 1);
        }
        else
        {
            Debug.Log("已经是最后一个场景了！");
            OnStoryEnd();
        }
    }

    /// <summary>
    /// 播放上一个场景
    /// </summary>
    public void PreviousScene()
    {
        if (currentSceneIndex > 0)
        {
            StopVideo();
            ShowScene(currentSceneIndex - 1);
        }
    }

    /// <summary>
    /// 重新开始
    /// </summary>
    public void RestartStory()
    {
        StopVideo();
        ShowScene(0);
    }

    /// <summary>
    /// 故事结束回调
    /// </summary>
    void OnStoryEnd()
    {
        Debug.Log("故事播放完毕！");
    }

    void OnDestroy()
    {
        // 清理VideoPlayer事件
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }

        // 禁用增强触摸支持
        if (enableTouch)
        {
            EnhancedTouchSupport.Disable();
        }
    }
}