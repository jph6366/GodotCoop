namespace GodotCoop;

using System;
using Chickensoft.Collections;
using Godot;

/// <summary>
/// Interface for the game repository, storing pure game logic not directly related to the game's view.
/// </summary>
public interface IGameRepo : IDisposable {
    /// <summary>Event invoked when the game ends.</summary>
    event Action<GameOverReason>? Ended;

    /// <summary>Event invoked whenever the player jumps.</summary>
    event Action? Jumped;

    /// <summary>Event invoked when the camera rotation changes.</summary>
    event Action<Quaternion>? CameraChanged;

    /// <summary>Pause status.</summary>
    IAutoProp<bool> IsPaused { get; }

    /// <summary>Player's position in global coordinates.</summary>
    IAutoProp<Vector3> PlayerGlobalPosition { get; }

    /// <summary>Camera's rotation using quaternion for 3D transformations.</summary>
    IAutoProp<Quaternion> CameraRotation { get; }

    /// <summary>Camera's global forward direction vector.</summary>
    Vector2 GlobalCameraDirection { get; }

    /// <summary>Handles the game over scenario.</summary>
    /// <param name="reason">The reason for game over.</param>
    void OnGameEnded(GameOverReason reason);

    /// <summary>Pauses the game.</summary>
    void Pause();

    /// <summary>Resumes the game.</summary>
    void Resume();

    /// <summary>Triggers the jump event.</summary>
    void OnJump();

    /// <summary>Updates the camera rotation.</summary>
    /// <param name="rotation">New camera rotation.</param>
    void SetCameraRotation(Quaternion rotation);

    /// <summary>Updates the player's global position.</summary>
    /// <param name="playerGlobalPosition">New player position in world coordinates.</param>
    void SetPlayerGlobalPosition(Vector3 playerGlobalPosition);
}

/// <summary>
/// Implementation of IGameRepo, managing game state and events.
/// </summary>
public class GameRepo : IGameRepo {
    public IAutoProp<bool> IsPaused => _isPaused;
    private readonly AutoProp<bool> _isPaused;

    public IAutoProp<Vector3> PlayerGlobalPosition => _playerGlobalPosition;
    private readonly AutoProp<Vector3> _playerGlobalPosition;

    public IAutoProp<Quaternion> CameraRotation => _cameraRotation;
    private readonly AutoProp<Quaternion> _cameraRotation;

    /// <summary>Calculates the global camera direction from rotation.</summary>
    public Vector2 GlobalCameraDirection => new(-_cameraRotation.Value.X, -_cameraRotation.Value.Z);

    public event Action<GameOverReason>? Ended;
    public event Action? Jumped;
    public event Action<Quaternion>? CameraChanged;

    private bool _disposedValue;

    /// <summary>Initializes a new instance of the GameRepo class.</summary>
    public GameRepo() {
        _isPaused = new AutoProp<bool>(false);
        _playerGlobalPosition = new AutoProp<Vector3>(Vector3.Zero);
        _cameraRotation = new AutoProp<Quaternion>(Quaternion.Identity);
    }

    /// <summary>Updates the player's position.</summary>
    /// <param name="playerGlobalPosition">New global position.</param>
    public void SetPlayerGlobalPosition(Vector3 playerGlobalPosition) =>
        _playerGlobalPosition.OnNext(playerGlobalPosition);

    /// <summary>Updates the camera rotation and triggers the CameraChanged event.</summary>
    /// <param name="rotation">New camera rotation.</param>
    public void SetCameraRotation(Quaternion rotation) {
        _cameraRotation.OnNext(rotation);
        CameraChanged?.Invoke(rotation);
    }

    /// <summary>Triggers the jump event.</summary>
    public void OnJump() => Jumped?.Invoke();

    /// <summary>Handles the game over scenario and pauses the game.</summary>
    /// <param name="reason">Reason for game over.</param>
    public void OnGameEnded(GameOverReason reason) {
        Pause();
        Ended?.Invoke(reason);
    }

    /// <summary>Pauses the game.</summary>
    public void Pause() => _isPaused.OnNext(true);

    /// <summary>Resumes the game and re-engages input.</summary>
    public void Resume() {
        _isPaused.OnNext(false);
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    /// <summary>Handles resource cleanup and event unsubscription.</summary>
    /// <param name="disposing">Indicates whether managed resources should be disposed.</param>
    protected void Dispose(bool disposing) {
        if (!_disposedValue) {
            if (disposing) {
                _playerGlobalPosition.OnCompleted();
                _playerGlobalPosition.Dispose();

                _cameraRotation.OnCompleted();
                _cameraRotation.Dispose();
            }

            Jumped = null;
            Ended = null;
            CameraChanged = null;

            _disposedValue = true;
        }
    }

    /// <summary>Releases resources and suppresses finalization.</summary>
    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
