using UnityEngine;

public class UpgradeMagicBallSkillEffect : SkillEffect
{
    public override void ApplyEffect() { WindMagicBall.onHitBoostEnabled = true; }
    public override void RemoveEffect() { WindMagicBall.onHitBoostEnabled = false; }
}
