using System.Data.Common;
using UnityEngine;

public class CastState : IState {
    private FSM manager;
    private Parameter parameter;
    private float timer = 3f;
    private BossRock currentRock;

    public CastState(FSM manager) {
        this.manager = manager;
        parameter = manager.parameter;
    }

    public void OnEnter() {
        Debug.Log(">>> 进入 CastState");

        parameter.anim.SetBool("Cast", true);

        // 在左边生成石头，石头自己开始往下掉
        GameObject rock = UnityEngine.Object.Instantiate(
            parameter.rockPrefabs,
            parameter.castPointLeft.position,
            Quaternion.identity
        );
        currentRock = rock.GetComponent<BossRock>();
    }

    public void OnUpdate() {
        timer -= Time.deltaTime;
        if (timer <= 0f) {
            Debug.Log("<<< CastState 结束，回到 Decision");
            //manager.TransitionState(StateType.Decision);
        }
    }

    public void OnExit() {
        timer = 2f;
        //parameter.anim.SetBool("Cast", false);
    }

    public void OnPushRock() {
        if (currentRock != null)
            currentRock.Launch();
    }
}
