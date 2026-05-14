using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 挂在每个存档槽的 Prefab 上。
/// MainMenu 动态生成时调用 Setup() 填入数据。
/// </summary>
public class SaveSlotUI : MonoBehaviour {
    [Header("UI References")]
    public TextMeshProUGUI slotNumberText;   // 例："存档 1"
    public TextMeshProUGUI dateTimeText;     // 存档时间
    public TextMeshProUGUI playTimeText;     // 游玩时间
    public TextMeshProUGUI skillPointsText;  // 技能点数
    public TextMeshProUGUI highestLevelText; // 最高关卡
    public Button selectButton;             // 点击选择这个存档
    public Button deleteButton;             // 点击删除这个存档（可选）

    private int slotIndex;
    private System.Action<int> onSelect;
    private System.Action<int> onDelete;

    // ── 填入存档资料 ──────────────────────────────────────────────────────────
    public void Setup(GameSaveData data, System.Action<int> selectCallback, System.Action<int> deleteCallback = null) {
        slotIndex = data.saveSlotIndex;
        onSelect = selectCallback;
        onDelete = deleteCallback;

        if (slotNumberText) slotNumberText.text = $"存档 {data.saveSlotIndex + 1}";
        if (dateTimeText) dateTimeText.text = data.saveDateTime;
        if (playTimeText) playTimeText.text = FormatTime(data.totalPlayTime);
        if (skillPointsText) skillPointsText.text = $"SP: {data.skillPoints}";
        if (highestLevelText) {
            highestLevelText.text = data.highestLevelReached == 0
                ? "Tutorial"
                : $"Level {data.highestLevelReached}";
        }

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => onSelect?.Invoke(slotIndex));

        if (deleteButton != null) {
            deleteButton.gameObject.SetActive(deleteCallback != null);
            deleteButton.onClick.RemoveAllListeners();
            deleteButton.onClick.AddListener(() => onDelete?.Invoke(slotIndex));
        }
    }

    // ── 格式化游玩时间 ─────────────────────────────────────────────────────────
    private string FormatTime(float seconds) {
        int h = (int)(seconds / 3600);
        int m = (int)(seconds % 3600 / 60);
        int s = (int)(seconds % 60);
        return h > 0 ? $"{h}h {m:D2}m" : $"{m}m {s:D2}s";
    }
}
