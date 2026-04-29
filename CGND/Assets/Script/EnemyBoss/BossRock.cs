using UnityEngine;

public class BossRock : MonoBehaviour
{
    public float fallSpeed = 6f;
    public float flySpeed = 15f;
    private bool isLaunched = false;

    void Update() {
        if (!isLaunched) {
            // 第一阶段：从上往下掉
            transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);
            //Debug.Log("Down");
        } else {
            // 第二阶段：被推出去，往右飞
            transform.Translate(Vector2.right * flySpeed * Time.deltaTime);
            //Debug.Log("RIght");
        }
    }

    // 由 CastState 调用，切换到飞行阶段
    public void Launch() {
        isLaunched = true;
    }
}
