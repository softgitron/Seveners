using Godot;
using System;

public partial class SukellusInput : Node2D
{
	private int sukellustaso = 0;

	private CanvasItem pintavesi;
	private ProgressBar syvyysmittari;

	public override void _Ready(){
		pintavesi = GetNode<CanvasItem>("pintavesi");
		syvyysmittari = GetNode<ProgressBar>("CanvasLayer/Control2/ColorRect/ProgressBar");
	}
	public override void _Process(double delta){
		if (Input.IsActionPressed("ui_down")){
			sukellustaso++;
		} else if (Input.IsActionPressed("ui_up")){
			sukellustaso--;
		}
		if (sukellustaso > 10){
			pintavesi.Visible = false;
		} else {
			pintavesi.Visible = true;
		}
		syvyysmittari.Value = sukellustaso;
		GD.Print("Current value: ",sukellustaso);
	}
}
