using Godot;
using System;

public partial class Main : Node
{
	[Export]
	public PackedScene LaserShotScene { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (Input.IsActionJustPressed("fire_primary"))
		{
			FirePrimary();
		}		
	}

	private void FirePrimary()
	{
		var newShot = LaserShotScene.Instantiate<LaserShot>();
		// newShot.Position = new Vector2(1000f, 0f);
		newShot.Position = GetNode<PlayerShip>("Player").Position;
		newShot.Rotation = GetNode<PlayerShip>("Player").Rotation;
		
		AddChild(newShot);
	}

}
