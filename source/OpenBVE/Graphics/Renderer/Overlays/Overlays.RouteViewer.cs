using System;
using System.Drawing;
using System.Globalization;
using OpenBveApi.Colors;

namespace OpenBve
{
	internal static partial class Renderer
	{
		private static void RenderRouteViewerOverlay()
		{
			string[][] Keys = new string[][] { new string[] { "F" }, new string[] { "N" }, new string[] { "E" }, new string[] { "C" }, new string[] { "M" }, new string[] { "I" } };
			RenderKeys(Screen.Width - 20, 4, 16, Keys);
			DrawString(Fonts.SmallFont, "Wireframe:", new Point(Screen.Width - 32, 4), TextAlignment.TopRight, Color128.White, true);
			DrawString(Fonts.SmallFont, "Normals:", new Point(Screen.Width - 32, 24), TextAlignment.TopRight, Color128.White, true);
			DrawString(Fonts.SmallFont, "Events:", new Point(Screen.Width - 32, 44), TextAlignment.TopRight, Color128.White, true);
			DrawString(Fonts.SmallFont, "CPU:", new Point(Screen.Width - 32, 64), TextAlignment.TopRight, Color128.White, true);
			DrawString(Fonts.SmallFont, "Mute:", new Point(Screen.Width - 32, 84), TextAlignment.TopRight, Color128.White, true);
			DrawString(Fonts.SmallFont, "Hide interface:", new Point(Screen.Width - 32, 104), TextAlignment.TopRight, Color128.White, true);
			//DrawString(Fonts.SmallFont, (RenderStatsOverlay ? "Hide" : "Show") + " renderer statistics", new Point(Screen.Width - 32, 124), TextAlignment.TopRight, Color128.White, true);
			Keys = new string[][] { new string[] { "F10" } };
			RenderKeys(Screen.Width - 32, 124, 30, Keys);
			Keys = new string[][] { new string[] { null, "W", null }, new string[] { "A", "S", "D" } };
			RenderKeys(4, Screen.Height - 40, 16, Keys);
			Keys = new string[][] { new string[] { null, "↑", null }, new string[] { "←", "↓", "→" } };
			RenderKeys(0 * Screen.Width - 48, Screen.Height - 40, 16, Keys);
			Keys = new string[][] { new string[] { "P↑" }, new string[] { "P↓" } };
			RenderKeys((int)(0.5 * Screen.Width + 32), Screen.Height - 40, 24, Keys);
			Keys = new string[][] { new string[] { null, "/", "*" }, new string[] { "7", "8", "9" }, new string[] { "4", "5", "6" }, new string[] { "1", "2", "3" }, new string[] { null, "0", "." } };
			RenderKeys(Screen.Width - 60, Screen.Height - 100, 16, Keys);
			
			if (Game.JumpToPositionEnabled)
			{
				DrawString(Fonts.SmallFont, "Jump to track position:", new Point(4, 80), TextAlignment.TopLeft, Color128.White, true);
				double distance;
				if (Double.TryParse(Game.JumpToPositionValue, out distance))
				{
					if (distance < Game.MinimumJumpToPositionValue - 100)
					{
						DrawString(Fonts.SmallFont, (Environment.TickCount % 1000 <= 500 ? Game.JumpToPositionValue + "_" : Game.JumpToPositionValue), new Point(4, 100), TextAlignment.TopLeft, Color128.Red, true);
					}
					else
					{
						DrawString(Fonts.SmallFont, (Environment.TickCount % 1000 <= 500 ? Game.JumpToPositionValue + "_" : Game.JumpToPositionValue), new Point(4, 100), TextAlignment.TopLeft, distance > TrackManager.CurrentTrack.Elements[TrackManager.CurrentTrack.Elements.Length - 1].StartingTrackPosition + 100
							? Color128.Red : Color128.Yellow, true);
					}

				}
			}
			
			// info
			double x = 0.5 * (double)Screen.Width - 256.0;
			CultureInfo Culture = CultureInfo.InvariantCulture;
			DrawString(Fonts.SmallFont, "Position: " + GetLengthString(World.CameraCurrentAlignment.TrackPosition) + " (X=" + GetLengthString(World.CameraCurrentAlignment.Position.X) + ", Y=" + GetLengthString(World.CameraCurrentAlignment.Position.Y) + "), Orientation: (Yaw=" + (World.CameraCurrentAlignment.Yaw * 57.2957795130824).ToString("0.00", Culture) + "°, Pitch=" + (World.CameraCurrentAlignment.Pitch * 57.2957795130824).ToString("0.00", Culture) + "°, Roll=" + (World.CameraCurrentAlignment.Roll * 57.2957795130824).ToString("0.00", Culture) + "°)", new Point((int)x, 4), TextAlignment.TopLeft, Color128.White, true);
			DrawString(Fonts.SmallFont, "Radius: " + GetLengthString(World.CameraTrackFollower.CurveRadius) + ", Cant: " + (1000.0 * World.CameraTrackFollower.CurveCant).ToString("0", Culture) + " mm, Adhesion=" + (100.0 * World.CameraTrackFollower.AdhesionMultiplier).ToString("0", Culture), new Point((int)x, 20), TextAlignment.TopLeft, Color128.White, true);
			/*
			if (Program.CurrentStation >= 0)
			{
				System.Text.StringBuilder t = new System.Text.StringBuilder();
				t.Append(Game.Stations[Program.CurrentStation].Name);
				if (Game.Stations[Program.CurrentStation].ArrivalTime >= 0.0)
				{
					t.Append(", Arrival: " + GetTime(Game.Stations[Program.CurrentStation].ArrivalTime));
				}
				if (Game.Stations[Program.CurrentStation].DepartureTime >= 0.0)
				{
					t.Append(", Departure: " + GetTime(Game.Stations[Program.CurrentStation].DepartureTime));
				}
				if (Game.Stations[Program.CurrentStation].OpenLeftDoors & Game.Stations[Program.CurrentStation].OpenRightDoors)
				{
					t.Append(", [L][R]");
				}
				else if (Game.Stations[Program.CurrentStation].OpenLeftDoors)
				{
					t.Append(", [L][-]");
				}
				else if (Game.Stations[Program.CurrentStation].OpenRightDoors)
				{
					t.Append(", [-][R]");
				}
				else
				{
					t.Append(", [-][-]");
				}
				switch (Game.Stations[Program.CurrentStation].StopMode)
				{
					case Game.StationStopMode.AllStop:
						t.Append(", Stop");
						break;
					case Game.StationStopMode.AllPass:
						t.Append(", Pass");
						break;
					case Game.StationStopMode.PlayerStop:
						t.Append(", Player stops - others pass");
						break;
					case Game.StationStopMode.PlayerPass:
						t.Append(", Player passes - others stop");
						break;
				}
				if (Game.Stations[Program.CurrentStation].StationType == Game.StationType.ChangeEnds)
				{
					t.Append(", Change ends");
				}
				t.Append(", Ratio=").Append((100.0 * Game.Stations[Program.CurrentStation].PassengerRatio).ToString("0", Culture)).Append("%");
				DrawString(Fonts.SmallFont, t.ToString(), new Point((int)x, 36), TextAlignment.TopLeft, Color128.White, true);
			}
			if (Interface.MessageCount == 1)
			{
				Keys = new string[][] { new string[] { "F9" } };
				RenderKeys(4, 72, 24, Keys);
				if (Interface.Messages[0].Type != Interface.MessageType.Information)
				{
					DrawString(Fonts.SmallFont, "Display the 1 error message recently generated.", new Point(32, 72), TextAlignment.TopLeft, Color128.Red, true);
				}
				else
				{
					//If all of our messages are information, then print the message text in grey
					DrawString(Fonts.SmallFont, "Display the 1 message recently generated.", new Point(32, 72), TextAlignment.TopLeft, Color128.White, true);
				}
			}
			else if (Interface.MessageCount > 1)
			{
				Keys = new string[][] { new string[] { "F9" } };
				RenderKeys(4, 72, 24, Keys);
				bool error = false;
				for (int i = 0; i < Interface.MessageCount; i++)
				{
					if (Interface.Messages[i].Type != Interface.MessageType.Information)
					{
						error = true;
					}

				}
				if (error)
				{
					DrawString(Fonts.SmallFont, "Display the " + Interface.MessageCount + " error messages recently generated.", new Point(32, 72), TextAlignment.TopLeft, Color128.Red, true);
				}
				else
				{
					DrawString(Fonts.SmallFont, "Display the " + Interface.MessageCount + " messages recently generated.", new Point(32, 72), TextAlignment.TopLeft, Color128.White, true);
				}
			}
			/*
			if (RenderStatsOverlay)
			{
				RenderKeys(4, Screen.Height - 126, 116, new string[][] { new string[] { "Renderer Statistics" } });
				DrawString(Fonts.SmallFont, "Total static objects: " + ObjectManager.ObjectsUsed, new Point(4, Screen.Height - 112), TextAlignment.TopLeft, Color128.White, true);
				DrawString(Fonts.SmallFont, "Total animated objects: " + ObjectManager.AnimatedWorldObjectsUsed, new Point(4, Screen.Height - 100), TextAlignment.TopLeft, Color128.White, true);
				DrawString(Fonts.SmallFont, "Current framerate: " + Game.InfoFrameRate.ToString("0.0", Culture) + "fps", new Point(4, Screen.Height - 88), TextAlignment.TopLeft, Color128.White, true);
				DrawString(Fonts.SmallFont, "Total opaque faces: " + Game.InfoStaticOpaqueFaceCount, new Point(4, Screen.Height - 76), TextAlignment.TopLeft, Color128.White, true);
				DrawString(Fonts.SmallFont, "Total alpha faces: " + (Renderer.AlphaListCount + Renderer.TransparentColorListCount), new Point(4, Screen.Height - 64), TextAlignment.TopLeft, Color128.White, true);
			}
			*/
		}

		/// <summary>Gets a formatted string for a length value</summary>
		/// <param name="Value">The value</param>
		/// <returns>The formatted string</returns>
		private static string GetLengthString(double Value)
		{
			System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.InvariantCulture;
			if (Game.RouteUnitOfLength.Length == 1 && Game.RouteUnitOfLength[0] == 1.0)
			{
				return Value.ToString("0.00", culture);
			}
			else
			{
				double[] values = new double[Game.RouteUnitOfLength.Length];
				for (int i = 0; i < Game.RouteUnitOfLength.Length - 1; i++)
				{
					values[i] = Math.Floor(Value / Game.RouteUnitOfLength[i]);
					Value -= values[i] * Game.RouteUnitOfLength[i];
				}
				values[Game.RouteUnitOfLength.Length - 1] = Value / Game.RouteUnitOfLength[Game.RouteUnitOfLength.Length - 1];
				System.Text.StringBuilder builder = new System.Text.StringBuilder();
				for (int i = 0; i < values.Length - 1; i++)
				{
					builder.Append(values[i].ToString(culture) + ":");
				}
				builder.Append(values[values.Length - 1].ToString("0.00", culture));
				return builder.ToString();
			}
		}
	}
}
