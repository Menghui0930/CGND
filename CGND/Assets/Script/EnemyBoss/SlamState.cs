using System.Data.Common;
using UnityEngine;

public class SlamState : IState {
    private FSM manager;
    private Parameter parameter;
    private float timer = 2f;

    public SlamState(FSM manager) {
        this.manager = manager;
        parameter = manager.parameter;
    }

    public void OnEnter() {
        Debug.Log(">>> 进入 SlamState");
        parameter.anim.SetBool("Slam",true);
    }

    public void OnUpdate() {
        timer -= Time.deltaTime;
        if (timer <= 0f) {
            Debug.Log("<<< SlamState 结束，回到 Decision");
            //manager.TransitionState(StateType.Decision);
        }
    }

    public void OnExit() {
        timer = 2f;
        //parameter.anim.SetBool("Slam",false);
    }

    public void OnSpawnSpike() {
        manager.StartCoroutine(SpawnSpikeWave());
    }

    private System.Collections.IEnumerator SpawnSpikeWave() {

        // 先在原地生成一个
        GameObject center = SpawnSpike(parameter.slamSpikeCenter.position);
        UnityEngine.Object.Destroy(center, parameter.spikeLifetime);

        // 往左往右同时扩散
        for (int i = 1; i <= parameter.spikeCount; i++) {
            yield return new WaitForSeconds(parameter.spawnDelay);

            Vector3 leftPos = parameter.slamSpikeCenter.position + Vector3.left * parameter.spacing * i;
            Vector3 rightPos = parameter.slamSpikeCenter.position + Vector3.right * parameter.spacing * i;

            GameObject left = SpawnSpike(leftPos);
            GameObject right = SpawnSpike(rightPos);

            UnityEngine.Object.Destroy(left, parameter.spikeLifetime);
            UnityEngine.Object.Destroy(right, parameter.spikeLifetime);
        }
    }

    private GameObject SpawnSpike(Vector3 position) {
        return UnityEngine.Object.Instantiate(
            parameter.spikePrefabs,
            position,
            Quaternion.identity
        );
    }
}
