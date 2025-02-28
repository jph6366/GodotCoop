namespace GodotCoop;

using Chickensoft.Introspection;
using Chickensoft.Serialization;
using Godot;

public partial class PlayerCameraLogic {
    /// <summary>Player camera data.</summary>
    [Meta, Id("player_camera_logic_data")]
    public partial record Data {
        /// <summary>
        /// The camera's target position. The camera will move towards this
        /// position each tick, allowing for smooth camera follow motion.
        /// </summary>
        [Save("target_position")]
        public required Vector3 TargetPosition { get; set; }

        /// <summary>
        /// Target offset (used to slide the camera when strafing.
        /// </summary>
        [Save("target_offset")]
        public required Vector3 TargetOffset { get; set; }

        /// <summary>
        /// The current target angle of the camera (used for rotation adjustments).
        /// </summary>
        [Save("target_angle")]
        public float TargetAngle { get; set; } = 0f;
    }
}