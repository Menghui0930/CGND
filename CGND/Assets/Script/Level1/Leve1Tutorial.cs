using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Leve1Tutorial : MonoBehaviour
{
    public InputAction _OpenSkillTree;

    [SerializeField] private GameObject _TutorialSkillTree;
    [SerializeField] private float WaitSomeTimePopUP = 1.5f;

    private bool DoOnce = true;

    private void Awake() {
        _OpenSkillTree = InputSystem.actions.FindAction("Interact");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _TutorialSkillTree.SetActive(false);   
    }

    // Update is called once per frame
    void Update()
    {
        if (_OpenSkillTree.WasPressedThisFrame() && DoOnce) {
            DoOnce = false;
            _TutorialSkillTree.SetActive(false);
        }
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
    }


}
