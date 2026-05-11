using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Level2Tutorial : MonoBehaviour
{
    public InputAction _OpenSkillTree;
    private InputAction _attack;

    [SerializeField] private GameObject _TutorialSkillTree;

    private bool DoOnce = true;

    private void Awake() {
        _OpenSkillTree = InputSystem.actions.FindAction("Interact");
        _attack = InputSystem.actions.FindAction("Attack");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        _TutorialSkillTree.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        if (_OpenSkillTree.WasPressedThisFrame() && DoOnce) {
            DoOnce = false;
            _TutorialSkillTree.SetActive(false);
            return;
        }
    }


    public void ShowTutorial() {
        _TutorialSkillTree.SetActive(true);
        Animator tutorialAnimator = _TutorialSkillTree.GetComponent<Animator>();

        StartCoroutine(WaitSomeTime());
        tutorialAnimator.Play("SkillTree_Tutorial_IN");
    }

    public IEnumerator WaitSomeTime() {
        yield return new WaitForSeconds(1f);
    }

    private void OnEnable() {
        LevelManager.OnGameStart += ShowTutorial;
    }

    private void OnDisable() {
        LevelManager.OnGameStart -= ShowTutorial;
    }
}
