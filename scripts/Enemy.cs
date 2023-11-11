using Godot;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

public partial class Enemy : Character
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		Lives = Constants.DEFAULT_ENEMY_LIVES;

		_bulletsNode = GetNode("../../Bullets") as Node2D;
		_bulletRes = ResourceLoader.Load(Godot.ProjectSettings.GlobalizePath("res://scenes/BulletEnemy.tscn")) as PackedScene;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (GameManager.IsPaused)
		{
			return;
		}

		base._Process(delta);

		if (CanPlay && CanFireBullet)
		{
			FireBullet();
		}
	}

	public override void FireBullet()
	{
		_audioFire.Play(0f);

		Bullet newBullet = _bulletRes.Instantiate() as Bullet;
		newBullet.Position = this.Position + new Vector2(0, _pixelSize.Y * 0.5f + 10f);
		newBullet.BulletType = Bullet.Type.ENEMY;
		_bulletsNode.AddChild(newBullet);

		ResetBulletTimer();
	}

	public override async void GetHit()
	{
		CanPlay = false;

		_animationPlayer.Play("Explode");
		_audioExplode.Play(0f);

		await ToSignal(_animationPlayer, "animation_finished");

		QueueFree();
	}

	public override void _on_area_entered(Area2D area)
	{
        if (area is Bullet bullet)
		{
			if (bullet.BulletType == Bullet.Type.PLAYER)
			{
				GetHit();

				bullet.QueueFree();
			}
		}
	}
}
