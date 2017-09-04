using System;
using OpenBveApi.Colors;
using OpenTK.Graphics.OpenGL;

namespace OpenBve
{
	internal static partial class Renderer
	{
		/// <summary>Renders the debug (F10) overlay</summary>
		private static void RenderDebugOverlays()
		{
			System.Globalization.CultureInfo Culture = System.Globalization.CultureInfo.InvariantCulture;
			// debug
			GL.Color4(0.5, 0.5, 0.5, 0.5);
			RenderOverlaySolid(0.0f, 0.0f, (double)Screen.Width, (double)Screen.Height);
			// actual handles
			{
				string t = "actual: " + (TrainManager.PlayerTrain.Specs.CurrentReverser.Actual == -1 ? "B" : TrainManager.PlayerTrain.Specs.CurrentReverser.Actual == 1 ? "F" : "N");
				if (TrainManager.PlayerTrain.Specs.SingleHandle)
				{
					t += " - " + (TrainManager.PlayerTrain.Specs.CurrentEmergencyBrake.Actual ? "EMG" : TrainManager.PlayerTrain.Specs.CurrentBrakeNotch.Actual != 0 ? "B" + TrainManager.PlayerTrain.Specs.CurrentBrakeNotch.Actual.ToString(Culture) : TrainManager.PlayerTrain.Specs.CurrentHoldBrake.Actual ? "HLD" : TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Actual != 0 ? "P" + TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Actual.ToString(Culture) : "N");
				}
				else if (TrainManager.PlayerTrain.Cars[TrainManager.PlayerTrain.DriverCar].Specs.BrakeType == TrainManager.CarBrakeType.AutomaticAirBrake)
				{
					t += " - " + (TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Actual != 0 ? "P" + TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Actual.ToString(Culture) : "N");
					t += " - " + (TrainManager.PlayerTrain.Specs.CurrentEmergencyBrake.Actual ? "EMG" : TrainManager.PlayerTrain.Specs.AirBrake.Handle.Actual == TrainManager.AirBrakeHandleState.Service ? "SRV" : TrainManager.PlayerTrain.Specs.AirBrake.Handle.Actual == TrainManager.AirBrakeHandleState.Lap ? "LAP" : "REL");
				}
				else
				{
					t += " - " + (TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Actual != 0 ? "P" + TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Actual.ToString(Culture) : "N");
					t += " - " + (TrainManager.PlayerTrain.Specs.CurrentEmergencyBrake.Actual ? "EMG" : TrainManager.PlayerTrain.Specs.CurrentBrakeNotch.Actual != 0 ? "B" + TrainManager.PlayerTrain.Specs.CurrentBrakeNotch.Actual.ToString(Culture) : TrainManager.PlayerTrain.Specs.CurrentHoldBrake.Actual ? "HLD" : "N");
				}
				DrawString(Fonts.SmallFont, t, new System.Drawing.Point(2, Screen.Height - 46), TextAlignment.TopLeft, Color128.White, true);
			}
			// safety handles
			{
				string t = "safety: " + (TrainManager.PlayerTrain.Specs.CurrentReverser.Actual == -1 ? "B" : TrainManager.PlayerTrain.Specs.CurrentReverser.Actual == 1 ? "F" : "N");
				if (TrainManager.PlayerTrain.Specs.SingleHandle)
				{
					t += " - " + (TrainManager.PlayerTrain.Specs.CurrentEmergencyBrake.Safety ? "EMG" : TrainManager.PlayerTrain.Specs.CurrentBrakeNotch.Safety != 0 ? "B" + TrainManager.PlayerTrain.Specs.CurrentBrakeNotch.Safety.ToString(Culture) : TrainManager.PlayerTrain.Specs.CurrentHoldBrake.Actual ? "HLD" : TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Safety != 0 ? "P" + TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Safety.ToString(Culture) : "N");
				}
				else if (TrainManager.PlayerTrain.Cars[TrainManager.PlayerTrain.DriverCar].Specs.BrakeType == TrainManager.CarBrakeType.AutomaticAirBrake)
				{
					t += " - " + (TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Safety != 0 ? "P" + TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Safety.ToString(Culture) : "N");
					t += " - " + (TrainManager.PlayerTrain.Specs.CurrentEmergencyBrake.Safety ? "EMG" : TrainManager.PlayerTrain.Specs.AirBrake.Handle.Safety == TrainManager.AirBrakeHandleState.Service ? "SRV" : TrainManager.PlayerTrain.Specs.AirBrake.Handle.Safety == TrainManager.AirBrakeHandleState.Lap ? "LAP" : "REL");
				}
				else
				{
					t += " - " + (TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Safety != 0 ? "P" + TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Safety.ToString(Culture) : "N");
					t += " - " + (TrainManager.PlayerTrain.Specs.CurrentEmergencyBrake.Safety ? "EMG" : TrainManager.PlayerTrain.Specs.CurrentBrakeNotch.Safety != 0 ? "B" + TrainManager.PlayerTrain.Specs.CurrentBrakeNotch.Safety.ToString(Culture) : TrainManager.PlayerTrain.Specs.CurrentHoldBrake.Actual ? "HLD" : "N");
				}
				DrawString(Fonts.SmallFont, t, new System.Drawing.Point(2, Screen.Height - 32), TextAlignment.TopLeft, Color128.White, true);
			}
			// driver handles
			{
				string t = "driver: " + (TrainManager.PlayerTrain.Specs.CurrentReverser.Driver == -1 ? "B" : TrainManager.PlayerTrain.Specs.CurrentReverser.Driver == 1 ? "F" : "N");
				if (TrainManager.PlayerTrain.Specs.SingleHandle)
				{
					t += " - " + (TrainManager.PlayerTrain.Specs.CurrentEmergencyBrake.Driver ? "EMG" : TrainManager.PlayerTrain.Specs.CurrentBrakeNotch.Driver != 0 ? "B" + TrainManager.PlayerTrain.Specs.CurrentBrakeNotch.Driver.ToString(Culture) : TrainManager.PlayerTrain.Specs.CurrentHoldBrake.Driver ? "HLD" : TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Driver != 0 ? "P" + TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Driver.ToString(Culture) : "N");
				}
				else if (TrainManager.PlayerTrain.Cars[TrainManager.PlayerTrain.DriverCar].Specs.BrakeType == TrainManager.CarBrakeType.AutomaticAirBrake)
				{
					t += " - " + (TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Driver != 0 ? "P" + TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Driver.ToString(Culture) : "N");
					t += " - " + (TrainManager.PlayerTrain.Specs.CurrentEmergencyBrake.Driver ? "EMG" : TrainManager.PlayerTrain.Specs.AirBrake.Handle.Driver == TrainManager.AirBrakeHandleState.Service ? "SRV" : TrainManager.PlayerTrain.Specs.AirBrake.Handle.Driver == TrainManager.AirBrakeHandleState.Lap ? "LAP" : "REL");
				}
				else
				{
					t += " - " + (TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Driver != 0 ? "P" + TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Driver.ToString(Culture) : "N");
					t += " - " + (TrainManager.PlayerTrain.Specs.CurrentEmergencyBrake.Driver ? "EMG" : TrainManager.PlayerTrain.Specs.CurrentBrakeNotch.Driver != 0 ? "B" + TrainManager.PlayerTrain.Specs.CurrentBrakeNotch.Driver.ToString(Culture) : TrainManager.PlayerTrain.Specs.CurrentHoldBrake.Driver ? "HLD" : "N");
				}
				DrawString(Fonts.SmallFont, t, new System.Drawing.Point(2, Screen.Height - 18), TextAlignment.TopLeft, Color128.White, true);
			}
			// debug information
			int texturesLoaded = Textures.GetNumberOfLoadedTextures();
			int texturesRegistered = Textures.GetNumberOfRegisteredTextures();
			int soundBuffersRegistered = Sounds.GetNumberOfLoadedBuffers();
			int soundBuffersLoaded = Sounds.GetNumberOfLoadedBuffers();
			int soundSourcesRegistered = Sounds.GetNumberOfRegisteredSources();
			int soundSourcesPlaying = Sounds.GetNumberOfPlayingSources();
			int car = 0;
			for (int i = 0; i < TrainManager.PlayerTrain.Cars.Length; i++)
			{
				if (TrainManager.PlayerTrain.Cars[i].Specs.IsMotorCar)
				{
					car = i;
					break;
				}
			}
			double mass = 0.0;
			for (int i = 0; i < TrainManager.PlayerTrain.Cars.Length; i++)
			{
				mass += TrainManager.PlayerTrain.Cars[i].Specs.MassCurrent;
			}
			string[] Lines = new string[] {
				"=system",
				"fps: " + Game.InfoFrameRate.ToString("0.0", Culture) + (MainLoop.LimitFramerate ? " (low cpu)" : ""),
				"score: " + Game.CurrentScore.Value.ToString(Culture),
				"",
				"=train",
				"speed: " + (Math.Abs(TrainManager.PlayerTrain.Specs.CurrentAverageSpeed) * 3.6).ToString("0.00", Culture) + " km/h",
				"power (car " + car.ToString(Culture) +  "): " + (TrainManager.PlayerTrain.Cars[car].Specs.CurrentAccelerationOutput < 0.0 ? TrainManager.PlayerTrain.Cars[car].Specs.CurrentAccelerationOutput * (double)Math.Sign(TrainManager.PlayerTrain.Cars[car].Specs.CurrentSpeed) : TrainManager.PlayerTrain.Cars[car].Specs.CurrentAccelerationOutput * (double)TrainManager.PlayerTrain.Specs.CurrentReverser.Actual).ToString("0.0000", Culture) + " m/s²",
				"acceleration: " + TrainManager.PlayerTrain.Specs.CurrentAverageAcceleration.ToString("0.0000", Culture) + " m/s²",
				"position: " + (TrainManager.PlayerTrain.Cars[0].FrontAxle.Follower.TrackPosition - TrainManager.PlayerTrain.Cars[0].FrontAxlePosition + 0.5 * TrainManager.PlayerTrain.Cars[0].Length).ToString("0.00", Culture) + " m",
				"elevation: " + (Game.RouteInitialElevation + TrainManager.PlayerTrain.Cars[TrainManager.PlayerTrain.DriverCar].FrontAxle.Follower.WorldPosition.Y).ToString("0.00", Culture) + " m",
				"temperature: " + (TrainManager.PlayerTrain.Specs.CurrentAirTemperature - 273.15).ToString("0.00", Culture) + " °C",
				"air pressure: " + (0.001 * TrainManager.PlayerTrain.Specs.CurrentAirPressure).ToString("0.00", Culture) + " kPa",
				"air density: " + TrainManager.PlayerTrain.Specs.CurrentAirDensity.ToString("0.0000", Culture) + " kg/m³",
				"speed of sound: " + (Game.GetSpeedOfSound(TrainManager.PlayerTrain.Specs.CurrentAirDensity) * 3.6).ToString("0.00", Culture) + " km/h",
				"passenger ratio: " + TrainManager.PlayerTrain.Passengers.PassengerRatio.ToString("0.00"),
				"total mass: " + mass.ToString("0.00", Culture) + " kg",
				"",
				"=route",
				"track limit: " + (TrainManager.PlayerTrain.CurrentRouteLimit == double.PositiveInfinity ? "unlimited" : ((TrainManager.PlayerTrain.CurrentRouteLimit * 3.6).ToString("0.0", Culture) + " km/h")),
				"signal limit: " + (TrainManager.PlayerTrain.CurrentSectionLimit == double.PositiveInfinity ? "unlimited" : ((TrainManager.PlayerTrain.CurrentSectionLimit * 3.6).ToString("0.0", Culture) + " km/h")),
				"total static objects: " + ObjectManager.ObjectsUsed.ToString(Culture),
				"total static GL_TRIANGLES: " + Game.InfoTotalTriangles.ToString(Culture),
				"total static GL_TRIANGLE_STRIP: " + Game.InfoTotalTriangleStrip.ToString(Culture),
				"total static GL_QUADS: " + Game.InfoTotalQuads.ToString(Culture),
				"total static GL_QUAD_STRIP: " + Game.InfoTotalQuadStrip.ToString(Culture),
				"total static GL_POLYGON: " + Game.InfoTotalPolygon.ToString(Culture),
				"total animated objects: " + ObjectManager.AnimatedWorldObjectsUsed.ToString(Culture),
				"",
				"=renderer",
				"static opaque faces: " + Game.InfoStaticOpaqueFaceCount.ToString(Culture),
				"dynamic opaque faces: " + DynamicOpaque.FaceCount.ToString(Culture),
				"dynamic alpha faces: " + DynamicAlpha.FaceCount.ToString(Culture),
				"overlay opaque faces: " + OverlayOpaque.FaceCount.ToString(Culture),
				"overlay alpha faces: " + OverlayAlpha.FaceCount.ToString(Culture),
				"textures loaded: " + texturesLoaded.ToString(Culture) + " / " + texturesRegistered.ToString(Culture),
				"",
				"=camera",
				"position: " + World.CameraTrackFollower.TrackPosition.ToString("0.00", Culture) + " m",
				"curve radius: " + World.CameraTrackFollower.CurveRadius.ToString("0.00", Culture) + " m",
				"curve cant: " + (1000.0 * Math.Abs(World.CameraTrackFollower.CurveCant)).ToString("0.00", Culture) + " mm" + (World.CameraTrackFollower.CurveCant < 0.0 ? " (left)" : World.CameraTrackFollower.CurveCant > 0.0 ? " (right)" : ""),
				"",
				"=sound",
				"sound buffers: " + soundBuffersLoaded.ToString(Culture) + " / " + soundBuffersRegistered.ToString(Culture),
				"sound sources: " + soundSourcesPlaying.ToString(Culture) + " / " + soundSourcesRegistered.ToString(Culture),
				(Interface.CurrentOptions.SoundModel == Sounds.SoundModels.Inverse ? "log clamp factor: " + Sounds.LogClampFactor.ToString("0.00") : "outer radius factor: " + Sounds.OuterRadiusFactor.ToString("0.00", Culture)),
				"",
				"=debug",
				"train plugin status: " + (TrainManager.PlayerTrain.Plugin != null ? (TrainManager.PlayerTrain.Plugin.PluginValid ? "ok" : "error") : "n/a"),
				"train plugin message: " + (TrainManager.PlayerTrain.Plugin != null ? (TrainManager.PlayerTrain.Plugin.PluginMessage ?? "n/a") : "n/a"),
				Game.InfoDebugString ?? ""
			};
			double x = 4.0;
			double y = 4.0;
			for (int i = 0; i < Lines.Length; i++)
			{
				if (Lines[i].Length != 0)
				{
					if (Lines[i][0] == '=')
					{
						string text = Lines[i].Substring(1);
						System.Drawing.Size size = MeasureString(Fonts.SmallFont, text);
						GL.Color4(0.35f, 0.65f, 0.90f, 0.8f);
						RenderOverlaySolid(x, y, x + size.Width + 6.0f, y + size.Height + 2.0f);
						DrawString(Fonts.SmallFont, text, new System.Drawing.Point((int)x + 3, (int)y), TextAlignment.TopLeft, Color128.White);
					}
					else
					{
						DrawString(Fonts.SmallFont, Lines[i], new System.Drawing.Point((int)x, (int)y), TextAlignment.TopLeft, Color128.White, true);
					}
					y += 14.0;
				}
				else if (y >= (double)Screen.Height - 240.0)
				{
					x += 280.0;
					y = 4.0;
				}
				else
				{
					y += 14.0;
				}
			}
		}
	}
}
