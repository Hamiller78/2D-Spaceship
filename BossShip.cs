namespace SpaceGame.Sprites;

using Godot;
using SpaceGame.Utlities;
using System;

public partial class BossShip : ShipBase
{
	[Export]
	public float CombatRange { get; set; } = 800f;

	[Signal]
	public delegate void TargetPositionUpdatedEventHandler(Vector2 position, Vector2 velocity);

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
	}

	private void PerformApproach(double delta)
	{
		var deltaPos = _targetPosition - Position;

		var desiredDeltaV = deltaPos.Normalized() * (float)Math.Sqrt(MaxAcceleration * deltaPos.Length());
		var deltaVDelta = desiredDeltaV - Velocity;

		var deltaVDeltaAngle = new Angle() { InRadians = deltaVDelta.Angle() };
		var rotationDegreesAngle = new Angle(RotationDegrees);
		var rotationDelta = deltaVDeltaAngle - rotationDegreesAngle;
		if (rotationDelta.InDegrees > 300f || rotationDelta.InDegrees < 60f)
		{
			RunEngine(delta);
		}
		else
		{
			StopEngine();
		}
		DeltaRotation.InDegrees = rotationDegreesAngle.GetTurnDirection(deltaVDeltaAngle, new Angle(0f), new Angle(360f))
									* TurnRateDegreesPerSecond
									* (float)delta;
	}

	public void OnTargetPositionUpdated(Vector2 position, Vector2 velocity)
	{
		_targetPosition = position;
		_targetVelocity = velocity;
		_targetRotation.InRadians = Position.AngleToPoint(_targetPosition);
		EmitSignal(SignalName.TargetPositionUpdated, position, velocity);
	}

	private void TurnToTarget(double delta)
	{
		var newRotation = NavigationManager.GetNewRotation(
			RotationDegrees,
			_targetRotation.InDegrees,
			TurnRateDegreesPerSecond,
			0f,
			360f,
			delta);
		DeltaRotation = new Angle(0f);  // TODO: Remove this when it is no longer used in base class
		RotationDegrees = newRotation;
	}

	private void RunEngine(double delta)
	{
		DeltaVelocity = MaxAcceleration * (float)delta;
	}

	private void StopEngine()
	{
		DeltaVelocity = 0f;
	}
}
