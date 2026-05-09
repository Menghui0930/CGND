using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Multiplayer.PlayMode;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;

public enum StateType {
    Decision,   // 决策者
    Fly,        // A飞到B 或 B飞到A
    Slam,       // 从中间往下砸
    Cast,       // 召唤物品 左/右
    Shield      // 防御盾
}

[Serializable]
public class Parameter {
    public int maxHealth = 500;
    public int currentHealth;
    public float moveSpeed = 2f;
    public float waitDuration = 1f;

    public Animator anim;

    [Header("Fly")]
    public Transform flyStartPointLeft;
    public Transform flyStartPointRight;
    public bool flyStartLeft;
    public GameObject firePrefabs;
    public float fireInterval;
    public bool flyStart = false;
    //public bool flystart;

    [Header("Slam")]
    public Transform slamStartPoint;    // 中间起始点
    public Transform slamSpikeCenter;    // 中间起始点
    public float spikeSpeed;
    public GameObject spikePrefabs;

    [Header("Cast")]
    public Transform castStartPoint;
    public GameObject rockPrefabs;
    public Transform castPointLeft;
    public Transform castPointRight;

    [Header("Shield")]
    public int shieldHealth = 300;
    public float shieldDuration = 5f;
    public ShieldController shieldObject;
    public bool shield1Used = false;
    public bool shield2Used = false;

    public PlayerMotor currentPlayer;
}

public class FSM : MonoBehaviour
{
    public Parameter parameter;
    private IState currentState;



    private Dictionary<StateType, IState> states = new Dictionary<StateType, IState>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        parameter.anim = GetComponent<Animator>();
        
        states.Add(StateType.Decision, new DecisionState(this));
        states.Add(StateType.Fly, new FlyState(this));
        states.Add(StateType.Slam, new SlamState(this));
        states.Add(StateType.Cast, new CastState(this));        

        //TransitionState(StateType.Decision);

        //TransitionState(StateType.Idle);
        parameter.currentHealth = parameter.maxHealth;
    }

    public void BossActivate() {
        StartCoroutine(OpeningSequence());
    }

    private IEnumerator OpeningSequence() {
        // 播放开场动画
        LevelManager.Instance.HealthUIFadeOut();
        parameter.anim.Play("Boss_Openning");

        // 等动画播完
        yield return null;
        float clipLength = parameter.anim.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(clipLength);

        parameter.currentPlayer.EnableControl();
        
        
        LevelManager.Instance.HealthUIFadeIn();
        // 正式开始 FSM
        TransitionState(StateType.Decision);
    }











    // Update is called once per frame
    void Update()
    {
        if (currentState == null) return;
        currentState.OnUpdate();
        CheckShieldThreshold();
    }

    public void TransitionState(StateType type) {
        if (currentState!= null) {
            currentState.OnExit();
        }
        currentState = states[type];
        currentState.OnEnter();
    }

    public void FlipTo(Vector2 target) {
        if (target != null) {
            if (transform.position.x > target.x) {
                transform.localScale = new Vector3(-1, 1, 1);
            } else if (transform.position.x < target.x) {
                transform.localScale = new Vector3(1,1,1);
            }
        } else {
            Debug.Log("no target");
        }
    }

    public void TransitionTo(Vector3 targetPosition) {
        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
            parameter.moveSpeed * Time.deltaTime
        );
    }

    private void CheckShieldThreshold() {
        float hpRatio = (float)parameter.currentHealth / parameter.maxHealth;

        if (!parameter.shield1Used && hpRatio <= 0.667f) {
            parameter.shield1Used = true;
            parameter.shieldObject.gameObject.SetActive(true);
            parameter.shieldObject.shieldActivate();   // 不切换状态，直接激活盾
        } else if (! parameter.shield2Used && hpRatio <= 0.333f) {
            parameter.shield2Used = true;
            parameter.shieldObject.shieldActivate();
        }
    }

    // fly
    public void FlyStartFireBall() {
        parameter.flyStart= true;
    }

    public void FlyEnd() {
        //parameter.flystart = false;
        if (currentState is FlyState) {
            parameter.anim.SetBool("Fly", false);
            TransitionState(StateType.Decision);
        }
    }

    //slam
    public void SpawnSpike() {
        if (currentState is SlamState slamState)
            slamState.OnSpawnSpike();   // 转发给 SlamState 处理
    }

    public void SlamEnd() {
        //parameter.flystart = false;
        if (currentState is SlamState) {
            parameter.anim.SetBool("Slam", false);
            TransitionState(StateType.Decision);
        }
    }

    // cast
    public void PushRock() {
        if (currentState is CastState castState)
            castState.OnPushRock();
    }

    public void CastEnd() {
        if (currentState is CastState) {
            parameter.anim.SetBool("Cast", false);
            TransitionState(StateType.Decision);
        }
    }

    private void SetCurrentPlayer(PlayerMotor new_currentPlayer) {
        parameter.currentPlayer = new_currentPlayer;
    }

    private void OnEnable() {
        LevelManager.OnPlayerSpawn += SetCurrentPlayer;
    }

    private void OnDisable() {
        LevelManager.OnPlayerSpawn -= SetCurrentPlayer;
    }

    // Shake Camera
    private void StartShakeCamera() {
        Camera2D.instance.StartShake(0.15f, 8f);  // intensity / speed 自己调
    }

    private void StopShakeCamera() {
        // 停止震动
        Camera2D.instance.StopShake();
        parameter.anim.applyRootMotion = true;
    }

    // Damage
    public void TakeDamage(int damage) {
        if (parameter.shieldObject != null && parameter.shieldObject.isActiveAndEnabled) return; // 有盾就无敌

        parameter.currentHealth -= damage;
        parameter.currentHealth = Mathf.Max(parameter.currentHealth, 0);

        // 相机震动反馈
        Camera2D.instance.StartShake(0.1f, 10f);
        StartCoroutine(StopShakeAfter(0.15f));

        if (parameter.currentHealth <= 0)
            BossDeath();
    }

    private IEnumerator StopShakeAfter(float delay) {
        yield return new WaitForSeconds(delay);
        Camera2D.instance.StopShake();
    }

    private void BossDeath() {
        // 之后你决定死亡动画、掉落、过关等
        Debug.Log("Boss Dead");
    }
}
