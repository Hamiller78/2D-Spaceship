using Godot;
using System;

public partial class PlayerShip : Area2D
{
	[Export]
	public float Acceleration { get; set; } = 200f; // How fast the player will move (pixels/sec).
	
	[Export]
	public float RadsPerSecond { get; set; } = 2f * (float)Math.PI * 0.4f;
	
	public Vector2 ScreenSize;
	
	private Vector2 _velocity = Vector2.Zero;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
		Position = new Vector2(ScreenSize.X / 2, ScreenSize.Y / 2);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var rotation = Rotation;

		if (Input.IsActionPressed("turn_left"))
		{
			Rotation -= RadsPerSecond * (float)delta;
			Rotation = Rotation % (float)(2d * Math.PI);
			GD.Print(Rotation);
		}

		if (Input.IsActionPressed("turn_right"))
		{
			Rotation += RadsPerSecond * (float)delta;
			Rotation = Rotation % (float)(2d * Math.PI);
		}

		if (Input.IsActionPressed("thrust_forward"))
		{
			var vx = (float)(_velocity.X + Math.Cos(Rotation - Math.PI / 2d) * Acceleration * delta);
			var vy = (float)(_velocity.Y + Math.Sin(Rotation - Math.PI / 2d) * Acceleration * delta);
			_velocity = new Vector2(vx, vy);
			GD.Print(vy);
		}
			
		var newX = (float)(Position.X + _velocity.X * delta + ScreenSize.X) % ScreenSize.X;
		var newY = (float)(Position.Y + _velocity.Y * delta + ScreenSize.Y) % ScreenSize.Y;
		var newPosition = new Vector2(newX, newY);
		Position = newPosition;
	}
}
