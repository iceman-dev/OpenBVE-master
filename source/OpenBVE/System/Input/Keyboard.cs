using System;
using System.Globalization;
using OpenBveApi.Math;
using OpenTK.Input;

namespace OpenBve
{
	internal static partial class MainLoop
	{
		/// <summary>Called when a KeyDown event is generated</summary>
		internal static void keyDownEvent(object sender, KeyboardKeyEventArgs e)
		{
			if (Loading.Complete == true && e.Key == OpenTK.Input.Key.F4 && e.Alt == true)
			{
				// Catch standard ALT + F4 quit and push confirmation prompt
				Game.Menu.PushMenu(Menu.MenuType.Quit);
				return;
			}
			BlockKeyRepeat = true;
			//Check for modifiers
			if (e.Shift) CurrentKeyboardModifier |= Interface.KeyboardModifier.Shift;
			if (e.Control) CurrentKeyboardModifier |= Interface.KeyboardModifier.Ctrl;
			if (e.Alt) CurrentKeyboardModifier |= Interface.KeyboardModifier.Alt;
			if (Game.CurrentInterface == Game.InterfaceType.Menu && Game.Menu.IsCustomizingControl())
			{
				Game.Menu.SetControlKbdCustomData(e.Key, CurrentKeyboardModifier);
				return;
			}
			//Traverse the controls array
			for (int i = 0; i < Interface.CurrentControls.Length; i++)
			{
				//If we're using keyboard for this input
				if (Interface.CurrentControls[i].Method == Interface.ControlMethod.Keyboard)
				{
					//Compare the current and previous keyboard states
					//Only process if they are different
					if (!Enum.IsDefined(typeof(Key), Interface.CurrentControls[i].Key)) continue;
					if (e.Key == Interface.CurrentControls[i].Key & Interface.CurrentControls[i].Modifier == CurrentKeyboardModifier)
					{

						Interface.CurrentControls[i].AnalogState = 1.0;
						Interface.CurrentControls[i].DigitalState = Interface.DigitalControlState.Pressed;
						//Key repeats should not be added in non-game interface modes, unless they are Menu Up/ Menu Down commands
						if (Game.CurrentInterface == Game.InterfaceType.Normal || Interface.CurrentControls[i].Command == Interface.Command.MenuUp || Interface.CurrentControls[i].Command == Interface.Command.MenuDown)
						{
							if (Interface.CurrentControls[i].Command == Interface.Command.CameraInterior |
								Interface.CurrentControls[i].Command == Interface.Command.CameraExterior |
								Interface.CurrentControls[i].Command == Interface.Command.CameraFlyBy |
								Interface.CurrentControls[i].Command == Interface.Command.CameraTrack)
							{
								//HACK: We don't want to bounce between camera modes when holding down the mode switch key
								continue;
							}
							AddControlRepeat(i);
						}
					}
				}
			}
			BlockKeyRepeat = false;
			//Remember to reset the keyboard modifier after we're done, else it repeats.....
			CurrentKeyboardModifier = Interface.KeyboardModifier.None;
			if (Interface.CurrentOptions.GameMode == Interface.GameMode.Developer && Game.CurrentInterface == Game.InterfaceType.Normal)
			{
				double speedModified = (Game.ShiftPressed ? 2.0 : 1.0) * (Game.ControlPressed ? 4.0 : 1.0) * (Game.AltPressed ? 8.0 : 1.0);
				//Handle the developer mode keys separately
				switch (e.Key)
				{
					case Key.ShiftLeft:
					case Key.ShiftRight:
						Game.ShiftPressed = true;
						break;
					case Key.ControlLeft:
					case Key.ControlRight:
						Game.ControlPressed = true;
						break;
					case Key.LAlt:
					case Key.RAlt:
						Game.AltPressed = true;
						break;
					case Key.A:
					case Key.Keypad4:
						World.CameraAlignmentDirection.Position.X = -World.CameraExteriorTopSpeed * speedModified;
						break;
					case Key.D:
					case Key.Keypad6:
						World.CameraAlignmentDirection.Position.X = World.CameraExteriorTopSpeed * speedModified;
						break;
					case Key.Keypad2:
						World.CameraAlignmentDirection.Position.Y = -World.CameraExteriorTopSpeed * speedModified;
						break;
					case Key.Keypad8:
						World.CameraAlignmentDirection.Position.Y = World.CameraExteriorTopSpeed * speedModified;
						break;
					case Key.W:
					case Key.Keypad9:
						World.CameraAlignmentDirection.TrackPosition = World.CameraExteriorTopSpeed * speedModified;
						break;
					case Key.S:
					case Key.Keypad3:
						World.CameraAlignmentDirection.TrackPosition = -World.CameraExteriorTopSpeed * speedModified;
						break;
					case Key.Left:
						World.CameraAlignmentDirection.Yaw = -World.CameraExteriorTopAngularSpeed * speedModified;
						break;
					case Key.Right:
						World.CameraAlignmentDirection.Yaw = World.CameraExteriorTopAngularSpeed * speedModified;
						break;
					case Key.Up:
						World.CameraAlignmentDirection.Pitch = World.CameraExteriorTopAngularSpeed * speedModified;
						break;
					case Key.Down:
						World.CameraAlignmentDirection.Pitch = -World.CameraExteriorTopAngularSpeed * speedModified;
						break;
					case Key.KeypadDivide:
						World.CameraAlignmentDirection.Roll = -World.CameraExteriorTopAngularSpeed * speedModified;
						break;
					case Key.KeypadMultiply:
						World.CameraAlignmentDirection.Roll = World.CameraExteriorTopAngularSpeed * speedModified;
						break;
					case Key.Keypad0:
						World.CameraAlignmentDirection.Zoom = World.CameraZoomTopSpeed * speedModified;
						break;
					case Key.KeypadPeriod:
						World.CameraAlignmentDirection.Zoom = -World.CameraZoomTopSpeed * speedModified;
						break;
					case Key.Keypad1:
						Game.ApplyPointOfInterest(-1, true);
						break;
					case Key.Keypad7:
						Game.ApplyPointOfInterest(1, true);
						break;
					case Key.PageUp:
						/*JumpToStation(1);
						CpuReducedMode = false;*/
						break;
					case Key.PageDown:
						/*JumpToStation(-1);
						CpuReducedMode = false;*/
						break;
					case Key.Keypad5:
						World.CameraCurrentAlignment.Yaw = 0.0;
						World.CameraCurrentAlignment.Pitch = 0.0;
						World.CameraCurrentAlignment.Roll = 0.0;
						World.CameraCurrentAlignment.Position = new Vector3(0.0, 2.5, 0.0);
						World.CameraCurrentAlignment.Zoom = 0.0;
						World.CameraAlignmentDirection = new World.CameraAlignment();
						World.CameraAlignmentSpeed = new World.CameraAlignment();
						World.VerticalViewingAngle = World.OriginalVerticalViewingAngle;
						UpdateViewport(ViewPortChangeMode.NoChange);
						World.UpdateAbsoluteCamera(0.0);
						World.UpdateViewingDistances();
						break;
					case Key.Minus:
					case Key.KeypadMinus:
						if (!Game.JumpToPositionEnabled)
						{
							Game.JumpToPositionEnabled = true;
							Game.JumpToPositionValue = "-";
						}
						break;
					case Key.Number0:
					case Key.Number1:
					case Key.Number2:
					case Key.Number3:
					case Key.Number4:
					case Key.Number5:
					case Key.Number6:
					case Key.Number7:
					case Key.Number8:
					case Key.Number9:
						if (!Game.JumpToPositionEnabled)
						{
							Game.JumpToPositionEnabled = true;
							Game.JumpToPositionValue = string.Empty;
						}
						Game.JumpToPositionValue += char.ConvertFromUtf32(48 + e.Key - Key.Number0);
						break;
					case Key.Period:
						if (!Game.JumpToPositionEnabled)
						{
							Game.JumpToPositionEnabled = true;
							Game.JumpToPositionValue = "0.";
						}
						else if (Game.JumpToPositionValue.IndexOf('.') == -1)
						{
							Game.JumpToPositionValue += ".";
						}
						break;
					case Key.BackSpace:
						if (Game.JumpToPositionEnabled && Game.JumpToPositionValue.Length != 0)
						{
							Game.JumpToPositionValue = Game.JumpToPositionValue.Substring(0, Game.JumpToPositionValue.Length - 1);
						}
						break;
					case Key.Enter:
						if (Game.JumpToPositionEnabled)
						{
							if (Game.JumpToPositionValue.Length != 0)
							{
								int direction;
								if (Game.JumpToPositionValue[0] == '-')
								{
									Game.JumpToPositionValue = Game.JumpToPositionValue.Substring(1);
									direction = -1;
								}
								else if (Game.JumpToPositionValue[0] == '+')
								{
									Game.JumpToPositionValue = Game.JumpToPositionValue.Substring(1);
									direction = 1;
								}
								else
								{
									direction = 0;
								}
								double value;
								if (double.TryParse(Game.JumpToPositionValue, NumberStyles.Float, CultureInfo.InvariantCulture,
									out value))
									if (value < TrackManager.CurrentTrack.Elements[TrackManager.CurrentTrack.Elements.Length - 1].StartingTrackPosition + 100 && value > Game.MinimumJumpToPositionValue - 100)
									{
										if (direction != 0)
										{
											value = World.CameraTrackFollower.TrackPosition + (double)direction * value;
										}
										TrackManager.UpdateTrackFollower(ref World.CameraTrackFollower, value, true, false);
										World.CameraCurrentAlignment.TrackPosition = value;
										World.UpdateAbsoluteCamera(0.0);
										World.UpdateViewingDistances();
									}
							}
						}
						Game.JumpToPositionEnabled = false;
						break;
					case Key.Escape:
						Game.JumpToPositionEnabled = false;
						break;
				}
			}
		}

