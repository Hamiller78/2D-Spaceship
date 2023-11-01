using Godot;
using System;

public partial class LaserShot : Area2D
{
	[Export]
	public double LifetimeInSeconds { get; set; } = 1.5d;

	private double _remainingLifetime;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_remainingLifetime = LifetimeInSeconds;
		GetNode<AudioStreamPlayer>("LaserSound").Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_remainingLifetime -= delta;
		if (_remainingLifetime <= 0d)
		{
			QueueFree();
		}
		Position += new Vector2(0f, 3f);
	}
}
