using Godot;
using System;

public partial class Turret : Area2D
{
	private Vector2 _targetPosition;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnTargetPositionUpdated(Vector2 position)
	{
		_targetPosition = position;
		var targetRotation = (float)Math.Atan2(position.Y - Position.Y, position.X - Position.X) + (float)(Math.PI / 2d);
		Rotation = targetRotation;
	}
}
