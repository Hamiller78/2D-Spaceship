using Godot;
using System;

public partial class Main : Node
{
	[Export]
	public PackedScene LaserShotScene { get; set; }

	public PackedScene MainScene { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void FirePrimary()
	{
		var newShot = LaserShotScene.Instantiate<LaserShot>();
		newShot.Position = GetNode<PlayerShip>("Player").Position;
		newShot.Rotation = GetNode<PlayerShip>("Player").Rotation;
		newShot.Velocity = GetNode<PlayerShip>("Player").Velocity
			+ new Vector2(
			  	(float)Math.Cos(newShot.Rotation - Math.PI / 2d) * newShot.Speed,
				(float)Math.Sin(newShot.Rotation - Math.PI / 2d) * newShot.Speed
			);
		AddChild(newShot);
	}
}
