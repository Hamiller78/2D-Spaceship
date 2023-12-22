using Godot;
using System;

using SpaceGame.Sprites;
using SpaceGame.Utlities;
using System.Numerics;

public partial class Turret : ShipBase
{
	[Signal]
	public delegate void TurretDestroyedEventHandler(Turret turret);

	[Export]
	public float FireRange { get; set; }

	[Export]
	public float ViewRange { get; set; }

	[Export]
	public float MaxAngle { get; set; }

	private Godot.Vector2 _targetPosition;
	private Angle _angleToTarget = new();

	public override void _Ready()
	{
		base._Ready();
	}

	public override void _Process(double delta)
	{
		var targetDistance = _targetPosition.DistanceTo(Position);
		if (targetDistance > ViewRange)
		{
			base._Process(delta);
			return;
		}

		TurnTurret(delta);

		if (targetDistance > FireRange)
		{
			base._Process(delta);
			return;
		}

		FirePrimary();
		base._Process(delta);
	}

	public void OnTargetPositionUpdated(Godot.Vector2 position, Godot.Vector2 velocity)
	{
		_targetPosition = position;
		_angleToTarget.InRadians = Position.AngleToPoint(_targetPosition);
	}

	private void TurnTurret(double delta)
	{
		var targetAngle = _angleToTarget;
		if (Math.Abs(targetAngle.InDegrees) > MaxAngle)
		{
			targetAngle = new Angle(Math.Sign(targetAngle.InDegrees) * MaxAngle);
		}

		var rotationDelta = targetAngle - new Angle(RotationDegrees);
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
}
