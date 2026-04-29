using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

//public enum Element_public { Grass, Water, Wind }

public class PlayerElementSwitch : PlayerState
{
    public static PlayerElementSwitch Instance_playerElementSwitch;
    public enum Element { Grass, Water, Wind }

    [Header("Element Sprites")]
    [SerializeField] private SpriteRenderer ball_SR;
    [SerializeField] private Sprite grassSprites;
    [SerializeField] private Sprite waterSprites;
    [SerializeField] private Sprite windSprites;
    [SerializeField] private float switchDuration = 0.3f;

    public Element current_element = Element.Grass;

    protected override void Awake() {
        base.Awake();
        Instance_playerElementSwitch = this;
        switching = InputSystem.actions.FindAction("Switch_element");
    }

    protected override void InitState() {
        base.InitState();
        ApplyElement(current_element);
    }

    public override void ExecuteState() {
        if (switching.WasPressedThisFrame()) {
            current_element = current_element switch {
                Element.Grass => Element.Water,
                Element.Water => Element.Wind,
                Element.Wind => Element.Grass,
                _ => Element.Grass
            };

            ApplyElement(current_element);
        }
    }

    private void ApplyElement(Element element) {
        Sprite targetSprite = element switch {
            Element.Grass => grassSprites,
            Element.Water => waterSprites,
            Element.Wind => windSprites,
            _ => grassSprites
        };

        StartCoroutine(FadeSprite(targetSprite));
    }

    private IEnumerator FadeSprite(Sprite targetSprite) {
        float elapsed = 0f;

        while (elapsed < switchDuration / 2f) {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / (switchDuration / 2f));
            ball_SR.color = new Color(1, 1, 1, alpha);
            yield return null;
        }

        ball_SR.sprite = targetSprite;

        elapsed = 0f;
        while (elapsed < switchDuration / 2f) {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / (switchDuration / 2f));
            ball_SR.color = new Color(1, 1, 1, alpha);
            yield return null;
        }

        ball_SR.color = new Color(1, 1, 1, 1);
    }

    
    public string GetCurrentElement() {
        string currentELementString = current_element switch {
            Element.Grass => "Grass",
            Element.Water => "Water",
            Element.Wind => "Wind",
            _ => "Grass"
        };
        return currentELementString;
    }
    

}
