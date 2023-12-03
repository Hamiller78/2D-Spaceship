using Godot;
using System;

using SpaceGame.Sprites;

public partial class EnemyShip2 : ShipBase
{
	[Export]
	public float FireRange { get; set; } = 700f;

	private Vector2 _targetPosition;
	private Vector2 _targetVelocity;
	private float _targetRotation;

	private enum ManeuverMode
	{
		None,
		Approach,
		Attack
	}

	private ManeuverMode _maneuverMode = ManeuverMode.None;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_maneuverMode = ManeuverMode.Approach;
		base._Ready();		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var targetDistance = _targetPosition.DistanceTo(Position);
		if (_maneuverMode == ManeuverMode.Approach && targetDistance <= FireRange)
		{
			_maneuverMode = ManeuverMode.Attack;
		}
		else if (_maneuverMode == ManeuverMode.Attack && targetDistance > FireRange + 200d)
		{
			_maneuverMode = ManeuverMode.Approach;
		}

		switch (_maneuverMode)
		{
			case ManeuverMode.Approach:
				PerformApproach(delta);
				break;
			case ManeuverMode.Attack:
				PerformAttack(delta);
				break;
			default:
				break;
		}

		base._Process(delta);
	}

	private void PerformAttack(double delta)
	{
		TurnToTarget(delta);

		if (Math.Abs(GetDeltaAngleToTarget()) < 45f)
		{
			// FirePrimary();
			StopEngine();
		}		
	}

	private void PerformApproach(double delta)
	{
		var deltaPos = _targetPosition - Position;

		var desiredDeltaV = deltaPos.Normalized() * (float)Math.Sqrt(MaxAcceleration * deltaPos.Length());
		var deltaVDelta = desiredDeltaV - Velocity;

		var rotationDelta = deltaVDelta.Angle() * 180f / Math.PI - RotationDegrees;
		if (Math.Abs(rotationDelta) < 60f)
		{
			RunEngine(delta);
		}
		else
		{
			StopEngine();
		}
		DeltaRotation = Math.Sign(rotationDelta) * TurnRateDegreesPerSecond * (float)delta;
	}

	public void OnTargetPositionUpdated(Vector2 position, Vector2 velocity)
	{
		_targetPosition = position;
		_targetVelocity = velocity;
		_targetRotation = Position.AngleToPoint(_targetPosition);
	}	

	private void TurnToTarget(double delta)
	{
		var rotationDelta = _targetRotation - (RotationDegrees - 90f);
		if (rotationDelta < -180f)
		{
			rotationDelta += 360f;
		}
		else if (rotationDelta > 180f)
		{
			rotationDelta -= 360f;
		}
		var rotationStep = TurnRateDegreesPerSecond * (float)delta;

		if (Math.Abs(rotationDelta) <= rotationStep)
		{
			DeltaRotation = rotationDelta;
		}
		else
		{
			DeltaRotation = Math.Sign(rotationDelta) * rotationStep;
		}
	}

	private void RunEngine(double delta)
	{
		if (!IsEngineRunning)
		{
			GD.Print("Engine running");
			IsEngineRunning = true;
			GetNode<AnimatedSprite2D>("ShipSprite/EngineFlame").Visible = true;
		}
		DeltaVelocity = MaxAcceleration * (float)delta;
	}

	private void StopEngine()
	{
		if (IsEngineRunning)
		{
			GD.Print("Engine stopped");
			IsEngineRunning = false;
			GetNode<AnimatedSprite2D>("ShipSprite/EngineFlame").Visible = false;
		}
	}

	private float GetDeltaAngleToTarget()
	{
		return _targetRotation - RotationDegrees;
	}
}
