using Godot;
using System;

public partial class Barrier : Area2D
{
	[Export]
	private AudioStreamPlayer _audioStreamPlayer;

	[Export]
	private AnimationPlayer _animationPlayer;

	
	public int Lives
	{
		get;
		set;
	} = Constants.DEFAULT_BARRIER_LIVES;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_animationPlayer.Play("4of4");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_accept"))
		{
			//GetHit();
		}
	}

	private void _on_area_entered(Area2D area)
	{

	}

	public async void GetHit()
	{
		Lives -= 1;

		_audioStreamPlayer.Play(0f);


		if (Lives <= 0)
		{
			Visible = false;

			await ToSignal(_audioStreamPlayer, "finished");
			
			QueueFree();
		}
		else
		{
			_animationPlayer.Play($"{Lives}of4");
		}
	}
}
