using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Leve1Tutorial : MonoBehaviour
{
    public InputAction _OpenSkillTree;
    private InputAction _attack;

    [SerializeField] private GameObject _TutorialSkillTree;
    [SerializeField] private float WaitSomeTimePopUP = 1.5f;

    [SerializeField] private GameObject _skillTreeTutorialCon;
    // 四个 Animator 的 clip 名字，按顺序填
    [SerializeField]
    private string[] tutorialAnimations = new string[]
    {
        "Tutorial_Step1",
        "Tutorial_Step2",
        "Tutorial_Step3",
        "Tutorial_Step4"
    };

    private Animator _animator;
    //private bool _doOnce = true;
    private bool _isInTutorial = false;
    private bool _waitingForClick = false;
    private int _currentStep = 0;

    private bool _firstButtonClicked = false;
    [SerializeField] private FirstSkillButton _firstSkillButton;   // 拖入第一个 SkillButton


    private bool DoOnce = true;

    private void Awake() {
        _OpenSkillTree = InputSystem.actions.FindAction("Interact");
        _attack = InputSystem.actions.FindAction("Attack");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _TutorialSkillTree.SetActive(false);   
        _animator = _skillTreeTutorialCon.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_OpenSkillTree.WasPressedThisFrame() && DoOnce) {
            DoOnce = false;
            _TutorialSkillTree.SetActive(false);
            StartCoroutine(RunTutorial());
        }

        // 教学进行中，玩家点左键 → 下一步
        if (_isInTutorial && _waitingForClick && _attack.WasPressedThisFrame()) {
            _waitingForClick = false;
        }
    }

    private IEnumerator RunTutorial() {
        _isInTutorial = true;
        _skillTreeTutorialCon.SetActive(true);
        _currentStep = 0;

        while (_currentStep < tutorialAnimations.Length) {
            _animator.Play(tutorialAnimations[_currentStep]);

            yield return null;
            float clipLength = _animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(clipLength);

            if (_currentStep == 0) {
                // 第一步：等玩家点击第一个 SkillButton
                _firstButtonClicked = false;
                yield return new WaitUntil(() => _firstButtonClicked);
            } else {
                // 其他步：等玩家点左键
                _waitingForClick = true;
                yield return new WaitUntil(() => !_waitingForClick);
            }

            _currentStep++;
        }

        _isInTutorial = false;
        _skillTreeTutorialCon.SetActive(false);
    }


    public void ShowTutorial() {
        _TutorialSkillTree.SetActive(true);
        Animator tutorialAnimator = _TutorialSkillTree.GetComponent<Animator>();

        StartCoroutine(WaitSomeTime());
        tutorialAnimator.Play("SkillTree_Tutorial_IN");
    }

    public IEnumerator WaitSomeTime() {
        yield return new WaitForSeconds(WaitSomeTimePopUP);
    }

    private void OnEnable() {
        LevelManager.OnGameStart += ShowTutorial;
        FirstSkillButton.OnSkillButtonClicked += OnSkillButtonClicked;
    }

    private void OnDisable() {
        LevelManager.OnGameStart -= ShowTutorial;
        FirstSkillButton.OnSkillButtonClicked -= OnSkillButtonClicked;
    }

    private void OnSkillButtonClicked(FirstSkillButton btn) {
        if (btn == _firstSkillButton)
            _firstButtonClicked = true;
    }

}
