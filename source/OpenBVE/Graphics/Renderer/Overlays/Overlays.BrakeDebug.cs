using OpenBveApi.Colors;
using OpenTK.Graphics.OpenGL;

namespace OpenBve
{
	internal static partial class Renderer
	{
		/// <summary>Renders the brake system debug overlay</summary>
		private static void RenderBrakeSystemDebug()
		{
			double oy = 64.0, y = oy, h = 16.0;
			bool[] heading = new bool[6];
			for (int i = 0; i < TrainManager.PlayerTrain.Cars.Length; i++)
			{
				double x = 96.0, w = 128.0;
				// brake pipe
				if (TrainManager.PlayerTrain.Cars[i].Specs.BrakeType == TrainManager.CarBrakeType.AutomaticAirBrake | TrainManager.PlayerTrain.Cars[i].Specs.BrakeType == TrainManager.CarBrakeType.ElectromagneticStraightAirBrake)
				{
					if (!heading[0])
					{
						DrawString(Fonts.SmallFont, "Brake pipe", new System.Drawing.Point((int)x, (int)(oy - 16)), TextAlignment.TopLeft, Color128.White, true);
						heading[0] = true;
					}
					GL.Color3(0.0f, 0.0f, 0.0f);
					RenderOverlaySolid(x, y, x + w, y + h);
					double p = TrainManager.PlayerTrain.Cars[i].Specs.AirBrake.BrakePipeCurrentPressure;
					double r = p / TrainManager.PlayerTrain.Cars[i].Specs.AirBrake.BrakePipeNormalPressure;
					GL.Color3(1.0f, 1.0f, 0.0f);
					RenderOverlaySolid(x, y, x + r * w, y + h);
				}
				x += w + 8.0;
				// auxillary reservoir
				if (TrainManager.PlayerTrain.Cars[i].Specs.BrakeType == TrainManager.CarBrakeType.AutomaticAirBrake | TrainManager.PlayerTrain.Cars[i].Specs.BrakeType == TrainManager.CarBrakeType.ElectromagneticStraightAirBrake)
				{
					if (!heading[1])
					{
						//RenderString(x, oy - 16.0, Fonts.FontType.Small, "Auxillary reservoir", -1, 0.75f, 0.75f, 0.75f, true);
						DrawString(Fonts.SmallFont, "Auxillary reservoir", new System.Drawing.Point((int)x, (int)(oy - 16)), TextAlignment.TopLeft, Color128.White, true);
						heading[1] = true;
					}
					GL.Color3(0.0f, 0.0f, 0.0f);
					RenderOverlaySolid(x, y, x + w, y + h);
					double p = TrainManager.PlayerTrain.Cars[i].Specs.AirBrake.AuxillaryReservoirCurrentPressure;
					double r = p / TrainManager.PlayerTrain.Cars[i].Specs.AirBrake.AuxillaryReservoirMaximumPressure;
					GL.Color3(0.5f, 0.5f, 0.5f);
					RenderOverlaySolid(x, y, x + r * w, y + h);
				}
				x += w + 8.0;
				// brake cylinder
				{
					if (!heading[2])
					{
						//RenderString(x, oy - 16.0, Fonts.FontType.Small, "Brake cylinder", -1, 0.75f, 0.5f, 0.25f, true);
						DrawString(Fonts.SmallFont, "Brake cylinder", new System.Drawing.Point((int)x, (int)(oy - 16)), TextAlignment.TopLeft, Color128.White, true);
						heading[2] = true;
					}
					GL.Color3(0.0f, 0.0f, 0.0f);
					RenderOverlaySolid(x, y, x + w, y + h);
					double p = TrainManager.PlayerTrain.Cars[i].Specs.AirBrake.BrakeCylinderCurrentPressure;
					double r = p / TrainManager.PlayerTrain.Cars[i].Specs.AirBrake.BrakeCylinderEmergencyMaximumPressure;
					GL.Color3(0.75f, 0.5f, 0.25f);
					RenderOverlaySolid(x, y, x + r * w, y + h);
				}
				x += w + 8.0;
				// main reservoir
				if (TrainManager.PlayerTrain.Cars[i].Specs.AirBrake.Type == TrainManager.AirBrakeType.Main)
				{
					if (!heading[3])
					{
						//RenderString(x, oy - 16.0, Fonts.FontType.Small, "Main reservoir", -1, 1.0f, 0.0f, 0.0f, true);
						DrawString(Fonts.SmallFont, "Main reservoir", new System.Drawing.Point((int)x, (int)(oy - 16)), TextAlignment.TopLeft, Color128.White, true);
						heading[3] = true;
					}
					GL.Color3(0.0f, 0.0f, 0.0f);
					RenderOverlaySolid(x, y, x + w, y + h);
					double p = TrainManager.PlayerTrain.Cars[i].Specs.AirBrake.MainReservoirCurrentPressure;
					double r = p / TrainManager.PlayerTrain.Cars[i].Specs.AirBrake.AirCompressorMaximumPressure;
					GL.Color3(1.0f, 0.0f, 0.0f);
					RenderOverlaySolid(x, y, x + r * w, y + h);
				}
				x += w + 8.0;
				// equalizing reservoir
				if (TrainManager.PlayerTrain.Cars[i].Specs.AirBrake.Type == TrainManager.AirBrakeType.Main)
				{
					if (!heading[4])
					{
						//RenderString(x, oy - 16.0, Fonts.FontType.Small, "Equalizing reservoir", -1, 0.0f, 0.75f, 0.0f, true);
						DrawString(Fonts.SmallFont, "Equalizing reservoir", new System.Drawing.Point((int)x, (int)(oy - 16)), TextAlignment.TopLeft, Color128.White, true);
						heading[4] = true;
					}
					GL.Color3(0.0f, 0.0f, 0.0f);
					RenderOverlaySolid(x, y, x + w, y + h);
					double p = TrainManager.PlayerTrain.Cars[i].Specs.AirBrake.EqualizingReservoirCurrentPressure;
					double r = p / TrainManager.PlayerTrain.Cars[i].Specs.AirBrake.EqualizingReservoirNormalPressure;
					GL.Color3(0.0f, 0.75f, 0.0f);
					RenderOverlaySolid(x, y, x + r * w, y + h);
				}
				x += w + 8.0;
				// straight air pipe
				if (TrainManager.PlayerTrain.Cars[i].Specs.BrakeType == TrainManager.CarBrakeType.ElectromagneticStraightAirBrake & TrainManager.PlayerTrain.Cars[i].Specs.AirBrake.Type == TrainManager.AirBrakeType.Main)
				{
					if (!heading[5])
					{
						//RenderString(x, oy - 16.0, Fonts.FontType.Small, "Straight air pipe", -1, 0.0f, 0.75f, 1.0f, true);
						DrawString(Fonts.SmallFont, "Straight air pipe", new System.Drawing.Point((int)x, (int)(oy - 16)), TextAlignment.TopLeft, Color128.White, true);
						heading[5] = true;
					}
					GL.Color3(0.0f, 0.0f, 0.0f);
					RenderOverlaySolid(x, y, x + w, y + h);
					double p = TrainManager.PlayerTrain.Cars[i].Specs.AirBrake.StraightAirPipeCurrentPressure;
					double r = p / TrainManager.PlayerTrain.Cars[i].Specs.AirBrake.BrakeCylinderEmergencyMaximumPressure;
					GL.Color3(0.0f, 0.75f, 1.0f);
					RenderOverlaySolid(x, y, x + r * w, y + h);
				} //x += w + 8.0;
				GL.Color3(0.0f, 0.0f, 0.0f);
				y += h + 8.0;
			}
		}
	}
}
