using Godot;
using System;

public partial class Main : Node
{
	[Export]
	public PackedScene TurretScene { get; set; }

	[Export]
	public PackedScene ExplosionScene { get; set; }

	public PackedScene MainScene { get; set; }

	private const int TURRET_COUNT = 10;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
		for (int i=0;i < TURRET_COUNT;i++)
		{
			SpawnTurret();
		}
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		GetNode<Camera2D>("Camera2D").Position = GetNode<PlayerShip>("Player").Position;
	}

	public void OnTurretDestroyed(Turret turret)
	{
		var explosion = ExplosionScene.Instantiate<Explosion>();
		explosion.Position = turret.Position;
		AddChild(explosion);
	}

	public void OnPlayerShipDestroyed(PlayerShip playerShip)
	{
		var explosion = ExplosionScene.Instantiate<Explosion>();
		explosion.Position = playerShip.Position;
		AddChild(explosion);
	}

	private void SpawnTurret()
	{
		var screenSize = GetNode<Area2D>("Player").GetViewportRect().Size;
		var turret = TurretScene.Instantiate<Turret>();
		turret.Position = new Vector2(GD.Randi() % (screenSize.X * 2f), GD.Randi() % (screenSize.Y * 2f));
		turret.Connect(nameof(Turret.TurretDestroyed), new Callable(this, nameof(OnTurretDestroyed)));

		var playerShip = GetNode<PlayerShip>("Player");
		playerShip.Connect(nameof(PlayerShip.PositionUpdated), new Callable(turret, nameof(Turret.OnTargetPositionUpdated)));

		AddChild(turret);
	}
}
