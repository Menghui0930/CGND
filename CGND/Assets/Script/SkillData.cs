using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "SkillTree/Skill")]
public class SkillData : ScriptableObject {
    public string skillName;
    [TextArea(2, 5)]
    public string description;
    public Sprite icon;
    public Sprite upgradedIcon;
    public int cost;
}