using Godot;
using System;

public partial class Launcher : Node3D
{
	[Export] public Label LaunchPercentageLabel; // Assign your Text/Label node in the Inspector

	// Input action names from your Project Settings -> Input Map
	[Export] public string IncreasePowerAction = "MoveRight";    
	[Export] public string DecreasePowerAction = "MoveLeft";  
	[Export] public string LaunchButtonAction = "Action"; 

	// Hardware Limits (Tweak to fit your real machine coil)
	[Export] public int MinPulseMs = 20;  
	[Export] public int MaxPulseMs = 60;  
	[Export] public int PowerStepMs = 2;  

	private int _currentPowerMs;

	public override void _Ready()
	{
		_currentPowerMs = MinPulseMs;
		UpdateTextDisplay();
	}

	public override void _Process(double delta)
	{
		// 1. Listen for Power Adjustments
		if (Input.IsActionJustPressed(IncreasePowerAction))
		{
			_currentPowerMs = Mathf.Clamp(_currentPowerMs + PowerStepMs, MinPulseMs, MaxPulseMs);
			UpdateTextDisplay();
		}
		else if (Input.IsActionJustPressed(DecreasePowerAction))
		{
			_currentPowerMs = Mathf.Clamp(_currentPowerMs - PowerStepMs, MinPulseMs, MaxPulseMs);
			UpdateTextDisplay();
		}

		// 2. Listen for Launch Trigger
		if (Input.IsActionJustPressed(LaunchButtonAction))
		{
			FirePlunger(_currentPowerMs);
		}
	}
	
	// display code for launcher power
	private void UpdateTextDisplay()
	{
		if (LaunchPercentageLabel != null)
		{
			// Calculate percentage: (Current - Min) / (Max - Min) * 100
			float range = MaxPulseMs - MinPulseMs;
			float currentOffset = _currentPowerMs - MinPulseMs;
			
			int percentage = (range > 0) ? (int)Mathf.Round((currentOffset / range) * 100f) : 0;

			// Update the display text
			LaunchPercentageLabel.AddThemeColorOverride("font_color", Colors.White);
			LaunchPercentageLabel.Text = $"LAUNCH POWER: {percentage}%";
		}
	}

	// firesw plunger
	private void FirePlunger(int pulseMs)
	{
		var eventParams = new Godot.Collections.Dictionary();
		eventParams.Add("pulse_ms", pulseMs);

		var mpf = GetNode("/root/Mpf");
		var bcp = mpf.Get("bcp").AsGodotObject();


		if (bcp != null)
		{
			bcp.Call("send_event_with_args", "godot_launch_ball", eventParams);
			GD.Print($"Ball launched over BCP at: {pulseMs}ms");
		}
		else
		{
			GD.PrintErr("MPF BCP bridge connection missing.");
		}
	}
}
