using System;
using UnityEngine;
using UnityEngine.UI;

public class FirstSkillButton : MonoBehaviour
{
    public static event Action<FirstSkillButton> OnSkillButtonClicked; 

    private Button _button;

    private void Awake() {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClick);
    }

    private void OnClick() {
        OnSkillButtonClicked?.Invoke(this);
    }

}
