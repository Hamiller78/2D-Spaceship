using Godot;
using System;

public partial class PlayerShip : Area2D
{
	[Export]
	public float Acceleration { get; set; } = 200f;
	
	[Export]
	public float RadsPerSecond { get; set; } = 2f * (float)Math.PI * 0.4f;
	
	[Export]
	public PackedScene LaserShotScene { get; set; }

    [Signal]
    public delegate void PositionUpdatedEventHandler(Vector2 position);

	[Signal]
	public delegate void PlayerShipDestroyedEventHandler(PlayerShip playerShip);

	public Vector2 ScreenSize;

	public Vector2 Velocity
	{
		get => _velocity;
	}

	private Vector2 _velocity = Vector2.Zero;

	private bool _isEngineRunning = false;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
		Position = new Vector2(-500f, ScreenSize.Y / 2f);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Movement
		if (Input.IsActionPressed("turn_left"))
		{
			Rotation -= RadsPerSecond * (float)delta;
			Rotation = Rotation % (float)(2d * Math.PI);
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
		}

		var newX = (float)(Position.X + _velocity.X * delta);
		var newY = (float)(Position.Y + _velocity.Y * delta);

		var newPosition = new Vector2(newX, newY);
		Position = newPosition;
		EmitSignal(SignalName.PositionUpdated, newPosition);
		
		// Sound
		if (Input.IsActionPressed("turn_left")
			|| Input.IsActionPressed("turn_right")
			|| Input.IsActionPressed("thrust_forward"))
		{
			if (!_isEngineRunning)
			{
				_isEngineRunning = true;
				GetNode<AudioStreamPlayer>("EngineSound").Play();
				GetNode<AnimatedSprite2D>("PlayerShip/EngineFlame").Visible = true;		
			}
		}
		else
		{
			if (_isEngineRunning)
			{
				_isEngineRunning = false;
				GetNode<AudioStreamPlayer>("EngineSound").Stop();
				GetNode<AnimatedSprite2D>("PlayerShip/EngineFlame").Visible = false;
			}			
		}

		// Functions
		if (Input.IsActionJustPressed("fire_primary"))
		{
			FirePrimary();
		}
	}

	public void OnAreaEntered(Area2D area)
	{
		if (area is LaserShot)
		{
			EmitSignal(SignalName.PlayerShipDestroyed, this);
			QueueFree();
		}
	}

	private void FirePrimary()
	{
		var newShot = LaserShotScene.Instantiate<LaserShot>();
		newShot.Position
			= Position
				+ new Vector2(
					70f * (float)Math.Cos(Rotation - Math.PI / 2d),
					70f * (float)Math.Sin(Rotation - Math.PI / 2d));;
		newShot.Rotation = Rotation;
		newShot.Velocity = Velocity
			+ new Vector2(
			  	(float)Math.Cos(newShot.Rotation - Math.PI / 2d) * newShot.Speed,
				(float)Math.Sin(newShot.Rotation - Math.PI / 2d) * newShot.Speed
			);
		GetParent().AddChild(newShot);
	}	
}
