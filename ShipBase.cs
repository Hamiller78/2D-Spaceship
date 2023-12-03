namespace SpaceGame.Sprites;

using Godot;
using System;

public partial class ShipBase : Area2D
{
	[Export]
	public float MaxAcceleration { get; set; } = 200f;
	
	[Export]
	public float TurnRateDegreesPerSecond { get; set; } = 360f * 0.4f;
	
	[Export]
	public PackedScene LaserShotScene { get; set; }

    [Signal]
    public delegate void PositionUpdatedEventHandler(Vector2 position, Vector2 velocity);

	[Signal]
	public delegate void ShipDestroyedEventHandler(PlayerShip playerShip);

	private Vector2 _screenSize;

	public Vector2 Velocity
	{
		get => _velocity;
	}

	protected float DeltaSpeed;
	protected float DeltaRotation;
	protected float DeltaVelocity;

	private Vector2 _velocity = Vector2.Zero;

	protected bool IsEngineRunning = false;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_screenSize = GetViewportRect().Size;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Movement
		RotationDegrees += DeltaRotation;
		RotationDegrees %= (float)360f;

		var vx = (float)(_velocity.X + Math.Cos(Rotation) * DeltaVelocity);
		var vy = (float)(_velocity.Y + Math.Sin(Rotation) * DeltaVelocity);
		_velocity = new Vector2(vx, vy);

		var newX = (float)(Position.X + _velocity.X * delta);
		var newY = (float)(Position.Y + _velocity.Y * delta);
		var newPosition = new Vector2(newX, newY);
		Position = newPosition;

		EmitSignal(SignalName.PositionUpdated, newPosition, _velocity);

		// Sound
		if (!IsEngineRunning && DeltaVelocity > 0f)
		{
			IsEngineRunning = true;
			GetNode<AudioStreamPlayer2D>("EngineAudioPlayer").Play();
			GetNode<AnimatedSprite2D>("ShipSprite/EngineSprite").Visible = true;
		}
		else
		{
			if (IsEngineRunning)
			{
				IsEngineRunning = false;
				GetNode<AudioStreamPlayer2D>("EngineAudioPlayer").Stop();
				GetNode<AnimatedSprite2D>("ShipSprite/EngineSprite").Visible = false;
			}			
		}
	}

	public void OnAreaEntered(Area2D area)
	{
		if (area is LaserShot)
		{
			EmitSignal(SignalName.ShipDestroyed, this);
			QueueFree();
		}
	}

	private void FirePrimary()
	{
		var newShot = LaserShotScene.Instantiate<LaserShot>();
		newShot.Position
			= Position
				+ new Vector2(
					70f * (float)Math.Cos(Rotation),
					70f * (float)Math.Sin(Rotation));;
		newShot.Rotation = Rotation;
		newShot.Velocity = Velocity
			+ new Vector2(
			  	(float)Math.Cos(newShot.Rotation) * newShot.Speed,
				(float)Math.Sin(newShot.Rotation) * newShot.Speed
			);
		GetParent().AddChild(newShot);
	}
}
