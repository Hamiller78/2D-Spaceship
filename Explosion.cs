using Godot;
using System;

public partial class Explosion : Node2D
{

	private bool _isAnimationFinished = false;
	private bool _isSoundFinished = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnAnimationFinished()
	{
		_isAnimationFinished = true;
		if (_isSoundFinished)
		{
			QueueFree();
		}
	}

	public void OnSoundFinished()
	{
		_isSoundFinished = true;
		if (_isAnimationFinished)
		{
			QueueFree();
		}
	}
}
