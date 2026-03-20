using UnityEngine;
using UnityEngine.Playables;

public class PlayerMotor : MonoBehaviour {
    private PlayerState[] _playerStates;

    private void Start() {
        _playerStates = GetComponents<PlayerState>();
    }


    void Update() {
        if (_playerStates.Length > 0) {
            foreach (PlayerState state in _playerStates) {
                state.LocalInput();
                state.ExecuteState();
                state.SetAnimation();
            }
        }
    }
}
