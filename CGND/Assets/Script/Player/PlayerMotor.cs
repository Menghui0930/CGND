using UnityEngine;
using UnityEngine.Playables;

public class PlayerMotor : MonoBehaviour {
    private PlayerState[] _playerStates;
    private PlayerController _playerController;

    [SerializeField] private bool isControllable = false;

    private void Start() {
        _playerStates = GetComponents<PlayerState>();
        _playerController = GetComponentInParent<PlayerController>();
    }


    void Update() {
        if(!isControllable) return;

        if (_playerStates.Length > 0) {
            foreach (PlayerState state in _playerStates) {
                state.LocalInput();
                state.ExecuteState();
                state.SetAnimation();
            }
        }
    }

    public void SpawnPlayer(Vector3 newPosition) {
        transform.position = new Vector3(newPosition.x,newPosition.y,0);
    }

    public void EnableControl() {
        isControllable = true;
        if (_playerController != null)
            _playerController.isPaused = false;
    }

    public void DisableControl() {
        isControllable = false;
        if (_playerController != null) {
            _playerController.isPaused = true;
            _playerController.SetHorizontalForce(0f);  
        }
    }
}
