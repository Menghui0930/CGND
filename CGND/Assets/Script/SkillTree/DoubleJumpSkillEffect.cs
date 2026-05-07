using UnityEngine;

public class DoubleJumpSkillEffect : SkillEffect {
    private PlayerJump _playerJump;

    private void Awake() {
        _playerJump = GetComponent<PlayerJump>();
        if (_playerJump == null)
            Debug.LogError("[DoubleJump] 找不到 PlayerJump 组件！");
        else
            Debug.Log("[DoubleJump] 成功找到 PlayerJump");
    }

    public override void ApplyEffect() {
        Debug.Log("[DoubleJump] ApplyEffect 被调用了");
        if (_playerJump == null) {
            Debug.LogError("[DoubleJump] _playerJump 是 null，无法解锁双跳！");
            return;
        }
        _playerJump.UnlockDoubleJump();
        Debug.Log("[DoubleJump] UnlockDoubleJump 已执行");
    }

    public override void RemoveEffect() { }
}