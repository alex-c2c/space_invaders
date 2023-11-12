using Godot;
using System;

public partial class Barrier : Character
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		Lives = Constants.DEFAULT_BARRIER_LIVES;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public override async void GetHit()
	{
		if (!CanPlay)
		{
			return;
		}

		CanPlay = false;

		Lives -= 1;

		_audioExplode?.Play(0f);
		_animationPlayer?.Play($"{Lives}of4");

		if (Lives <= 0)
		{
			await ToSignal(_audioExplode, "finished");

			QueueFree();
		}

		CanPlay = true;
	}

	public override void _on_area_entered(Area2D area)
	{
		if (area is Bullet bullet)
		{
			if (bullet.BulletType == Bullet.Type.ENEMY)
			{
				GetHit();

				bullet.QueueFree();
			}
		}
	}
}
