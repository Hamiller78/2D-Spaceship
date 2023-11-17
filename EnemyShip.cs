using Godot;
using System;

public partial class EnemyShip : Area2D
{
	[Export]
	public float Acceleration { get; set; } = 200f;
	
	[Export]
	public float RadsPerSecond { get; set; } = 2f * (float)Math.PI * 0.4f;
	
	[Export]
	public PackedScene LaserShotScene { get; set; }

	[Export]
	public float RechargeTime { get; set; } = 0.5f;
	private Vector2 _targetPosition;
	private float _targetRotation;
	private float _rechargeTimeRemaining = 0f;	

	[Signal]
	public delegate void EnemyShipDestroyedEventHandler(EnemyShip enemyShip);

	public Vector2 Velocity
	{
		get => _velocity;
	}

	private Vector2 _velocity = Vector2.Zero;

	private bool _isEngineRunning = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		TurnToTarget(delta);
	}

	public void OnAreaEntered(Area2D area)
	{
		if (area is LaserShot)
		{
			EmitSignal(SignalName.EnemyShipDestroyed, this);
			QueueFree();
		}
	}	

	public void OnTargetPositionUpdated(Vector2 position)
	{
		_targetPosition = position;
		_targetRotation = (float)(Math.Atan2(position.Y - Position.Y, position.X - Position.X) + Math.PI / 2d);
	}	

	private void TurnToTarget(double delta)
	{
		var rotationDelta = _targetRotation - Rotation;
		if (rotationDelta < -Math.PI)
		{
			rotationDelta += (float)(2d * Math.PI);
		}
		else if (rotationDelta > Math.PI)
		{
			rotationDelta -= (float)(2d * Math.PI);
		}
		var rotationStep = RadsPerSecond * (float)delta;

		if (Math.Abs(rotationDelta) <= rotationStep)
		{
			Rotation = _targetRotation;
		}
		else
		{
			Rotation += Math.Sign(rotationDelta) * rotationStep;
		}
	}
}
