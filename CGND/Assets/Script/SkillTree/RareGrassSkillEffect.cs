using UnityEngine;

public class RareGrassSkillEffect : SkillEffect {
    public override void ApplyEffect() {
        GrassTilemapManager.instance.UnlockRareGrass();
    }

    public override void RemoveEffect() {
        GrassTilemapManager.instance.LockRareGrass();
    }
}