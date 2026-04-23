using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.InputSystem.EnhancedTouch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

/// <summary>
/// 视觉小说场景播放器
/// 负责管理场景图片的切换和播放
/// </summary>
public class VisualNovelPlayer : MonoBehaviour
{
    [Header("Scene pic")]
    [Tooltip("按顺序拖入所有场景图片")]
    public List<Sprite> sceneSprites = new List<Sprite>();

    [Header("UI组件")]
    [Tooltip("用于显示场景图片的Image组件")]
    public Image sceneImage;

    [Tooltip("进度文本 (可选)")]
    public Text progressText;

    [Tooltip("提示文本 (可选)")]
    public Text hintText;

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
    private CanvasGroup canvasGroup;

    void Start()
    {
        // 初始化CanvasGroup用于淡入淡出
        if (useFadeEffect && sceneImage != null)
        {
            canvasGroup = sceneImage.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = sceneImage.gameObject.AddComponent<CanvasGroup>();
            }
        }

        // 显示第一个场景
        if (sceneSprites.Count > 0)
        {
            ShowScene(0);
        }
        else
        {
            Debug.LogWarning("场景图片列表为空！请在Inspector中添加场景图片。");
        }
    }

    void Update()
    {
        bool inputDetected = false;
        // 如果正在过渡中，不接受输入
        if (isTransitioning) return;

        // 检测输入
        // 鼠标点击
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
        if (enableTouch)
        {
            if (Touch.activeTouches.Count > 0 && Touch.activeTouches[0].phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                inputDetected = true;
            }
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
        if (index < 0 || index >= sceneSprites.Count)
        {
            Debug.LogWarning($"场景索引 {index} 超出范围！");
            return;
        }

        currentSceneIndex = index;

        if (useFadeEffect && canvasGroup != null)
        {
            StartCoroutine(FadeToScene(sceneSprites[index]));
        }
        else
        {
            // 直接切换
            sceneImage.sprite = sceneSprites[index];
            UpdateUI();
        }
    }

    /// <summary>
    /// 淡入淡出切换场景
    /// </summary>
    private IEnumerator FadeToScene(Sprite newSprite)
    {
        isTransitioning = true;

        // 淡出
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;

        // 更换图片
        sceneImage.sprite = newSprite;
        UpdateUI();

        // 短暂延迟
        yield return new WaitForSeconds(0.1f);

        // 淡入
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        isTransitioning = false;
    }

    /// <summary>
    /// 播放下一个场景
    /// </summary>
    public void NextScene()
    {
        if (currentSceneIndex < sceneSprites.Count - 1)
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
            ShowScene(currentSceneIndex - 1);
        }
        else
        {
            Debug.Log("已经是第一个场景了！");
        }
    }

    /// <summary>
    /// 重新开始
    /// </summary>
    public void RestartStory()
    {
        ShowScene(0);
    }

    /// <summary>
    /// 跳转到指定场景
    /// </summary>
    public void JumpToScene(int index)
    {
        ShowScene(index);
    }

    /// <summary>
    /// 更新UI显示
    /// </summary>
    private void UpdateUI()
    {
        // 更新进度文本
        if (progressText != null)
        {
            progressText.text = $"场景 {currentSceneIndex + 1} / {sceneSprites.Count}";
        }

        // 更新提示文本
        if (hintText != null)
        {
            if (currentSceneIndex < sceneSprites.Count - 1)
            {
                hintText.text = "点击继续 ▶";
            }
            else
            {
                hintText.text = "故事结束";
            }
        }
    }

    /// <summary>
    /// 故事结束时的回调
    /// 可以在这里添加自定义逻辑，比如显示结束界面
    /// </summary>
    private void OnStoryEnd()
    {
        Debug.Log("故事播放完毕！");
        // 在这里可以添加：
        // - 显示结束画面
        // - 播放结束音效
        // - 跳转到主菜单
        // - 显示重新开始按钮等
    }

    /// <summary>
    /// 获取当前场景索引
    /// </summary>
    public int GetCurrentSceneIndex()
    {
        return currentSceneIndex;
    }

    /// <summary>
    /// 获取场景总数
    /// </summary>
    public int GetTotalScenes()
    {
        return sceneSprites.Count;
    }

    /// <summary>
    /// 是否是最后一个场景
    /// </summary>
    public bool IsLastScene()
    {
        return currentSceneIndex >= sceneSprites.Count - 1;
    }

    /// <summary>
    /// 是否是第一个场景
    /// </summary>
    public bool IsFirstScene()
    {
        return currentSceneIndex == 0;
    }
}