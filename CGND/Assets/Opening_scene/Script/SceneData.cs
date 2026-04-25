using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// 场景数据类 - 支持图片和视频
/// </summary>
[System.Serializable]
public class SceneData
    {
        public enum SceneType
        {
            Image,      // 静态图片
            Video       // 视频
        }

        [Header("场景类型")]
        public SceneType sceneType = SceneType.Image;

        [Header("图片场景")]
        [Tooltip("场景图片（当类型为Image时使用）")]
        public Sprite sceneSprite;

        [Header("视频场景")]
        [Tooltip("视频文件（当类型为Video时使用）")]
        public VideoClip videoClip;

        [Tooltip("视频是否循环播放")]
        public bool loopVideo = false;

        [Tooltip("视频播放完后是否自动跳到下一个场景")]
        public bool autoNextAfterVideo = false;

        [Tooltip("视频播放期间是否允许点击跳过")]
        public bool canSkipVideo = true;

        [Header("场景名称（可选）")]
        public string sceneName = "";

        /// <summary>
        /// 是否是图片场景
        /// </summary>
        public bool IsImage()
        {
            return sceneType == SceneType.Image;
        }

        /// <summary>
        /// 是否是视频场景
        /// </summary>
        public bool IsVideo()
        {
            return sceneType == SceneType.Video;
        }
    }
