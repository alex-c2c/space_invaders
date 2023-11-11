using Godot;
using System;
using System.Diagnostics;

public partial class Main : Node2D
{
	[Export]
	private Node2D _menu;

	[Export]
	private Node2D _enemyHive;

	[Export]
	private Label _labelHighscore;

	[Export]
	private Label _labelScore;

	[Export]
	private Sprite2D[] _spriteLivesArray;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_menu.Visible = false;

		Player player = GetNode<Player>("Player");
		player.Connect(Player.SignalName.LiveChange, new Callable(this, MethodName._on_player_live_change));
		player.Connect(Player.SignalName.Die, new Callable(this, MethodName._on_player_die));
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

	private void ToggleMenu()
	{
		_menu.Visible = !GameManager.IsPaused;
		GameManager.IsPaused = _menu.IsVisibleInTree();
	}

	private void SetupLevel()
	{

	}

	private void ResetLevel()
	{

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
		GetNode<Player>("Player").Pause(_menu.IsVisibleInTree());

		foreach (Node child in _enemyHive.GetChildren())
		{
			if (child is Enemy enemy)
			{
				enemy.Pause(_menu.IsVisibleInTree());
			}
		}
	}

	private void _on_player_live_change(int currentLives)
	{
		Debug.Print($"live change: {currentLives}");
	}

	private void _on_player_die()
	{
		Debug.Print($"Player die");
	}
}