		/// <summary>Called when a KeyUp event is generated</summary>
		internal static void keyUpEvent(object sender, KeyboardKeyEventArgs e)
		{
			if (Game.PreviousInterface == Game.InterfaceType.Menu & Game.CurrentInterface == Game.InterfaceType.Normal)
			{
				//Block the first keyup event after the menu has been closed, as this may produce unwanted effects
				//if the menu select key is also mapped in-game
				Game.PreviousInterface = Game.InterfaceType.Normal;
				return;
			}
			//We don't need to check for modifiers on key up
			BlockKeyRepeat = true;
			//Traverse the controls array
			for (int i = 0; i < Interface.CurrentControls.Length; i++)
			{
				//If we're using keyboard for this input
				if (Interface.CurrentControls[i].Method == Interface.ControlMethod.Keyboard)
				{
					//Compare the current and previous keyboard states
					//Only process if they are different
					if (!Enum.IsDefined(typeof(Key), Interface.CurrentControls[i].Key)) continue;
					if (e.Key == Interface.CurrentControls[i].Key & Interface.CurrentControls[i].AnalogState == 1.0 & Interface.CurrentControls[i].DigitalState > Interface.DigitalControlState.Released)
					{
						Interface.CurrentControls[i].AnalogState = 0.0;
						Interface.CurrentControls[i].DigitalState = Interface.DigitalControlState.Released;
						RemoveControlRepeat(i);
					}
				}
			}
			BlockKeyRepeat = false;
		}
	}
}
