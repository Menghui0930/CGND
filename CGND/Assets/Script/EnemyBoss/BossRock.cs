using System;
using UnityEngine;

public class BossRock : MonoBehaviour {
    private Action _onDestroy;

    public void SetOnDestroy(Action callback) {
        _onDestroy = callback;
    }

    private void OnDestroy() {
        _onDestroy?.Invoke();
    }

    // 你原本的 Launch() 和落地逻辑保持不变
}