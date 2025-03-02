using Godot;
using System;

public partial class CharacterBody3d : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 7.5f;
	public const float FrisbeeForce = 1.0f;
	public const float FrisbeeSpin = 0.5f;
	public const float LiftCoefficient = 1.2f;
	public const float DragCoefficient = 0.3f;
	public const float Gravity = -9.8f;

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Apply gravity
		if (!IsOnFloor())
		{
			velocity.Y += Gravity * (float)delta;
		}

		// Handle Jump
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle movement
		Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();

		// Check for collisions and apply forces to RigidBody3D
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			KinematicCollision3D collision = GetSlideCollision(i);
			if (collision.GetCollider() is RigidBody3D rigidBody)
			{
				Vector3 throwDirection = new Vector3(1, 0.2f, 0); // Rightward with slight upward arc
				ApplyFrisbeeForces(rigidBody, throwDirection);
			}
			else if (collision.GetCollider() is MeshInstance3D meshInstance)
			{
				Vector3 throwDirection = new Vector3(1, 0.2f, 0);
				ApplyFrisbeeForces(meshInstance, throwDirection);
			}

		}
	}

	private void ApplyFrisbeeForces(Node3D frisbee, Vector3 throwDirection)
{
	throwDirection = throwDirection.Normalized(); // Ensure it's a unit vector
	float speed = FrisbeeForce; // Use a fixed speed

	// Calculate lift force
	float angleOfAttack = Mathf.Atan2(throwDirection.Y, speed);
	float liftMagnitude = LiftCoefficient * speed * speed * Mathf.Sin(angleOfAttack);
	Vector3 lift = new Vector3(0, liftMagnitude, 0);

	// Calculate drag force
	float dragMagnitude = DragCoefficient * speed * speed;
	Vector3 drag = -throwDirection * dragMagnitude;

	Vector3 finalVelocity = (throwDirection * speed) + lift + drag;

	if (frisbee is RigidBody3D rigidBody)
	{
		rigidBody.ApplyImpulse(finalVelocity);
		rigidBody.AngularVelocity = new Vector3(0, FrisbeeSpin, 0);
	}
	else if (frisbee is MeshInstance3D meshInstance)
	{
		// Simulate motion for MeshInstance3D by modifying its transform
		meshInstance.GlobalTransform = new Transform3D(
			meshInstance.GlobalTransform.Basis,
			meshInstance.GlobalTransform.Origin + finalVelocity * (float)GetProcessDeltaTime()
		);
	}
}


}
