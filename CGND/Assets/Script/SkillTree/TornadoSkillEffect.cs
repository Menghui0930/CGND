using System.Threading;
using UnityEngine;

public class TornadoSkillEffect : SkillEffect
{
    private PlayerAttack _playerAttack;

    private void Awake() {
        _playerAttack = GetComponent<PlayerAttack>();
    }

    public override void ApplyEffect() { _playerAttack.UnlockTornado(); }
    public override void RemoveEffect() { _playerAttack.LockTornado(); }
}
