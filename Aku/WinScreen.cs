using Godot;
using System;

public partial class WinScreen : Node2D
{
    private const string gameSRC= "res://Aku_MainMenu/main_menu.tscn";
    	private void _on_actual_exit_pressed() 
        {
		    //GetTree().Quit(); 
            GetTree().ChangeSceneToFile(gameSRC);
        }
}
