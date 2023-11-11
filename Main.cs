using Godot;
using System;

public partial class Main : Node
{
	[Export]
	public PackedScene TurretScene { get; set; }

	public PackedScene MainScene { get; set; }


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var screenSize = GetNode<Area2D>("Player").GetViewportRect().Size;
		var turret = TurretScene.Instantiate<Turret>();
		turret.Position = new Vector2(GD.Randi() % screenSize.X, GD.Randi() % screenSize.Y);
		
		var playerShip = GetNode<PlayerShip>("Player");
		playerShip.Connect(nameof(PlayerShip.PositionUpdated), new Callable(turret, nameof(Turret.OnTargetPositionUpdated)));

		AddChild(turret);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
