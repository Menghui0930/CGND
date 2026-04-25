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
        Debug.Log("SpawnSPike");
        // 生成往左的刺
        GameObject leftSpike = UnityEngine.Object.Instantiate(
            parameter.spikePrefabs,
            parameter.slamSpikeCenter.position,
            Quaternion.identity
        );
        leftSpike.GetComponent<Rigidbody2D>().linearVelocity = Vector2.left * parameter.spikeSpeed;

        // 生成往右的刺
        GameObject rightSpike = UnityEngine.Object.Instantiate(
            parameter.spikePrefabs,
            parameter.slamSpikeCenter.position,
            Quaternion.identity
        );
        rightSpike.GetComponent<Rigidbody2D>().linearVelocity = Vector2.right * parameter.spikeSpeed;

        Debug.Log("刺生成！");
    }
}
