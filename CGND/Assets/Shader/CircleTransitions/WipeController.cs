using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WipeController : MonoBehaviour
{
    public static WipeController instance;
    private Animator anim;
    private Image _image;
    private readonly int _circleSizeId = Shader.PropertyToID("_Circle_Size");
    private bool isIn = false;

    public float circleSize = 0;

    private InputAction testBlackFade;
    private void Awake() {
        instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        _image = GetComponent<Image>();
        testBlackFade = InputSystem.actions.FindAction("TriggerBlackFade");
        FadeIn();
    }

    // Update is called once per frame
    void Update()
    {
        _image.materialForRendering.SetFloat(_circleSizeId,circleSize);

        //TriggerBlackFade
        if (testBlackFade.WasPressedThisFrame()){
            if (isIn) {
                FadeOut();
            } else {
                FadeIn();
            }
        }
    }

    public void FadeIn() {
        anim.SetTrigger("In");
        isIn = true;
    }

    public void FadeOut() {
        anim.SetTrigger("Out");
        isIn = false;
    }
}
