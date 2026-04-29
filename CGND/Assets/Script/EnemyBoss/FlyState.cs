using UnityEngine;

public class FlyState : IState {
    private FSM manager;
    private Parameter parameter;
    //private float timer = 2f; // 模拟动作持续时间
    private float fireTimer = 1f;

    private bool flyStartPoint;
    public FlyState(FSM manager) { 
        this.manager = manager; 
        parameter = manager.parameter;
    
    }

    public void OnEnter() {
        Debug.Log(">>> 进入 FlyState");
        Debug.Log($"SetBool Fly = true, 当前Animator状态: {parameter.anim.GetCurrentAnimatorStateInfo(0).IsName("Fly")}");
        if(parameter.flyStartLeft) {
            parameter.anim.SetBool("FlyLeft", true);
        } else {
            parameter.anim.SetBool("FlyRight", true);
        }
 
        fireTimer = parameter.fireInterval;

    }

    public void OnUpdate() {

        if (parameter.flyStart) {
            fireTimer += Time.deltaTime;
            if (fireTimer >= parameter.fireInterval) {
                fireTimer = 0f;
                SpawnFire();
            }
        }

    }

    public void OnExit() {
        //timer = 1.5f; // 重置，不然第二次进来 timer 已经是 0
        fireTimer = 0;
        parameter.flyStart = false;
        
    }

    private void SpawnFire() {
        GameObject fire = UnityEngine.Object.Instantiate(
            parameter.firePrefabs,
            manager.transform.position,
            Quaternion.identity
        );
    }


}
