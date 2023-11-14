using Godot;
using System;

public partial class Bullet : Area2D
{
	private const float SPEED = 500f;

	public enum Type
	{
		PLAYER = 0,
		ENEMY = 1
	}

	public Type BulletType
	{
		get;
		set;
	} = Type.PLAYER;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (GameManager.IsPaused)
		{
			return;
		}
		
		int _yDir = BulletType == Type.PLAYER ? -1 : 1;
		this.Position += new Vector2(0f, (float)(SPEED * delta * _yDir));

		if (this.Position.Y < -10f || this.Position.Y > 1300f)
		{
			QueueFree();
		}
	}

	private void _on_area_entered(Area2D area)
	{
		if (area is Bullet bullet)
		{
			if (bullet.BulletType != BulletType)
			{
				bullet.QueueFree();

				QueueFree();
			}
		}
	}
}
