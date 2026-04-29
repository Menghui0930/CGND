using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class SpikePlatform1 : MonoBehaviour
{
    [Header("Spike")]
    public GameObject spike1;
    public GameObject spike2;

    [Header("Flash")]
    [Tooltip("After player step platform, wait flshing time ")]
    public float waitTime = 3f;
    [Tooltip("flsh time")]
    public float flashDuration = 1.5f;
    [Tooltip("flash Speed）")]
    public float flashSpeed = 8f;

    [Header("Exit Setting")]
    [Tooltip("Time after player exit platform")]
    public float hideDelay = 2f;

    // ── Spike Status ──────────────────────────────────────────────────
    private enum SpikeState { Idle, Waiting, Flashing, Active }
    private SpikeState _state = SpikeState.Idle;

    private bool _playerOn = false;
    private Coroutine _sequenceCoroutine = null;
    private Coroutine _hideCoroutine = null;


    private void OnCollisionEnter2D(Collision2D collision) {
        if (!collision.gameObject.CompareTag("Player")) return;

        _playerOn = true;

        // stop (hideCoroutine) because player come back
        if (_hideCoroutine != null) {
            StopCoroutine(_hideCoroutine);
            _hideCoroutine = null;
        }

        // only spike status is Idle can start (avoid repeat trigger)
        if (_state == SpikeState.Idle) {
            _sequenceCoroutine = StartCoroutine(SpikeSequence());
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if (!collision.gameObject.CompareTag("Player")) return;

        _playerOn = false;

        switch (_state) {
            case SpikeState.Waiting:
            case SpikeState.Flashing:
                // Spike not fully appeared yet: After SpikeSequence finishes running and enters Active state,
                //it will automatically check _playerOn and then trigger a delay before disappearing.
                break;

            case SpikeState.Active:
                // Spike has appeared, player leaves → disappears after a hideDelay second delay.
                _hideCoroutine = StartCoroutine(HideAfterDelay());
                break;
        }
    }


    private IEnumerator SpikeSequence() {
        // 1. wait
        _state = SpikeState.Waiting;
        yield return new WaitForSeconds(waitTime);

        //2. Open Spike and Start flash
        _state = SpikeState.Flashing;
        EnableSpikeVisuals(true);
        yield return StartCoroutine(FlashSpikes(flashDuration));

        // 3. Spike show and enable BoxCollider
        SetSpikeColor(spike1, Color.white);
        SetSpikeColor(spike2, Color.white);
        EnableSpikeColliders(true);
        _state = SpikeState.Active;

        // Check player still stay platform or not
        if (!_playerOn) {
            _hideCoroutine = StartCoroutine(HideAfterDelay());
        }
    }

    private IEnumerator HideAfterDelay() {
        yield return new WaitForSeconds(hideDelay);
        ResetSpikes();
    }

    private void ResetSpikes() {
        StopAllCoroutines();
        _sequenceCoroutine = null;
        _hideCoroutine = null;
        _state = SpikeState.Idle;

        EnableSpikeColliders(false);
        EnableSpikeVisuals(false);
    }

    private IEnumerator FlashSpikes(float duration) {
        float elapsed = 0f;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float t = Mathf.PingPong(elapsed * flashSpeed, 1f);
            Color flash = Color.Lerp(Color.white, Color.red, t);
            SetSpikeColor(spike1, flash);
            SetSpikeColor(spike2, flash);
            yield return null;
        }
    }

    private void EnableSpikeVisuals(bool enable) {
        spike1.SetActive(enable);
        spike2.SetActive(enable);
    }

    private void EnableSpikeColliders(bool enable) {
        spike1.GetComponent<BoxCollider2D>().enabled = enable;
        spike2.GetComponent<BoxCollider2D>().enabled = enable;
    }

    private void SetSpikeColor(GameObject obj, Color color) {
        if (obj == null) return;
        var sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = color;
    }
}

