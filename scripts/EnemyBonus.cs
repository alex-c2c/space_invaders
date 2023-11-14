using System;
using System.Threading.Tasks;
using Godot;

public partial class EnemyBonus : Enemy
{
    [Export]
    private AudioStreamPlayer _audioBonus;

    private float _tmpAudioBonusTime;
	private Tween _tween;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

        _tmpAudioBonusTime = 0f;
		
		_tween  = CreateTween();
		_tween.TweenProperty(this, "position", new Vector2(800f, Position.Y), 5.0f);
		_tween.TweenCallback(Callable.From(QueueFree));
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

    public override void Pause(bool isPaused)
    {
        base.Pause(isPaused);

        if (isPaused)
        {
			_tween.Pause();

            if (_audioBonus.Playing)
            {
                _tmpAudioBonusTime = _audioBonus.GetPlaybackPosition();
                _audioBonus.Stop();
            }
        }
        else
        {
			_tween.Play();

            if (_tmpAudioBonusTime > 0f)
            {
                _audioBonus.Play(_tmpAudioBonusTime);
                _tmpAudioBonusTime = 0f;
            }
        } 
    }

    public override void _on_area_entered(Area2D area)
	{
		if (area is Bullet bullet)
		{
			if (bullet.BulletType == Bullet.Type.PLAYER)
			{
				_tween.Stop();

				GetHit();

				bullet.QueueFree();
			}
		}
		else if (area is Barrier barrier)
		{
			_tween.Stop();

			GetHit();

			barrier.GetHit();
		}
	}
}
