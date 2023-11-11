using Godot;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

public partial class Player : Character
{
	[Signal]
	public delegate void LiveChangeEventHandler(int currentLives);

	[Signal]
	public delegate void DieEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		Lives = Constants.DEFAULT_PLAYER_LIVES;

		_bulletsNode = GetNode("../Bullets") as Node2D;
		_bulletRes = ResourceLoader.Load(Godot.ProjectSettings.GlobalizePath("res://scenes/Bullet.tscn")) as PackedScene;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (GameManager.IsPaused || !CanPlay)
		{
			return;
		}

		base._Process(delta);

		float input = Input.GetActionStrength("ui_left") - Input.GetActionStrength("ui_right");

		if (input > 0)
		{
			_animationPlayer.Play("MoveLeft");
		}
		else if (input < 0)
		{
			_animationPlayer.Play("MoveRight");
		}
		else
		{
			_animationPlayer.Play("Idle");
		}

		if (Input.IsActionJustPressed("ui_accept") && CanFireBullet)
		{
			FireBullet();
		}

		Vector2 position = this.Position;
		position -= new Vector2((float)(input * Constants.DEFAULT_MOVE_SPEED * delta), 0f);
		position.X = Mathf.Clamp(position.X, 0, Constants.SCREEN_WIDTH);
		this.Position = position;
	}

	public override void FireBullet()
	{
		if (!CanPlay || !CanFireBullet)
		{
			return;
		}

		_audioFire.Play(0f);

		Bullet newBullet = _bulletRes.Instantiate() as Bullet;
		newBullet.Position = this.Position - new Vector2(0, _pixelSize.Y * 0.5f);
		newBullet.BulletType = Bullet.Type.PLAYER;
		_bulletsNode.AddChild(newBullet);

		ResetBulletTimer();
	}

	public override async void GetHit()
	{
		CanPlay = false;

		Lives -= 1;
		EmitSignal(SignalName.LiveChange, Lives);

		_animationPlayer.Play("Explode");
		_audioExplode.Play(0f);

		await ToSignal(_animationPlayer, "animation_finished");

		if (Lives > 0)
		{
			_animationPlayer.Play("Revive");

			await ToSignal(_animationPlayer, "animation_finished");

			_animationPlayer.Play("Idle");

			CanPlay = true;
		}
		else
		{
			EmitSignal(SignalName.Die);
		}
	}

	public override void _on_area_entered(Area2D area)
	{
		if (area is Bullet bullet)
		{			
			if (bullet.BulletType == Bullet.Type.ENEMY && CanPlay)
			{
				GetHit();

				bullet.QueueFree();
			}
		}
	}
}
