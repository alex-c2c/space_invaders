using Godot;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;

public partial class Main : Node2D
{
	[Export]
	private Node2D _menu;

	[Export]
	private EnemyHive _enemyHive;

	[Export]
	private Label _labelHighscore;

	[Export]
	private Label _labelScore;

	[Export]
	private Label _LabelMenu;

	[Export]
	private Sprite2D[] _spriteLivesArray;

	[Export]
	private Barrier[] _barrierArray;

	[Export]
	private Player _player;

	[Export]
	private Timer _bonusEnemyTimer;

	private LevelData _levelData;

	private double _bonusEnemySpawnTimer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_menu.Visible = false;

		GameManager.LoadHighscoreData();
		UpdateHighScoreLabel();

		_player = GetNode<Player>("Player");
		_player.Connect(Player.SignalName.LiveChange, new Callable(this, MethodName._on_player_live_change));
		_player.Connect(Player.SignalName.Die, new Callable(this, MethodName._on_player_die));

		UpdateLivesIcon(Constants.DEFAULT_PLAYER_LIVES);

		LevelManager.LoadLevelData();
		EnemyManager.LoadEnemyData();

		_levelData = LevelManager.GetLevelData(1);

		SetupLevel();

		_enemyHive.SetupData(_levelData);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_text_clear_carets_and_selection"))
		{
			AudioManager.PlaySFX(_menu.IsVisibleInTree() ? AudioManager.Sfx.Cancel : AudioManager.Sfx.Menu, this);

			ToggleMenu();
		}
	}

	private void ToggleMenu(bool isGameOver = false)
	{
		_LabelMenu.Text = isGameOver ? "GAME OVER" : "GAME PAUSED";
		_menu.Visible = !GameManager.IsPaused;
		GameManager.IsPaused = _bonusEnemyTimer.Paused = _menu.IsVisibleInTree();
	}

	private void SetupLevel()
	{
		if (_levelData == null)
		{
			Debug.Print($"Error! Unable to get data for level 1!");
			return;
		}

		float x = 0f;
		float y = 100f;

		Dictionary<int, PackedScene> resDict = new Dictionary<int, PackedScene>();
		List<string> enemyStringList = _levelData	.EnemyList;

		foreach (string line in enemyStringList)
		{
			List<int> list = line.Split(",").Select(Int32.Parse)?.ToList();

			float diffY = 60f;
			float diffX = 700f / list.Count;

			int index = 0;
			foreach (int id in list)
			{
				bool success = resDict.TryGetValue(id, out PackedScene res);

				if (!success)
				{
					res = ResourceLoader.Load(Godot.ProjectSettings.GlobalizePath($"res://scenes/characters/Enemy{id}.tscn")) as PackedScene;
					resDict[id] = res;
				}

				if (res != null)
				{
					EnemyData enemyData = EnemyManager.GetEnemyData(id);
					if (enemyData != null)
					{
						Enemy enemy = res.Instantiate() as Enemy;
						enemy.Name = $"Enemy{id}-{index+1}";
						enemy.SetupData(enemyData);
						_enemyHive.AddChild(enemy);

						if (x == 0f)
						{
							x = enemy.GetWidth() + 15f;
						}

						enemy.Position = new Vector2(x + diffX * index, y);

						enemy.Connect(Enemy.SignalName.Die, new Callable(this, MethodName._on_enemy_die));

						enemy.StartAnimationIdle();
					}
				}

				index++;
			}

			index = 0;
			y += diffY;
		}

		_bonusEnemySpawnTimer = _levelData.BonusEnemySpawnTimeSec;
	}

	private void ResetLevel()
	{
		_enemyHive.Reset();

		foreach (Enemy child in _enemyHive.GetChildren())
		{
			child.QueueFree();
		}

		SetupLevel();

		foreach (Barrier barrier in _barrierArray)
		{
			barrier.Reset();
		}

		_player.Reset();
		UpdateLivesIcon (_player.Lives);

		Node2D bulletsNode = GetNode("Bullets") as Node2D;
		foreach (var child in bulletsNode.GetChildren())
		{
			child.QueueFree();
		}

		GameManager.Score = 0;
		UpdateScoreLabel();
	}

	private void _on_button_reset_pressed()
	{
		AudioManager.PlaySFX(AudioManager.Sfx.Confirm, this);

		ResetLevel();

		ToggleMenu();
	}

	private async void _on_button_quit_pressed()
	{
		AudioStreamPlayer asp = AudioManager.PlaySFX(AudioManager.Sfx.Confirm, this);

		await ToSignal(asp, "finished");

		GetTree().Quit();
	}

	private void _on_menu_visibility_changed()
	{
		bool _menuVisibleInTree = _menu.IsVisibleInTree();

		GetNode<Player>("Player").Pause(_menuVisibleInTree);

		foreach (Node child in _enemyHive.GetChildren())
		{
			if (child is Enemy enemy)
			{
				enemy.Pause(_menuVisibleInTree);
			}
		}

		GetNode<EnemyBonus>("EnemyBonus").Pause(_menuVisibleInTree);
	}

	private void _on_player_live_change(int currentLives)
	{
		UpdateLivesIcon(currentLives);
	}

	private void _on_player_die()
	{		
		GameManager.SetHighscore();

		ToggleMenu(true);

		GameManager.IsPaused = true;
	}

	private void _on_enemy_die(int points)
	{
		GameManager.Score += points;
		GameManager.SetHighscore();

		UpdateScoreLabel();
		UpdateHighScoreLabel();
	}

	private void _on_bonus_enemy_timer_timeout()
	{
		PackedScene res = ResourceLoader.Load(Godot.ProjectSettings.GlobalizePath($"res://scenes/characters/EnemyBonus.tscn")) as PackedScene;
		EnemyBonus enemyBonus = res.Instantiate() as EnemyBonus;
		enemyBonus.Name = "EnemyBonus";
		enemyBonus.Position = new Vector2(-50f, new Random().Next(200, 800));
		enemyBonus.SetupData(EnemyManager.GetEnemyData(99));
		AddChild(enemyBonus);

		enemyBonus.Connect(Enemy.SignalName.Die, new Callable(this, MethodName._on_enemy_die));
	}

	private void UpdateHighScoreLabel()
	{
		_labelHighscore.Text = $"Highscore\n{GameManager.Highscore}";
	}

	private void UpdateScoreLabel()
	{
		_labelScore.Text = $"Score\n{GameManager.Score}";
	}

	private void UpdateLivesIcon(int currentLives)
	{
		for (int i = 0; i < _spriteLivesArray.Length; i++)
		{
			Sprite2D sprite = _spriteLivesArray[i];
			if (sprite.Material is ShaderMaterial shaderMaterial)
			{
				shaderMaterial.SetShaderParameter("_isOn", i >= currentLives);
			}
		}
	}
}
