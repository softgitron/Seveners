using Godot;

public partial class Torpedo : CharacterBody2D
{
	public Vector2 pos;
	public float rotation;
	public float direction;
	float speed = 300;
	float lifetime = 10;
	public override void _Ready()
	{
		base._Ready();
		GlobalPosition = pos;
		GlobalRotation = rotation;
		NodeCollection.Instance.RegisterNode(this);
	}
	public void Initialize(Vector2 pos, float rotation)
	{
		GlobalRotation = rotation;
		GlobalPosition = pos;
	}
	public override void _PhysicsProcess(double delta)
	{
		var vel = new Vector2(0, -speed).Rotated(direction);
		var collision = MoveAndCollide(vel * (float)delta);
		if (collision != null)
		{
			if (collision.GetCollider() is Submarine)
			{
				var player = (Submarine)collision.GetCollider();
				player.TakeDamage(10);
			}
			if (collision.GetCollider() is AiEntity){
				var enemy = (AiEntity)collision.GetCollider();
				enemy.TakeDamage(50);
			}
			QueueFree();
		}
	}
	public override void _Process(double delta)
	{
		lifetime -= (float)delta;
		if (lifetime < 0)
		{
			QueueFree();
		}
	}

	public override void _Notification(int what)
	{
		if (what == NotificationExitTree) NodeCollection.Instance.UnregisterNode(this);
	}

}
