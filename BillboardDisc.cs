using Godot;
using System;

public partial class BillboardDisc : Sprite3D
{
	private Vector3 originalScale;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		originalScale = Scale;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
			Transform3D cameraTransform = GetViewport().GetCamera3D().GlobalTransform;
			Transform3D spriteTransform = GlobalTransform;
			
			Vector3 basisX  = spriteTransform.Basis.X;
			Vector3 basisY = spriteTransform.Basis.Y;
			Vector3 basisZ = spriteTransform.Basis.Z;
			
			basisZ = (cameraTransform.Origin - spriteTransform.Origin).Normalized() * originalScale.Z;
			basisX = basisY.Cross(basisZ).Normalized() * originalScale.X;
			
			GlobalTransform = new Transform3D(basisX, basisY, basisZ, spriteTransform.Origin);
			

	}
}
