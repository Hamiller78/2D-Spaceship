namespace SpaceGame.Sprites;

using Godot;

using SpaceGame.Utlities;

public partial class PlayerShip : ShipBase
{
	public override void _Ready()
	{
		Position = new Vector2(-500f, 175f);
		base._Ready();
	}

	public override void _Process(double delta)
	{
		// Movement
		DeltaRotation = new Angle(0f);

		if (Input.IsActionPressed("turn_left"))
		{
			DeltaRotation = new Angle(-TurnRateDegreesPerSecond * (float)delta);
		}

		if (Input.IsActionPressed("turn_right"))
		{
			DeltaRotation = new Angle(TurnRateDegreesPerSecond * (float)delta);
		}

		DeltaVelocity = 0f;
		if (Input.IsActionPressed("thrust_forward"))
		{
			DeltaVelocity = MaxAcceleration * (float)delta;
		}

		// Functions
		if (Input.IsActionPressed("fire_primary"))
		{
			FirePrimary();
		}

		base._Process(delta);
	}
}