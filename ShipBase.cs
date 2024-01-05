namespace SpaceGame.Sprites;

using Godot;
using System;

using SpaceGame.Utlities;

public partial class ShipBase : Area2D
{
	[Export]
	public float MaxAcceleration { get; set; }

	[Export]
	public float TurnRateDegreesPerSecond { get; set; }

	[Export]
	public PackedScene LaserShotScene { get; set; }

	[Export]
	public float RechargeTime { get; set; }

	[Export]
	public PackedScene ExplosionScene { get; set; }

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
	protected Angle DeltaRotation = new();
	protected float DeltaVelocity;

	private Vector2 _velocity = Vector2.Zero;

	protected bool IsEngineRunning = false;

	private float _rechargeTimeRemaining = 0f;
	private AudioStreamPlayer2D _engineAudioPlayer;
	private AnimatedSprite2D _engineSprite;

	public override void _Ready()
	{
		_screenSize = GetViewportRect().Size;
		try
		{
			_engineAudioPlayer = GetNode<AudioStreamPlayer2D>("EngineAudioPlayer");
		}
		catch
		{
			_engineAudioPlayer = null;
		}
		if (_engineAudioPlayer is not null)
		{
			_engineAudioPlayer.StreamPaused = true;
		}
		try
		{
			_engineSprite = GetNode<AnimatedSprite2D>("ShipSprite/EngineSprite");
			_engineSprite.Play();
		}
		catch
		{
			_engineSprite = null;
		}
	}

	public override void _Process(double delta)
	{
		// Movement
		var newRotation = new Angle(RotationDegrees) + DeltaRotation;
		RotationDegrees = newRotation.InDegrees;

		var vx = (float)(_velocity.X + Math.Cos(Rotation) * DeltaVelocity);
		var vy = (float)(_velocity.Y + Math.Sin(Rotation) * DeltaVelocity);
		_velocity = new Vector2(vx, vy);

		var newX = (float)(Position.X + _velocity.X * delta);
		var newY = (float)(Position.Y + _velocity.Y * delta);
		var newPosition = new Vector2(newX, newY);
		Position = newPosition;

		EmitSignal(SignalName.PositionUpdated, newPosition, _velocity);

		// Sound & Engine flame
		if (!IsEngineRunning && DeltaVelocity > 0f)
		{
			IsEngineRunning = true;
			if (_engineAudioPlayer is not null)
			{
				_engineAudioPlayer.StreamPaused = false;
			}
			if (_engineSprite is not null)
			{
				_engineSprite.Visible = true;
			}
		}
		else
		{
			if (IsEngineRunning)
			{
				IsEngineRunning = false;
				if (_engineAudioPlayer is not null)
				{
					_engineAudioPlayer.StreamPaused = true;
				}
				if (_engineSprite is not null)
				{
					_engineSprite.Visible = false;
				}
			}
		}

		if (_rechargeTimeRemaining > 0f)
		{
			_rechargeTimeRemaining -= (float)delta;
		}
	}

	public void OnAreaEntered(Area2D area)
	{
		if (area is LaserShot)
		{
			var explosion = ExplosionScene.Instantiate<Explosion>();
			explosion.Position = GlobalPosition;
			GetTree().Root.AddChild(explosion);
			QueueFree();
			area.QueueFree();
		}
	}

	protected void FirePrimary()
	{
		if (_rechargeTimeRemaining > 0f)
		{
			return;
		}

		var newShot = LaserShotScene.Instantiate<LaserShot>();
		newShot.Position
			= Position
				+ new Vector2(
					90f * (float)Math.Cos(Rotation),
					90f * (float)Math.Sin(Rotation));
		newShot.Rotation = Rotation;
		newShot.Velocity = Velocity
			+ new Vector2(
			  	(float)Math.Cos(newShot.Rotation) * newShot.Speed,
				(float)Math.Sin(newShot.Rotation) * newShot.Speed
			);
		GetParent().AddChild(newShot);

		_rechargeTimeRemaining = RechargeTime;
	}
}
