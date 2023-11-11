using Godot;
using System;

public partial class EnemyBase : Area2D
{
	[Export]
	private AudioStreamPlayer _audioHit;

	[Export]
	private AudioStreamPlayer _audioFire;

	public AudioStreamPlayer AudioHit
	{
		get;
	}

	public AudioStreamPlayer AudioFire
	{
		get;
	}


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void GetHit()
	{

	}

	public void PlaySFXHit()
	{
		_audioHit.Play(0f);
	}

	public void PlaySFXFire()
	{
		_audioFire.Play(0f);
	}
}
