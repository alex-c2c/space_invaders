using Godot;
using System.Diagnostics;
using System.Threading.Tasks;

public partial class Character : Area2D
{
	[Export]
	protected AudioStreamPlayer _audioFire;

	[Export]
	protected AudioStreamPlayer _audioExplode;

	[Export]
	protected AnimationPlayer _animationPlayer;

	private const int FIRE_RESET_TIME_MS = 500;
    private float _tmpAudioFireTime = 0f;
    private float _tmpAudioExplodeTime = 0f;
    protected Vector2 _initialPosition;
	protected Vector2 _pixelSize;
	protected PackedScene _bulletRes;
	protected Node2D _bulletsNode;
    protected Sprite2D _sprite;

	public bool CanFireBullet
	{
		get;
		set;
	} = true;

    public bool CanPlay
    {
        get;
        set;
    } = true;

    public int Lives
    {
        get;
        set;
    }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        _initialPosition = this.Position;

		_sprite = GetNode("SpriteBody") as Sprite2D;

		_pixelSize = new Vector2(_sprite.Texture.GetWidth() / Constants.SPRITESHEET_H_FRAME * Scale.X, _sprite.Texture.GetHeight() / Constants.SPRITESHEET_V_FRAME * Scale.Y);

        ZeroSpritePosition();
	}

    public override void _Process(double delta)
    {
        if (GameManager.IsPaused)
        {
            return;
        }
    }

    virtual public void FireBullet()
	{

	}

    virtual public void GetHit()
    {
        
    }

    public virtual void Pause(bool isPaused)
    {
        if (isPaused)
        {
            _animationPlayer.Pause();

            if (_audioExplode.Playing)
            {
                _tmpAudioExplodeTime = _audioExplode.GetPlaybackPosition();
                _audioExplode.Stop();
            }

            if (_audioFire.Playing)
            {
                _tmpAudioFireTime = _audioFire.GetPlaybackPosition();
                _audioFire.Stop();
            }
        }
        else
        {
            _animationPlayer.Play();

            if (_tmpAudioExplodeTime > 0f)
            {
                _audioExplode.Play(_tmpAudioExplodeTime);
                _tmpAudioExplodeTime = 0f;
            }

            if (_tmpAudioFireTime > 0f)
            {
                _audioFire.Play(_tmpAudioFireTime);
                _tmpAudioFireTime = 0f;
            }
        }
    }

	protected async void ResetBulletTimer()
	{
		CanFireBullet = false;

		await Task.Delay(FIRE_RESET_TIME_MS);

		CanFireBullet = true;
	}

    public virtual void Reset()
    {
        CanPlay = true;
        Visible = true;
    }

    public float GetWidth()
    {
        return _pixelSize.X;
    }

    public void ZeroSpritePosition()
    {
        _sprite.Position = Vector2.Zero;
    }

    public virtual void _on_area_entered(Area2D area)
	{
        
	}
}
