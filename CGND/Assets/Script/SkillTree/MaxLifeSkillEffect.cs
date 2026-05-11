using UnityEngine;

public class MaxLifeSkillEffect : SkillEffect {
    private Health _health;

    private void Awake() {
        _health = GetComponentInParent<Health>();
    }

    public override void ApplyEffect() {
        _health.AddMaxLife(2);
    }

    public override void RemoveEffect() {
    }
}