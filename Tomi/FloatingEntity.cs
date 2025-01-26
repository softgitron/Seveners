using Godot;
using System;
using System.Diagnostics;

public partial class EnemyControl : CharacterBody2D
{
	[Export]
	public float Speed = 200.0f;
	[Export]
	public int Health = 10;
	private AnimatedSprite2D animatedSprite2D;
	private Node2D player;
	private Node2D homeBase;
	private Node2D currentTarget;
	private float distanceToTarget;
	private bool isAttacking;
	private bool dead;
	private Random random = new Random();
	private NavigationAgent2D nav;
	private AudioStreamPlayer2D swingAudio;
	public override void _Ready()
	{
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		homeBase = GetNode<Node2D>("/root/Main Root/Home");
		nav = (NavigationAgent2D)GetNode("NavigationAgent2D");
		swingAudio = GetNode<AudioStreamPlayer2D>("SwingAudio");
		RandomizeSpeed();
		UpdateNavTarget();
	}
	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		// Targeting
		if (player != null)
		{
			currentTarget = player;
		}
		else if (homeBase != null)
		{
			currentTarget = homeBase;
		}
		else
		{
			currentTarget = null;
		}

		var directionToTarget = currentTarget.Position - Position;
		distanceToTarget = directionToTarget.Length();

		// Velocity
		if (currentTarget != null && !isAttacking)
		{
			if (distanceToTarget < 80)
			{
				Debug.Print("Attack");
				Velocity = Vector2.Zero;
				isAttacking = true;
			}
			else
			{
				var direction = nav.GetNextPathPosition() - GlobalPosition;
				direction = direction.Normalized();
				Velocity = direction.Normalized() * Speed;
			}
		}
		else
		{
			Velocity = Vector2.Zero;
		}
		// Animation
		if (Velocity.Length() > 0)
		{
			animatedSprite2D.Play("Run");
			if (Velocity.X > 0)
			{
				animatedSprite2D.FlipH = false;
			}
			else
			{
				animatedSprite2D.FlipH = true;
			}
		}
		else if (isAttacking)
		{
			animatedSprite2D.Play("Attack");
			if (!swingAudio.Playing) swingAudio.Play();
		}
		else
		{
			animatedSprite2D.Play("Idle");
		}

		MoveAndSlide();
	}

	private void _on_detection_body_entered(Node2D body)
	{
		player = body;
		UpdateNavTarget();
	}


	private void _on_detection_body_exited(Node2D body)
	{
		player = null;
		UpdateNavTarget();
	}
	//private void _on_attack_animation_finished()
	//{
	//    if (isAttacking)
	//    {
	//        isAttacking = false;
	//        RandomizeSpeed();
	//        if (distanceToTarget < 80)
	//        {
	//            if (player != null)
	//            {
	//                PlayerWizard wizard = (PlayerWizard)player;
	//                wizard.Health -= 5;
	//                Debug.Print("Player base health is " + wizard.Health);
	//            }
	//            else
	//            {
	//                HomeBase home = (HomeBase)homeBase;
	//                home.Health -= 5;
	//                Debug.Print("Home base health is " + home.Health);
	//            }
	//        }
	//        else
	//        {
	//            Debug.Print("missed");
	//        }
	//    }
	//}
	private void RandomizeSpeed()
	{
		Speed = 100 + (float)random.NextDouble() * 300;
	}

	private void _on_timer_timeout()
	{
		if (random.NextDouble() < 0.05)
			UpdateNavTarget();
	}

	private void UpdateNavTarget()
	{
		if (currentTarget != null)
			nav.TargetPosition = currentTarget.GlobalPosition;
	}
	//public void take_damage(int damage, bool firedByPlayer)
	//{
	//    Health -= damage;
	//    if (Health <= 0)
	//        if (firedByPlayer)
	//        {
	//            ActionService.ActionPerformed(ScoringService.ActionType.KillingAnEnemy);
	//        }
	//    QueueFree();
	//}
}
