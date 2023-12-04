using Godot;
using System;

using SpaceGame.Sprites;
using SpaceGame.Utlities;

public partial class EnemyShip2 : ShipBase
{
	[Export]
	public float CombatRange { get; set; } = 800f;

	private Vector2 _targetPosition;
	private Vector2 _targetVelocity;
	private Angle _targetRotation = new();

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
		if (_maneuverMode == ManeuverMode.Approach && targetDistance <= CombatRange)
		{
			_maneuverMode = ManeuverMode.Attack;
		}
		else if (_maneuverMode == ManeuverMode.Attack && targetDistance > CombatRange + 100f)
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
		StopEngine();
		TurnToTarget(delta);

		if (Math.Abs(_targetRotation.InDegrees - RotationDegrees) < 45f)
		{
			FirePrimary();
			StopEngine();
		}		
	}

	private void PerformApproach(double delta)
	{
		var deltaPos = _targetPosition - Position;

		var desiredDeltaV = deltaPos.Normalized() * (float)Math.Sqrt(MaxAcceleration * deltaPos.Length());
		var deltaVDelta = desiredDeltaV - Velocity;

		var rotationDelta = new Angle() { InRadians = deltaVDelta.Angle() - Rotation };
		if (Math.Abs(rotationDelta.InDegrees) < 60f)
		{
			RunEngine(delta);
		}
		else
		{
			StopEngine();
		}
		DeltaRotation.InDegrees = Math.Sign(rotationDelta.InDegrees) * TurnRateDegreesPerSecond * (float)delta;
	}

	public void OnTargetPositionUpdated(Vector2 position, Vector2 velocity)
	{
		_targetPosition = position;
		_targetVelocity = velocity;
		_targetRotation.InRadians = Position.AngleToPoint(_targetPosition);
	}	

	private void TurnToTarget(double delta)
	{
		var rotationDelta = _targetRotation - new Angle(RotationDegrees);
		var rotationStep = new Angle(TurnRateDegreesPerSecond * (float)delta);

		if (Math.Abs(rotationDelta.InDegrees) <= rotationStep.InDegrees)
		{
			DeltaRotation = rotationDelta;
		}
		else
		{
			DeltaRotation = new Angle(Math.Sign(rotationDelta.InDegrees) * rotationStep.InDegrees);
		}
	}

	private void RunEngine(double delta)
	{
		if (!IsEngineRunning)
		{
			IsEngineRunning = true;
			var engineSprite = GetNode<AnimatedSprite2D>("EnemyShip2/ShipArea/ShipSprite/EngineSprite");
			engineSprite.Visible = true;
		}
		DeltaVelocity = MaxAcceleration * (float)delta;
	}

	private void StopEngine()
	{
		if (IsEngineRunning)
		{
			IsEngineRunning = false;
			var engineSprite = GetNode<AnimatedSprite2D>("EnemyShip2/ShipArea/ShipSprite/EngineSprite");
			engineSprite.Visible = false;
		}
		DeltaVelocity = 0f;
	}
}
