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

	public void OnTargetPositionUpdated(Godot.Vector2 position, Godot.Vector2 velocity)
	{
		_targetPosition = position;
		_angleToTarget.InRadians = GlobalPosition.AngleToPoint(_targetPosition);
	}

	private void TurnTurret(double delta)
	{
		// Get the rotation the turret has to go for to face the target
		var relativeRotationDegrees = new Angle(RotationDegrees - GlobalRotationDegrees);
		var targetRotation = _angleToTarget - relativeRotationDegrees;
		GD.Print($"targetRotation: {targetRotation.InDegrees}");
		// Set the rotation step and adjust for movement limit
		DeltaRotation = new Angle(Math.Sign(targetRotation.InDegrees - RotationDegrees) * TurnRateDegreesPerSecond * (float)delta);
		GD.Print($"DeltaRotation before correction: {DeltaRotation.InDegrees}");
		if (RotationDegrees + DeltaRotation.InDegrees > MaxRotationDegrees)
		{
			DeltaRotation = new Angle(MaxRotationDegrees - RotationDegrees);
		}
		else if (RotationDegrees + DeltaRotation.InDegrees < -MinRotationDegrees)
		{
			DeltaRotation = new Angle(MinRotationDegrees - RotationDegrees);
		}
		GD.Print($"DeltaRotation after correction: {DeltaRotation.InDegrees}");
	}
}
