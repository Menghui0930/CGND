using UnityEngine;

public class DoubleShotSkillEffect : SkillEffect {
    private PlayerBasicAttack _playerBasicAttack;

    private void Awake() {
        _playerBasicAttack = GetComponent<PlayerBasicAttack>();
    }

    public override void ApplyEffect() {
        _playerBasicAttack.UnlockDoubleShot();
    }

    public override void RemoveEffect() {
        _playerBasicAttack.LockDoubleShot();
    }
}