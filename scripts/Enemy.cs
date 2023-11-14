using System;
using System.Threading.Tasks;
using Godot;

public partial class Enemy : Character
{
	[Signal]
	public delegate void DieEventHandler(int points);

	private EnemyData _enemyData;

	private int _id;
	private int _points;
	private int _hp;


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
		if (GameManager.IsPaused || !CanPlay)
		{
			return;
		}

		base._Process(delta);
	}

	public void SetupData(EnemyData data)
	{
		_enemyData = data;

		_id = data.Id;
		_points = data.Points;
		_hp = data.Hp;
	}

	public override void FireBullet()
	{
		_audioFire?.Play(0f);

		Bullet newBullet = _bulletRes.Instantiate() as Bullet;
		newBullet.Position = this.GlobalPosition + new Vector2(_sprite.Position.X, _pixelSize.Y * 0.5f + 10f);
		newBullet.BulletType = Bullet.Type.ENEMY;
		_bulletsNode.AddChild(newBullet);

		ResetBulletTimer();
	}

	public override async void GetHit()
	{
		CanPlay = false;

		EmitSignal(SignalName.Die, _points);

		_animationPlayer?.Play("Explode");
		_audioExplode?.Play(0f);

		await ToSignal(_animationPlayer, "animation_finished");

		QueueFree();
		//this.Visible = false;
	}

    public override void Reset()
    {
        base.Reset();

		_hp = _enemyData.Hp;

		_animationPlayer.Play("Default");
    }

	public void StopAnimation()
	{
		_animationPlayer.Stop();
	}

	public async void StartAnimationIdle()
	{
		await Task.Delay(new Random().Next(0, 1500));

		_animationPlayer.Play("Idle");
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
		else if (area is Barrier barrier)
		{
			GetHit();

			barrier.GetHit();
		}
	}
}
