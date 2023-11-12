using Godot;
using System;

public partial class Turret : Area2D
{
	[Export]
	public PackedScene LaserShotScene { get; set; }

	[Signal]
	public delegate void TurretDestroyedEventHandler(Turret turret);

	public float TurnRate { get; set; } = 2f * (float)Math.PI * 0.2f;
	public float FireRange { get; set; } = 600f;
	public float ViewRange { get; set; } = 800f;
	public float RechargeTime { get; set; } = 0.5f;
	private Vector2 _targetPosition;
	private float _targetRotation;
	private float _rechargeTimeRemaining = 0f;

	public override void _Ready()
	{
		_rechargeTimeRemaining = RechargeTime;
	}

	public override void _Process(double delta)
	{
		if (_rechargeTimeRemaining > 0f)
		{
			_rechargeTimeRemaining -= (float)delta;
		}

		var targetDistance = _targetPosition.DistanceTo(Position);
		if (targetDistance > ViewRange)
		{
			return;
		}

		TurnTurret(delta);

		if (targetDistance > FireRange)
		{
			return;
		}

		FirePrimary();
	}

	public void OnTargetPositionUpdated(Vector2 position)
	{
		_targetPosition = position;
		_targetRotation = (float)(Math.Atan2(position.Y - Position.Y, position.X - Position.X) + Math.PI / 2d);
	}

	public void OnAreaEntered(Area2D area)
	{
		if (area is LaserShot)
		{
			EmitSignal(SignalName.TurretDestroyed, this);
			QueueFree();
		}
	}

	private void TurnTurret(double delta)
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
		var rotationStep = TurnRate * (float)delta;

		if (Math.Abs(rotationDelta) <= rotationStep)
		{
			Rotation = _targetRotation;
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
					90f * (float)Math.Cos(Rotation - Math.PI / 2d),
					90f * (float)Math.Sin(Rotation - Math.PI / 2d));
		newShot.Rotation = Rotation;
		newShot.Velocity =
			new Vector2(
			  	(float)Math.Cos(newShot.Rotation - Math.PI / 2d) * newShot.Speed,
				(float)Math.Sin(newShot.Rotation - Math.PI / 2d) * newShot.Speed
			);
		GetParent().AddChild(newShot);

		_rechargeTimeRemaining = RechargeTime;
	}
}
