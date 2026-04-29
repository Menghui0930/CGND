using UnityEngine;

/// <summary>
/// 背景跟随相机
/// 
/// 使用方法：
///   1. 把此脚本挂到背景图片的 GameObject 上
///   2. 把 Main Camera 拖进 cam 栏位
///   3. 调整 offset 来设定背景相对于相机的位置
/// </summary>
public class BackgroundFollow : MonoBehaviour {
    [Header("相机")]
    public Camera cam;

    [Tooltip("背景相对于相机的偏移（Z 轴一定要设，否则背景会跑进相机里）")]
    public Vector3 offset = new Vector3(0f, 0f, 10f);

    [Header("跟随轴向")]
    public bool followX = true;
    public bool followY = true;

    void LateUpdate() {
        if (cam == null) return;

        Vector3 camPos = cam.transform.position;

        float x = followX ? camPos.x + offset.x : transform.position.x;
        float y = followY ? camPos.y + offset.y : transform.position.y;
        float z = transform.position.z; // Z 轴保持原来的（2D 不动）

        transform.position = new Vector3(x, y, z);
    }
}