using Godot;
using System;
using System.Runtime.CompilerServices;


public partial class main_menu : Control
{
	private const string gameSRC= "res://main.tscn";
	private CanvasItem controlForMainMenu;
	private CanvasItem controlForOptions;
	private CanvasItem controlExit;
	private Slider mainSlider, musicSlider, sfxSlider, speechSlider;
	private int master, music, sfx, speech;

	

	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		mainSlider = GetNode<Slider>("Options/VBoxContainer/Master");
		musicSlider = GetNode<Slider>("Options/VBoxContainer/Music");
		sfxSlider = GetNode<Slider>("Options/VBoxContainer/SFX");
		speechSlider = GetNode<Slider>("Options/VBoxContainer/Speech");

		master = AudioServer.GetBusIndex("Master");
		music = AudioServer.GetBusIndex("Music");
		sfx = AudioServer.GetBusIndex("SFX");
		speech = AudioServer.GetBusIndex("Speech");

		controlForMainMenu = GetNode<CanvasItem>("VBoxMain");
		controlForOptions = GetNode<CanvasItem>("Options");
		controlExit = GetNode<CanvasItem>("ExitControl");

		controlForOptions.Hide();
		controlExit.Hide();

		mainSlider.SetValueNoSignal(Mathf.DbToLinear(mainSlider.Value));
		musicSlider.SetValueNoSignal(Mathf.DbToLinear(musicSlider.Value));
		sfxSlider.SetValueNoSignal(Mathf.DbToLinear(sfxSlider.Value));
		speechSlider.SetValueNoSignal(Mathf.DbToLinear(speechSlider.Value));
	}

	private static void HideAndShow(CanvasItem toBeHidden, CanvasItem toBeShown)
	{
		toBeHidden.Hide();
		toBeShown.Show();
	}

	private void _on_start_pressed()
	{
		// Add scene change here!!
		GetTree().ChangeSceneToFile(gameSRC);
	}

	private void _on_options_pressed()
	{
		HideAndShow(controlForMainMenu, controlForOptions);
	}

	private void _on_quit_pressed()
	{
		HideAndShow(controlForMainMenu, controlExit);
	}

	private void _on_go_back_pressed()
	{
		HideAndShow(controlForOptions, controlForMainMenu);
	}

	private void _on_master_value_changed(float value) { AudioServer.SetBusVolumeDb(master, (float)Mathf.LinearToDb(value)); }
	private void _on_music_value_changed(float value) { AudioServer.SetBusVolumeDb(music, (float)Mathf.LinearToDb(value)); }
	private void _on_sfx_value_changed(float value) { AudioServer.SetBusVolumeDb(sfx, (float)Mathf.LinearToDb(value)); }
	private void _on_speech_value_changed(float value) { AudioServer.SetBusVolumeDb(speech, (float)Mathf.LinearToDb(value)); }

	private void _on_actual_exit_pressed() { 
		GD.Print("Actual exit");
		GetTree().Quit(); }
		//Modern amusement
}
