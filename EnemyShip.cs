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

	public float FireRange { get; set; } = 600f;

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
		if (_rechargeTimeRemaining > 0f)
		{
			_rechargeTimeRemaining -= (float)delta;
		}

		TurnToTarget(delta);

		var targetDistance = _targetPosition.DistanceTo(Position);
		if (targetDistance <= FireRange && Math.Abs(_targetRotation - Rotation + (float)Math.PI / 2f) < Math.PI / 4d)
		{
			FirePrimary();
			StopEngine();
		}
		else if (Math.Abs(_targetRotation - Rotation + (float)Math.PI / 2f) < Math.PI / 4d)
		{
			RunEngine(delta);
		}
		else
		{
			StopEngine();
		}

		var newX = (float)(Position.X + _velocity.X * delta);
		var newY = (float)(Position.Y + _velocity.Y * delta);

		var newPosition = new Vector2(newX, newY);
		Position = newPosition;		
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
		_targetRotation = Position.AngleToPoint(_targetPosition);
	}	

	private void TurnToTarget(double delta)
	{
		var rotationDelta = _targetRotation - (Rotation - (float)Math.PI / 2f);
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
			Rotation = _targetRotation + (float)Math.PI / 2f;
		}
		else
		{
			Rotation += Math.Sign(rotationDelta) * rotationStep;
		}
	}

	private void FirePrimary()
	{
		if (_rechargeTimeRemaining > 0f)
		{
			return;
		}

		var newShot = LaserShotScene.Instantiate<LaserShot>();
		newShot.Position
			= Position
				+ new Vector2(
					150f * (float)Math.Cos(Rotation - Math.PI / 2d),
					150f * (float)Math.Sin(Rotation - Math.PI / 2d));
		newShot.Rotation = Rotation;
		newShot.Velocity =
			new Vector2(
			  	(float)Math.Cos(newShot.Rotation - Math.PI / 2d) * newShot.Speed,
				(float)Math.Sin(newShot.Rotation - Math.PI / 2d) * newShot.Speed
			);
		GetParent().AddChild(newShot);

		_rechargeTimeRemaining = RechargeTime;
	}

	private void RunEngine(double delta)
	{
		if (!_isEngineRunning)
		{
			GD.Print("Engine running");
			_isEngineRunning = true;
			GetNode<AnimatedSprite2D>("ShipSprite/EngineFlame").Visible = true;
		}

			var vx = (float)(_velocity.X + Math.Cos(Rotation - Math.PI / 2d) * Acceleration * delta);
			var vy = (float)(_velocity.Y + Math.Sin(Rotation - Math.PI / 2d) * Acceleration * delta);
			_velocity = new Vector2(vx, vy);
	}

	private void StopEngine()
	{
		if (_isEngineRunning)
		{
			GD.Print("Engine stopped");
			_isEngineRunning = false;
			GetNode<AnimatedSprite2D>("ShipSprite/EngineFlame").Visible = false;
		}
	}
}
