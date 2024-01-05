using Godot;
using System;

using SpaceGame.Sprites;
using SpaceGame.Utlities;

public partial class Turret : ShipBase
{
	[Signal]
	public delegate void TurretDestroyedEventHandler(Turret turret);

	[Export]
	public float FireRange { get; set; }

	[Export]
	public float ViewRange { get; set; }

	[Export]
	public float StartRotationDegrees { get; set; }

	[Export]
	public float MinRotationDegrees { get; set; }

	[Export]
	public float MaxRotationDegrees { get; set; }

	private Vector2 _targetPosition;
	private Angle _angleToTarget = new();

	public override void _Ready()
	{
		RotationDegrees = StartRotationDegrees;
		base._Ready();
	}

	public override void _Process(double delta)
	{
		DeltaRotation = new Angle(0f);

		var targetDistance = _targetPosition.DistanceTo(GlobalPosition);
		if (targetDistance < ViewRange)
		{
			TurnTurret(delta);
		}

		if (targetDistance < FireRange)
		{
			FirePrimary();
		}

		base._Process(delta);
	}

	public void OnTargetPositionUpdated(Vector2 position, Vector2 velocity)
	{
		_targetPosition = position;
		_angleToTarget
			= NavigationManager.GetGlobalAngleToTarget(
				GlobalPosition,
				_targetPosition,
				GlobalRotationDegrees - RotationDegrees);
	}

	private void TurnTurret(double delta)
	{
		var newRotation = NavigationManager.GetNewRotation(
			RotationDegrees,
			_angleToTarget.InDegrees,
			TurnRateDegreesPerSecond,
			MinRotationDegrees,
			MaxRotationDegrees,
			delta);
		DeltaRotation = new Angle(0f);  // TODO: REmove this when it is no longer used in base class
		RotationDegrees = newRotation;
	}
}