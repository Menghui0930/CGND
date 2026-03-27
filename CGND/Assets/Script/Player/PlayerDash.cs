using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class PlayerDash : PlayerState
{
    [Header("Settings")]
    private bool canDash = true;
    //private bool isDashing;
    [SerializeField] private float dashingDis = 3f;
    [SerializeField] private float dashingTime = 0.2f;
    [SerializeField] private float dashingCoolDown = 1f;

    [SerializeField] private LayerMask wallLayer;


    protected override void Awake() {
        base.Awake();
        dashing = InputSystem.actions.FindAction("Dash");
    }
    protected override void InitState() {
        base.InitState();
    }

    protected override void GetInput() {
        if (dashing.WasPressedThisFrame() && canDash) {
            //Debug.Log("Start Dash");
            StartCoroutine(Dash());
            
        }
    }

    public override void ExecuteState() {
        
    }

    private IEnumerator Dash() {
        canDash = false;
        //isDashing = true;

        float direction = _playerController.facingRight ? 1f : -1f;

        Vector2 startPosition = _playerController.transform.position;
        float distance = GetDashDistance(startPosition, direction, dashingDis);
        Vector2 targetPosition = startPosition + Vector2.right * direction * distance;

        float elapsedTime = 0f;
        while (elapsedTime < dashingTime) {
            float t = elapsedTime / dashingTime;
            float smoothT = 1f - Mathf.Pow(1f - t, 2f);
            _playerController.transform.position = Vector2.Lerp(startPosition, targetPosition, smoothT);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _playerController.transform.position = targetPosition; // Forced alignment at the last point
        //isDashing = false;

        yield return new WaitForSeconds(dashingCoolDown);
        canDash = true;
    }

    private float GetDashDistance(Vector2 startPosition, float direction, float maxDistance) {
        RaycastHit2D hit = Physics2D.Raycast(startPosition, Vector2.right * direction, maxDistance, wallLayer);
        Debug.DrawRay(startPosition, Vector2.right * direction * maxDistance, Color.red, 0.2f);

        if (hit.collider != null) {
            //Debug.Log("Obstacle detected:" + hit.collider.gameObject.name);
            return hit.distance - 0.1f;  // leave some space to avoid too close wall
        }
        return maxDistance;
    }

    public override void SetAnimation() {
        base.SetAnimation();
    }


    /*
    private IEnumerator Dash() {
        Debug.Log("Dash enter");
        canDash = false;
        isDashing = true;

        float direction = _playerController.facingRight ? 1f : -1f;
        if (direction == 0) direction = 1f;

        Vector2 startPosition = _playerController.transform.position;
        float adjustedDistance = GetDashDistance(startPosition, direction, 3f);

        float targetX = startPosition.x + adjustedDistance * direction;

        float dashSpeed = adjustedDistance / dashingTime;
        float elapsedTime = 0f;

        while (elapsedTime < dashingTime) {
            float newX = _playerController.transform.position.x + dashSpeed * direction * Time.deltaTime;

            if (direction > 0) newX = Mathf.Min(newX, targetX);
            else newX = Mathf.Max(newX, targetX);

            //_playerController.transform.position += new Vector3(dashSpeed * direction * Time.deltaTime, 0, 0);
            _playerController.transform.position = new Vector3(newX, _playerController.transform.position.y, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);
        isDashing = false;

        yield return new WaitForSeconds(dashingCoolDown);
        canDash = true; 


    }
    */

}
