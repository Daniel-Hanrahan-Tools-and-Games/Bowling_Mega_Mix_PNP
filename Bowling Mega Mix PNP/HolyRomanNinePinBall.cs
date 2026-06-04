using Godot;
using System;
using System.Threading;

public partial class HolyRomanNinePinBall : RigidBody3D
{
	
	// variables for ball
	[Export] public float Speed = 10.0f;
	
	private Camera3D _camera;
	private Vector3 _targetPosition;
	private bool _hasTarget = false;
	
	// The amount of frames and frame turns there are and the players placemet on those turns and frames
	// Amount of frame turns there are
	int intFrameTurns = 1;
	// The frame turn the player is on
	int intFrameTurnsOn = 0;
	// Amount of frames there are
	int intFrames = 24;
	// The frame the player is on
	int intFrameOn = 1;
	
	// Lock to frevent player moving ball left and right after ball is launched
	float dblMoveLock = 0;
	
	private Vector3 originalPosition;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_camera = GetViewport().GetCamera3D();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Screen-to-World projection is best handled in _Process
		UpdateTargetPosition();
		
		// pin respawn code and only does it when ball has hit a specific point
		if (GlobalPosition.Z < -9){
			intFrameTurnsOn += 1;
			if (intFrameTurns == intFrameTurnsOn){
				// respawns pins and ball
				GetTree().ReloadCurrentScene();
				// resets frame turn for next frame
				intFrameTurnsOn = 0;
				
				intFrameOn = intFrameOn + 1;
				
				if (intFrameOn > intFrames) {
					GetTree().ChangeSceneToFile("res://MainMenu.tscn");
				}
			}
		}
	}
	// ball tracking code for next 2 overrides
	public override void _IntegrateForces(PhysicsDirectBodyState3D state)
	{
		if (!_hasTarget) return;

		// Calculate velocity needed to reach the target
		Vector3 currentPosition = state.Transform.Origin;
		Vector3 direction = _targetPosition - currentPosition;
		
		// Lock the Y axis if you only want flat 2D movement
		direction.Y = 0; 

		// Apply smooth velocity through the physics state
		state.LinearVelocity = direction * Speed;
	}

	private void UpdateTargetPosition()
	{
		if (_camera == null) return;

		Vector2 mousePos = GetViewport().GetMousePosition();
		Vector3 rayOrigin = _camera.ProjectRayOrigin(mousePos);
		Vector3 rayNormal = _camera.ProjectRayNormal(mousePos);

		// Plane sitting at the object's current height
		Plane movementPlane = new Plane(Vector3.Up, GlobalPosition.Y);

		Vector3? intersection = movementPlane.IntersectsRay(rayOrigin, rayNormal);

		if (intersection.HasValue)
		{
			_targetPosition = intersection.Value;
			_hasTarget = true;
		}
}
}
