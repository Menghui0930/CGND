using UnityEngine;

public class ShieldSkillEffect : SkillEffect {
    private PlayerShield _barrier;

    private void Awake() {
        _barrier = GetComponent<PlayerShield>();
    }

    public override void ApplyEffect() { _barrier.UnlockBarrier(); }
    public override void RemoveEffect() { _barrier.LockBarrier(); }
}