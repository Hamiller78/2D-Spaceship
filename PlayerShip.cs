using Godot;
using System;

public partial class PlayerShip : Area2D
{
	[Export]
	public int Speed { get; set; } = 400; // How fast the player will move (pixels/sec).
	
	public Vector2 ScreenSize;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
		Position = new Vector2(ScreenSize.X / 2, ScreenSize.Y / 2);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var velocity = Vector2.Zero; // The player's movement vector.
		var rotation = Rotation;

		if (Input.IsActionPressed("turn_left"))
		{
			Rotation -= 10f * (float)delta;
			Rotation = Rotation % (float)(2d * Math.PI);
			GD.Print(Rotation);
		}

		if (Input.IsActionPressed("turn_right"))
		{
			Rotation += 10f * (float)delta;
			Rotation = Rotation % (float)(2d * Math.PI);
			GD.Print(Rotation);
		}

		if (Input.IsActionPressed("thrust_forward"))
		{
			var newX = (float)(Position.X + Math.Cos(Rotation - Math.PI / 2d));
			var newY = (float)(Position.Y + Math.Sin(Rotation - Math.PI / 2d));
			var newPosition = new Vector2(newX, newY);
			Position = newPosition;
		}	
	}
}
