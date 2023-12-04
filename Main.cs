using Godot;
using System;
using System.Numerics;

using SpaceGame.Sprites;

public partial class Main : Node
{
	[Export]
	public PackedScene TurretScene { get; set; }

	[Export]
	public PackedScene ExplosionScene { get; set; }

	[Signal]
	public delegate void PrintDebugMessageEventHandler(string debugMessage);

	public PackedScene MainScene { get; set; }

	private const int TURRET_COUNT = 10;

	private int _score = 0;
	private int _turretsLeft = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
		SpawnTurrets();
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		var camera = GetNode<Camera2D>("Camera2D");
		var playerShip = GetNode<Area2D>("Player");
		camera.Position = playerShip.Position;

		var closestEnemyPosition = GetFurthestTurret();
		var deltaX = Math.Abs(playerShip.Position.X - closestEnemyPosition.X);
		var deltaY = Math.Abs(playerShip.Position.Y - closestEnemyPosition.Y);
		playerShip = GetNode<PlayerShip>("Player");
		var screenSize = GetNode<Area2D>("Player").GetViewportRect().Size;
		var zoomX = Math.Min(1f, 0.5f * screenSize.X / deltaX);
		var zoomY = Math.Min(1f, 0.5f * screenSize.Y / deltaY);
		var zoom = Math.Min(zoomX, zoomY);

		camera.Zoom = new Godot.Vector2(zoom, zoom);
	}

	public void OnTurretDestroyed(Turret turret)
	{
		var explosion = ExplosionScene.Instantiate<Explosion>();
		explosion.Position = turret.Position;
		AddChild(explosion);
		_score++;
		_turretsLeft--;
		EmitSignal(SignalName.PrintDebugMessage, $"Score: {_score}");

		if (_turretsLeft == 0)
		{
			SpawnTurrets();
		}
	}

	public void OnPlayerShipDestroyed(PlayerShip playerShip)
	{
		var explosion = ExplosionScene.Instantiate<Explosion>();
		explosion.Position = playerShip.Position;
		AddChild(explosion);
	}

	public void OnEnemyShipDestroyed(EnemyShip enemyShip)
	{
		var explosion = ExplosionScene.Instantiate<Explosion>();
		explosion.Position = enemyShip.Position;
		AddChild(explosion);
	}

	public void OnShipDestroyed(ShipBase ship)
	{
		var explosion = ExplosionScene.Instantiate<Explosion>();
		explosion.Position = ship.Position;
		AddChild(explosion);
	}


	private void SpawnTurrets()
	{
		for (int i = 0; i < TURRET_COUNT; i++)
		{
			var screenSize = GetNode<Area2D>("Player").GetViewportRect().Size;
			var turret = TurretScene.Instantiate<Turret>();
			turret.Position = new Godot.Vector2(GD.Randi() % (screenSize.X * 2f), GD.Randi() % (screenSize.Y * 2f));
			turret.Connect(nameof(Turret.TurretDestroyed), new Callable(this, nameof(OnTurretDestroyed)));

			var playerShip = GetNode<PlayerShip>("Player");
			playerShip.Connect(nameof(PlayerShip.PositionUpdated), new Callable(turret, nameof(Turret.OnTargetPositionUpdated)));

			AddChild(turret);
			turret.AddToGroup("Turrets");
		}
		_turretsLeft += TURRET_COUNT;
	}

	private Godot.Vector2 GetFurthestTurret()
	{
		var playerShip = GetNode<PlayerShip>("Player");
		var furthestDistance = 0f;
		Turret furthestTurret = null;

		foreach (var turretNode in GetTree().GetNodesInGroup("Turrets"))
		{
			var turret = turretNode as Turret;
			var distance = turret.Position.DistanceTo(playerShip.Position);
			if (distance > furthestDistance)
			{
				furthestDistance = distance;
				furthestTurret = turret;
			}
		}

		return furthestTurret.Position;
	}
}
