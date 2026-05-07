using UnityEngine;

public class DashSkillEffect : SkillEffect {
    private PlayerDash _playerDash;

    private void Awake() {
        _playerDash = GetComponent<PlayerDash>();
    }

    public override void ApplyEffect() {
        _playerDash.UnlockDash();
    }

    public override void RemoveEffect() {
        _playerDash.LockDash();
    }
}