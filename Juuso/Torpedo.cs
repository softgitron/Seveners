using Godot;

public partial class Torpedo : CharacterBody2D
{
	public Vector2 pos;
	public float rotation;
	public float direction;
	float speed = 400;
	float lifetime = 4;
	PackedScene explosionScene = (PackedScene)GD.Load("res://Roni/Explosion.tscn");

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
				player.TakeDamage(15);
			}
			if (collision.GetCollider() is AiEntity)
			{
				var enemy = (AiEntity)collision.GetCollider();
				enemy.TakeDamage(50);
			}
			SpawnExplosion();
			QueueFree();
		}
	}
	public override void _Process(double delta)
	{
		lifetime -= (float)delta;
		if (lifetime < 0)
		{
			SpawnExplosion();
			QueueFree();
		}
	}

	public override void _Notification(int what)
	{
		if (what == NotificationExitTree) NodeCollection.Instance.UnregisterNode(this);
	}

	public void SpawnExplosion()
	{
		Node2D explosion = (Node2D)explosionScene.Instantiate();
		explosion.GlobalPosition = GlobalPosition;
		GetTree().Root.AddChild(explosion);
	}
}
