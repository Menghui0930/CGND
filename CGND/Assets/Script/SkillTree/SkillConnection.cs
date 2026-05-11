using UnityEngine;
using UnityEngine.UI;

public class SkillConnection : MonoBehaviour {
    [Header("Sprites")]
    [SerializeField] private Sprite normalLine;
    [SerializeField] private Sprite glowLine;

    [Header("Connected Skills")]
    [SerializeField] private SkillButton sourceSkill;  // 这条线的起点
    [SerializeField] private SkillButton targetSkill;  // 这条线的终点

    private Image _lineImage;

    private void Awake() {
        _lineImage = GetComponent<Image>();
    }

    private void Start() {
        UpdateVisual();
    }

    public void UpdateVisual() {
        bool sourceActive = sourceSkill == null || sourceSkill.isActive;
        bool targetActive = targetSkill != null && targetSkill.isActive;

        _lineImage.sprite = (sourceActive && targetActive) ? glowLine : normalLine;
    }
}