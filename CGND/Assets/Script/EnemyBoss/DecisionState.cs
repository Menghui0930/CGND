using UnityEngine;

public class DecisionState : IState {
    private FSM manager;
    private Parameter parameter;

    private float waitTimer;
    private bool isWaiting;

    private StateType nextState;
    private Vector3 targetPosition;  // 下一个动作的起始点

    private StateType lastState = StateType.Decision;

    private (StateType state, float weight)[] actionTable = {
        (StateType.Fly,   30f),
        (StateType.Slam,  25f),
        (StateType.Cast,  30f),
        // Shield 不放进随机池，由外部触发
    };

    public DecisionState(FSM manager) {
        this.manager = manager;
        parameter = manager.parameter;
    }

    public void OnEnter() {
        nextState = PickNext();                       // 先抽好下一个动作
        targetPosition = GetStartPosition(nextState); // 拿到对应起始点
        Debug.Log($"下一个动作：{nextState}，目标位置：{targetPosition}，当前位置：{manager.transform.position}");
        parameter.anim.applyRootMotion = true;

        // 每次进入 Decision 就开始等待
        waitTimer = parameter.waitDuration;
        isWaiting = true;
    }

    public void OnUpdate() {
        if (!isWaiting) return;
        waitTimer -= Time.deltaTime;

        if (waitTimer <= 0f) {
            manager.TransitionTo(targetPosition);
        }

        bool arrivedAtStart = Vector2.Distance(manager.transform.position, targetPosition) < 0.1f;

        if (waitTimer <= 0f && arrivedAtStart) {
            isWaiting = false;
            parameter.anim.applyRootMotion = false;
            manager.TransitionState(nextState);
        }

        /*
        manager.transform.position = Vector2.MoveTowards(manager.transform.position, new Vector2(parameter.flyStartPoint.position.x,parameter.flyStartPoint.position.y),parameter.moveSpeed * Time.deltaTime);

        if (Vector2.Distance(manager.transform.position, targetPosition) < .1f) {
            Debug.Log("arrived");
        }*/


        //Debug.Log("Update");

        //manager.FlipTo(new Vector3(targetPosition.x, targetPosition.y));

        //waitTimer -= Time.deltaTime;
    }

    private StateType PickNext() {

        // add all the action weigh together exclude the last State
        float totalWeight = 0f;
        foreach (var entry in actionTable)
            if (entry.state != lastState)
                totalWeight += entry.weight;

        // random roll a number
        float roll = UnityEngine.Random.Range(0f, totalWeight);
        float cumulative = 0f;

        // add cumulative, if roll is 40
        // if first cumulative is 0, second will add first action weight c=25
        // second add is c=50, so 25 < 50  it mean the next action is second action
        foreach (var entry in actionTable) {
            if (entry.state == lastState) continue;
            cumulative += entry.weight;
            if (roll <= cumulative) {
                lastState = entry.state;

                // fly
                if (entry.state == StateType.Fly) {
                    parameter.flyStartLeft = Random.Range(0, 2) == 0; // true = 从A出发，false = 从B出发
                    Debug.Log($"Fly 方向：{(parameter.flyStartLeft ? "A→B" : "B→A")}");
                }

                Debug.Log($"Decision 抽到：{entry.state}"); // ← 加这一行
                return entry.state;
            }
        }

        return StateType.Fly;
    }

    private Vector3 GetStartPosition(StateType state) {
        //Debug.Log("GetstartPosition");
        return state switch {
            StateType.Fly => parameter.flyStartLeft    // 根据方向选起始点
                          ? parameter.flyStartPointLeft.position
                          : parameter.flyStartPointRight.position,
            StateType.Slam => parameter.slamStartPoint.position,
            StateType.Cast => parameter.castStartPoint.position,
            _ => manager.transform.position
        };
    }


    public void OnExit() {
        isWaiting = false;
        waitTimer = 0f;
    }
}
