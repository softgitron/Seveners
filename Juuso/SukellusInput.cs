using Godot;
using System;

public partial class SukellusInput : Node2D
{
	private int depth = 0;
	
	private Node pintavesi;

	public override void _Ready(){
		pintavesi = GetNode<Node>("pintavesi");
}
	public override void _Process(float delta){
		if (Input.IsActionPressed("ui_down")){
			value++;
		} else if (Input.IsActionPressed("ui_up")){
			value--;
		}
		if (value > 10){
			pintavesi.Visible = false;
		} else {
			pintavesi.Visible = true;
		}
		GD.Print("Current value: ",value);
	}
}
