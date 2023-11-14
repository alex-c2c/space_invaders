using Godot;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

public partial class EnemyHive : Node2D
{
	[Export]
	private Node2D _bulletsNode;

	private LevelData _levelData;
	private double _moveIntervalSec;
	private double _moveIntervalFactor;
	private double _shootIntervalSec;
	private double _shootIntervalFactor;

	private bool _initialized = false;
	private double _moveCounter;
	private double _shootCounter;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	public void SetupData(LevelData levelData)
	{
		_levelData = levelData;

		Reset();

		_initialized = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (GameManager.IsPaused || !_initialized)
		{
			return;
		}

		_moveIntervalSec -= delta * _moveIntervalFactor;
		if (_moveIntervalSec <= 0)
		{
			Move();

			_moveCounter += 1f;

			_moveIntervalSec = _levelData.HiveMoveIntervalSec;
			_moveIntervalFactor += 0.05f;
		}

		_shootIntervalSec -= delta * _shootIntervalFactor;
		if (_shootIntervalSec <= 0)
		{
			if (!CheckCanFireBullet())
			{
				return;
			}

			FireBullet();

			_shootCounter += 1f;

			_shootIntervalSec = _levelData.HiveShootIntervalSec * new Random().Next(5,15) * 0.1f;// _shootIntervalFactor;
			_shootIntervalFactor += 0.05f;
		}

		//Debug.Print($"Move: {_moveIntervalSec} | Shoot: {_shootIntervalSec}");
	}

	public void Move()
	{
		if (_moveCounter % 6 == 1 || _moveCounter % 6 == 0)
		{
			// move right
			this.Position += new Vector2(20f, 0f);
		}
		else if (_moveCounter % 6 == 2 || _moveCounter % 6 == 5)
		{
			// move down
			this.Position += new Vector2(0f, 30f);
		}
		else if (_moveCounter % 6 == 3 || _moveCounter % 6 == 4)
		{
			// move left
			this.Position -= new Vector2(20f, 0f);
		}
	}

	public void FireBullet()
	{
		Enemy enemy = GetChild(GetRandomChildIndex()) as Enemy;
		enemy?.FireBullet();
		/*
		int[] indexArray = new int[3]{GetRandomChildIndex(), -1, -1};
		for (int i = 1; i < 3; i++)
		{
			while (true)
			{
				int newIndex = GetRandomChildIndex();
				bool hasDupe = false;

				for (int j = 0; j < i; j++)
				{
					if (indexArray[j] == newIndex)
					{
						hasDupe = true;
						break;
					}
				}
				
				if (!hasDupe)
				{
					indexArray[i] = newIndex;
					break;
				}
			}
		}

		for (int i = 0; i < 3; i++)
		{
			Enemy enemy = GetChild(indexArray[i]) as Enemy;
			if (enemy != null)
			{
				enemy.FireBullet();
			}

			await Task.Delay(new Random().Next(0, (int)(1000 * _shootIntervalFactor * _shootCounter)));
		}
		*/
	}

	public void Reset()
	{
		this.Position = Vector2.Zero;

		_moveIntervalSec = _levelData.HiveMoveIntervalSec;
		_moveIntervalFactor = _levelData.HiveMoveIntervalFactor;
		_shootIntervalSec = _levelData.HiveShootIntervalSec;
		_shootIntervalFactor = _levelData.HiveShootIntervalFactor;

		_moveCounter = 1f;
		_shootCounter = 1f;
	}

	private int GetEnemyBulletCount()
	{
		int count = 0;
		foreach (Bullet bullet in _bulletsNode.GetChildren())
		{
			if (bullet.BulletType == Bullet.Type.ENEMY)
			{
				count++;
			}
		}

		return count;
	}

	private bool CheckCanFireBullet()
	{
		float bCount = (float)GetEnemyBulletCount();
		float eCount = (float)GetChildCount();

		//Debug.Print($"{bCount} / {eCount} | {(bCount / eCount) < 0.15f}");

		return ((float)GetEnemyBulletCount() / (float)GetChildCount()) < 0.15f;
	}

	private int GetRandomChildIndex()
	{
		return new Random().Next(0, GetChildCount());
	}
}
