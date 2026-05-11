using UnityEngine;
using System.Collections.Generic;

public class CastState : IState {
    private FSM manager;
    private Parameter parameter;

    // 追踪每个点的石头
    private GameObject[] spawnedRocks;
    private List<int> availableIndexes = new List<int>();

    public CastState(FSM manager) {
        this.manager = manager;
        parameter = manager.parameter;
        spawnedRocks = new GameObject[4];
    }

    public void OnEnter() {
        Debug.Log(">>> 进入 CastState");
        parameter.anim.SetBool("Cast", true);
    }

    public void OnUpdate() { }

    public void OnExit() {
        parameter.anim.SetBool("Cast", false);
    }

    // 由 Animation Event 调用 → FSM.PushRock() → 这里
    public void OnSpawnRock() {
        int index = GetAvailableIndex();
        if (index == -1) return;

        GameObject rock = UnityEngine.Object.Instantiate(
            parameter.rockPrefabs,
            parameter.castSpawnPoints[index].position,
            Quaternion.identity
        );

        spawnedRocks[index] = rock;

        // 石头落地后通知清除记录
        BossRock bossRock = rock.GetComponent<BossRock>();
        bossRock?.SetOnDestroy(() => spawnedRocks[index] = null);
    }

    private int GetAvailableIndex() {
        // 收集目前没有石头的点
        availableIndexes.Clear();
        for (int i = 0; i < spawnedRocks.Length; i++) {
            if (spawnedRocks[i] == null)
                availableIndexes.Add(i);
        }

        // 全部都有石头 → 清空全部重来
        if (availableIndexes.Count == 0) {
            ResetAllRocks();
            for (int i = 0; i < spawnedRocks.Length; i++)
                availableIndexes.Add(i);
        }

        // 随机选一个可用的点
        int randomIndex = availableIndexes[Random.Range(0, availableIndexes.Count)];
        return randomIndex;
    }

    private void ResetAllRocks() {
        for (int i = 0; i < spawnedRocks.Length; i++) {
            if (spawnedRocks[i] != null) {
                UnityEngine.Object.Destroy(spawnedRocks[i]);
                spawnedRocks[i] = null;
            }
        }
        Debug.Log("四个点全满，重置所有石头");
    }

    public void Reset() {
        ResetAllRocks();
    }
}