using UnityEngine;

public abstract class SkillEffect : MonoBehaviour {
    public SkillData skillData; 
    public abstract void ApplyEffect();
    public abstract void RemoveEffect(); 
}