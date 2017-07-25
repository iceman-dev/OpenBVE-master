using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenBveApi.Colors;
using OpenBveApi.Math;

namespace OpenBve
{
	class MSTSCabviewFileParser
	{
		// constants
		internal static double StackDistance = 0.000001;
		/// <remarks>EyeDistance is required to be 1.0 by UpdateCarSectionElement and by UpdateCameraRestriction, thus cannot be easily changed.</remarks>
		internal const double EyeDistance = 1.0;

		// parse panel config
		internal static bool ParseCabViewFile(string File, System.Text.Encoding Encoding, TrainManager.Train Train)
		{
			string Folder = Path.GetDirectoryName(File);
			System.Globalization.CultureInfo Culture = System.Globalization.CultureInfo.InvariantCulture;

			string header;
			using (StreamReader reader = new StreamReader(File))
			{
				header = reader.ReadLine() ?? "";
			}
			if (header != "SIMISA@@@@@@@@@@JINX0h0t______")
			{
				//Not a recognised MSTS header
				return false;
			}
			//Read all lines into string list
			List<string> Lines = System.IO.File.ReadLines(File).ToList();
			//Remove first line
			Lines.RemoveAt(0);
			//Remove junk blank lines & trim
			for (int i = Lines.Count - 1; i < 0; i--)
			{
				Lines[i] = Lines[i].Trim();
				if (string.IsNullOrEmpty(Lines[i]))
				{
					Lines.RemoveAt(i);
				}
			}

			//Init
			DriverX = Train.Cars[Train.DriverCar].DriverX;
			DriverY = Train.Cars[Train.DriverCar].DriverY;
			DriverZ = Train.Cars[Train.DriverCar].DriverZ;

			//Parse CVF file

			bool CvF = false;
			Component currentComponent = new Component();
			List<Component> cabComponents = new List<Component>();
			int CurrentLevel = 0;
			for (int i = 0; i < Lines.Count; i++)
			{
				if (Lines[i].StartsWith("Tr_CabViewFile"))
				{
					//Cabview file header found
					CvF = true;
				}
				if (CvF == true)
				{
					for (int j = 0; j < Lines[i].Length; j++)
					{
						if (Lines[i][j] == '(')
						{
							CurrentLevel++;
						}

						if (Lines[i][j] == ')')
						{
							CurrentLevel--;
							if (CurrentLevel < 0)
							{
								//Too many closing brackets
								return false;
							}
						}
					}
					int fob = Lines[i].IndexOf('(');
					int fcb = Lines[i].IndexOf(')');
					if (fob != -1 && fcb != -1)
					{
						//Item declaration
						string itm = Lines[i].Substring(0, fob).Trim().ToLowerInvariant();
						string data = Lines[i].Substring(fob + 1, fcb - fob - 1).Trim();
						data = data.Replace(@"\\", @"\");
						switch (CurrentLevel)
						{
							case 0:
								//An item declaration shouldn't be present outside the structure
								throw new InvalidDataException();
							case 1:
								ParseBaseData(itm, data);
								break;
							case 3:
								switch (currentComponent.Type)
								{
									case ComponentType.None:
										throw new InvalidDataException();
									case ComponentType.Dial:
										ParseDialData(Folder, itm, data, ref currentComponent);
										break;
									case ComponentType.Lever:
										ParseLeverData(Folder, itm, data, ref currentComponent);
										break;
									case ComponentType.TriState:
										ParseTriStateData(Folder, itm, data, ref currentComponent);
										break;
								}
								break;
						}

					}
					else if (fob != -1 && fcb == -1)
					{
						//Section Header
						string itm = Lines[i].Substring(0, fob).Trim().ToLowerInvariant();
						currentComponent = new Component();
						switch (itm)
						{
							case "dial":
								currentComponent.Type = ComponentType.Dial;
								break;
							case "lever":
								currentComponent.Type = ComponentType.Lever;
								break;
							case "tristate":
								currentComponent.Type = ComponentType.TriState;
								break;
						}
					}
					else if (fob == -1 && fcb != -1)
					{
						//Section closure
						//Add the current component to the list of components contained within the cab & reinit variable
						cabComponents.Add(currentComponent);
						currentComponent = new Component();

					}

				}
			}
			if (CurrentLevel != 0)
			{
				//Improperly closed brackets
				return false;
			}


			//Create panel
			if (PanelMainImage == null)
			{
				return false;
			}
			{ //Create camera restriction
				double WorldWidth, WorldHeight;
				if (Screen.Width >= Screen.Height)
				{
					WorldWidth = 2.0 * Math.Tan(0.5 * World.HorizontalViewingAngle) * EyeDistance;
					WorldHeight = WorldWidth / World.AspectRatio;
				}
				else
				{
					WorldHeight = 2.0 * Math.Tan(0.5 * World.VerticalViewingAngle) * EyeDistance / World.AspectRatio;
					WorldWidth = WorldHeight * World.AspectRatio;
				}
				double x0 = (PanelLeft - PanelCenterX) / PanelResolution;
				double x1 = (PanelRight - PanelCenterX) / PanelResolution;
				double y0 = (PanelCenterY - PanelBottom) / PanelResolution * World.AspectRatio;
				double y1 = (PanelCenterY - PanelTop) / PanelResolution * World.AspectRatio;
				World.CameraRestrictionBottomLeft = new Vector3(x0 * WorldWidth, y0 * WorldHeight, EyeDistance);
				World.CameraRestrictionTopRight = new Vector3(x1 * WorldWidth, y1 * WorldHeight, EyeDistance);
				Train.Cars[Train.DriverCar].DriverYaw = Math.Atan((PanelCenterX - PanelOriginX) * WorldWidth / PanelResolution);
				Train.Cars[Train.DriverCar].DriverPitch = Math.Atan((PanelOriginY - PanelCenterY) * WorldWidth / PanelResolution);
			}
			PanelMainImage = OpenBveApi.Path.CombineFile(Folder, PanelMainImage);
			if (System.IO.File.Exists(PanelMainImage))
			{
				Textures.Texture tday;
				Textures.RegisterTexture(PanelMainImage, new OpenBveApi.Textures.TextureParameters(null, null), out tday);
				OpenBVEGame.RunInRenderThread(() =>
				{
					Textures.LoadTexture(tday, Textures.OpenGlTextureWrapMode.ClampClamp);
				});
				PanelBitmapWidth = (double)tday.Width;
				PanelBitmapHeight = (double)tday.Height;
				CreateElement(Train, 0.0, 0.0, PanelBitmapWidth, PanelBitmapHeight, 0.5, 0.5, 0.0, PanelResolution, PanelLeft, PanelRight, PanelTop, PanelBottom, PanelBitmapWidth, PanelBitmapHeight, PanelCenterX, PanelCenterY, PanelOriginX, PanelOriginY, DriverX, DriverY, DriverZ, tday, null, new Color32(255, 255, 255, 255), false);
			}
			else
			{
				//Main panel image doesn't exist
				return false;
			}
			int Layer = 1;
			for (int i = 0; i < cabComponents.Count; i++)
			{
				//Check for texture existance
				if (System.IO.File.Exists(cabComponents[i].TexturePath) && cabComponents[i].Units != null)
				{
					//Create and register texture

					//Create element
					double rW = 1024.0 / 640.0;
					double rH = 768.0 / 480.0;
					int wday, hday;
					int j;
					string f;
					switch (cabComponents[i].Type)
					{
						case ComponentType.Dial:
							Textures.Texture tday;
							Textures.RegisterTexture(cabComponents[i].TexturePath, new OpenBveApi.Textures.TextureParameters(null, null), out tday);
							OpenBVEGame.RunInRenderThread(() =>
							{
								Textures.LoadTexture(tday, Textures.OpenGlTextureWrapMode.ClampClamp);
							});
							//Get final position from the 640px panel (Yuck...)
							cabComponents[i].Position.X *= rW;
							cabComponents[i].Position.Y *= rH;
							cabComponents[i].Size.X *= rW;
							cabComponents[i].Size.Y *= rH;
							cabComponents[i].PivotPoint *= rH;
							double w = (double)tday.Width;
							double h = (double)tday.Height;
							j = CreateElement(Train, cabComponents[i].Position.X, cabComponents[i].Position.Y, cabComponents[i].Size.X, cabComponents[i].Size.Y, (0.5 * cabComponents[i].Size.X) / (w * rW), cabComponents[i].PivotPoint / (h * rH), (double)Layer * StackDistance, PanelResolution, PanelLeft, PanelRight, PanelTop, PanelBottom, PanelBitmapWidth, PanelBitmapHeight, PanelCenterX, PanelCenterY, PanelOriginX, PanelOriginY, DriverX, DriverY, DriverZ, tday, null, new Color32(255, 255, 255), false);
							Train.Cars[Train.DriverCar].CarSections[0].Elements[j].RotateZDirection = new Vector3(0.0, 0.0, -1.0);
							Train.Cars[Train.DriverCar].CarSections[0].Elements[j].RotateXDirection = new Vector3(1.0, 0.0, 0.0);
							Train.Cars[Train.DriverCar].CarSections[0].Elements[j].RotateYDirection = Vector3.Cross(Train.Cars[Train.DriverCar].CarSections[0].Elements[j].RotateZDirection, Train.Cars[Train.DriverCar].CarSections[0].Elements[j].RotateXDirection);
							f = GetStackLanguageFromSubject(Train, cabComponents[i].Units, "Dial " + " in " + File);
							cabComponents[i].InitialAngle -= 360;
							cabComponents[i].InitialAngle *= 0.0174532925199433; //degrees to radians
							cabComponents[i].LastAngle *= 0.0174532925199433;
							double a0 = (cabComponents[i].InitialAngle * cabComponents[i].Maximum - cabComponents[i].LastAngle * cabComponents[i].Minimum) / (cabComponents[i].Maximum - cabComponents[i].Minimum);
							double a1 = (cabComponents[i].LastAngle - cabComponents[i].InitialAngle) / (cabComponents[i].Maximum - cabComponents[i].Minimum);
							f += " " + a1.ToString(Culture) + " * " + a0.ToString(Culture) + " +";
							Train.Cars[Train.DriverCar].CarSections[0].Elements[j].RotateZFunction = FunctionScripts.GetFunctionScriptFromPostfixNotation(f);
							//MSTS cab dials are backstopped as standard
							Train.Cars[Train.DriverCar].CarSections[0].Elements[j].RotateZFunction.Minimum = cabComponents[i].InitialAngle;
							Train.Cars[Train.DriverCar].CarSections[0].Elements[j].RotateZFunction.Maximum = cabComponents[i].LastAngle;
							
							break;
						case ComponentType.Lever:
							cabComponents[i].Position.X *= rW;
							cabComponents[i].Position.Y *= rH;

							Program.CurrentHost.QueryTextureDimensions(cabComponents[i].TexturePath, out wday, out hday);
							if (wday > 0 & hday > 0)
							{
								Textures.Texture[] textures = new Textures.Texture[cabComponents[i].TotalFrames];
								int row = 0;
								int column = 0;
								int frameWidth = wday / cabComponents[i].HorizontalFrames;
								int frameHeight = hday / cabComponents[i].VerticalFrames;
								for (int k = 0; k < cabComponents[i].TotalFrames; k++)
								{
									Textures.RegisterTexture(cabComponents[i].TexturePath, new OpenBveApi.Textures.TextureParameters(new OpenBveApi.Textures.TextureClipRegion(column * frameWidth, row * frameHeight, frameWidth, frameHeight), null), out textures[k]);
									if (column < cabComponents[i].HorizontalFrames - 1)
									{
										column++;
									}
									else
									{
										column = 0;
										row++;
									}
								}
								j = -1;
								for (int k = 0; k < textures.Length; k++)
								{
									int l = CreateElement(Train, cabComponents[i].Position.X, cabComponents[i].Position.Y, cabComponents[i].Size.X * rW, cabComponents[i].Size.Y * rH, 0.5, 0.5, (double)Layer * StackDistance, PanelResolution, PanelLeft, PanelRight, PanelTop, PanelBottom, PanelBitmapWidth, PanelBitmapHeight, PanelCenterX, PanelCenterY, PanelOriginX, PanelOriginY, DriverX, DriverY, DriverZ, textures[k], null, new Color32(255, 255, 255, 255), k != 0);
									if (k == 0) j = l;
								}
								f = GetStackLanguageFromSubject(Train, cabComponents[i].Units, "Lever " + " in " + File);
								Train.Cars[Train.DriverCar].CarSections[0].Elements[j].StateFunction = FunctionScripts.GetFunctionScriptFromPostfixNotation(f);
							}
							break;
						case ComponentType.TriState:
							cabComponents[i].Position.X *= rW;
							cabComponents[i].Position.Y *= rH;
							Program.CurrentHost.QueryTextureDimensions(cabComponents[i].TexturePath, out wday, out hday);
							if (wday > 0 & hday > 0)
							{
								Textures.Texture[] textures = new Textures.Texture[cabComponents[i].TotalFrames];
								int row = 0;
								int column = 0;
								int frameWidth = wday / cabComponents[i].HorizontalFrames;
								int frameHeight = hday / cabComponents[i].VerticalFrames;
								for (int k = 0; k < cabComponents[i].TotalFrames; k++)
								{
									Textures.RegisterTexture(cabComponents[i].TexturePath, new OpenBveApi.Textures.TextureParameters(new OpenBveApi.Textures.TextureClipRegion(column * frameWidth, row * frameHeight, frameWidth, frameHeight), null), out textures[k]);
									if (column < cabComponents[i].HorizontalFrames - 1)
									{
										column++;
									}
									else
									{
										column = 0;
										row++;
									}
								}
								j = -1;
								for (int k = 0; k < textures.Length; k++)
								{
									int l = CreateElement(Train, cabComponents[i].Position.X, cabComponents[i].Position.Y, cabComponents[i].Size.X * rW, cabComponents[i].Size.Y * rH, 0.5, 0.5, (double)Layer * StackDistance, PanelResolution, PanelLeft, PanelRight, PanelTop, PanelBottom, PanelBitmapWidth, PanelBitmapHeight, PanelCenterX, PanelCenterY, PanelOriginX, PanelOriginY, DriverX, DriverY, DriverZ, textures[k], null, new Color32(255, 255, 255, 255), k != 0);
									if (k == 0) j = l;
								}
								f = GetStackLanguageFromSubject(Train, cabComponents[i].Units, "TriState " + " in " + File);
								Train.Cars[Train.DriverCar].CarSections[0].Elements[j].StateFunction = FunctionScripts.GetFunctionScriptFromPostfixNotation(f);
							}
							break;
					}
					Layer++;
				}
				else
				{
					continue;
				}

			}
			return true;
		}

		static string PanelMainImage = null;
		static string HeadoutLeftImage = null;
		static string HeadoutRightImage = null;
		static double DriverX;
		static double DriverY;
		static double DriverZ;
		static double PanelResolution = 1024.0;
		static double PanelLeft = 0.0, PanelRight = 1024.0;
		static double PanelTop = 0.0, PanelBottom = 768.0;
		static double PanelCenterX = 0.0, PanelCenterY = 240.0;
		static double PanelOriginX = 0.0, PanelOriginY = 240.0;
		static double PanelBitmapWidth = 640.0, PanelBitmapHeight = 480.0;


		private static void ParseBaseData(string Command, string Data)
		{
			switch (Command)
			{
				case "cabviewtype":
					// ?? Number to select steam and diesel ??
					break;
				case "cabviewfile":
					//Loads texture
					PanelMainImage = Data;
					//Front, then left, then right

					//Duplicate declared in CabViewWindowFile (??)
					break;
				case "cabviewwindow":
					//Sets initial clip viewport
					//Presumably xPos, yPos, hClip, vClip
					string[] splitData = Data.Split(' ');
					if (splitData.Length != 4)
					{
						//Must contain 4 parameters
						throw new InvalidDataException();
					}
					for (int i = 0; i < 4; i++)
					{
						switch (i)
						{
							case 0:
								Double.TryParse(splitData[i], out PanelLeft);
								break;
							case 1:
								Double.TryParse(splitData[i], out PanelTop);
								break;
							case 2:
								//Double.TryParse(splitData[i], out PanelBitmapWidth);
								break;
							case 3:
								//Double.TryParse(splitData[i], out PanelBitmapHeight);
								break;
						}
					}
					break;
				case "position":
					//Position of cab within loco
					//Presumably X,Y,Z
					break;
				case "direction":
					//Dunno....
					break;
				case "enginedata":
					//Default loco folder this is associated with I think
					//Can probably be ignored
					break;
			}
		}

		private class Component
		{
			internal ComponentType Type = ComponentType.None;
			internal string TexturePath;
			internal Subject Subject;
			internal string Units;
			internal Vector2 Position = new Vector2(0, 0);
			internal Vector2 Size = new Vector2(0, 0);
			internal double PivotPoint;
			internal double InitialAngle;
			internal double LastAngle;
			internal double Maximum;
			internal double Minimum;
			internal int TotalFrames = 0;
			internal int HorizontalFrames = 0;
			internal int VerticalFrames = 0;
		}

		private static void ParseDialData(string Folder, string Command, string Data, ref Component Dial)
		{
			string[] splitData;
			switch (Command)
			{
				case "graphic":
					//Loads image
					Dial.TexturePath = OpenBveApi.Path.CombineFile(Folder, Data);
					break;
				case "pivot":
					double.TryParse(Data, out Dial.PivotPoint);
					break;
				case "position":
					splitData = Data.Split(' ');
					if (splitData.Length != 4)
					{
						//Must contain 4 parameters
						throw new InvalidDataException();
					}
					Double.TryParse(splitData[0], out Dial.Position.X);
					Double.TryParse(splitData[1], out Dial.Position.Y);
					Double.TryParse(splitData[2], out Dial.Size.X);
					Double.TryParse(splitData[3], out Dial.Size.Y);
					break;
				case "type":
					//Defines the subject of the dial
					switch (Data.ToLowerInvariant())
					{
						case "speedometer dial":
							Dial.Subject = Subject.Speedometer;
							break;
						case "brake_pipe dial":
							Dial.Subject = Subject.BrakePipe;
							break;
						case "brake_cyl dial":
							Dial.Subject = Subject.BrakeCylinder;
							break;
						case "ammeter dial":
							Dial.Subject = Subject.Ammeter;
							break;
						//default:
						//	NOT IMPLEMENTED ALL VARIANTS YET
						//	throw new NotSupportedException(Data + " is not a supported subject for a Dial");
					}
					break;
				case "units":
					//Defines the units in which the subject is measured on the dial
					switch (Data.ToLowerInvariant())
					{
						case "amps":
							if (Dial.Subject != Subject.Ammeter)
							{
								throw new NotSupportedException("AMPS are not a valid unit for a dial with a subject of " + Dial.Subject);
							}
							Dial.Units = "motor";
							break;
						case "miles_per_hour":
							Dial.Units = "mph";
							break;
						case "psi":
							switch (Dial.Subject)
							{
								case Subject.BrakeCylinder:
									Dial.Units = "bc_psi";
									break;
								case Subject.BrakePipe:
									Dial.Units = "bp_psi";
									break;
								default:
									throw new NotSupportedException("PSI are not a valid unit for a dial with a subject of " + Dial.Subject);

							}
							break;
					}
					break;
				case "scalepos":
					splitData = Data.Split(' ');
					if (splitData.Length != 2)
					{
						//Must contain 2 parameters
						throw new InvalidDataException();
					}
					Double.TryParse(splitData[0], out Dial.InitialAngle);
					Double.TryParse(splitData[1], out Dial.LastAngle);
					break;
				case "scalerange":
					if (Dial.Subject == Subject.Ammeter)
					{
						Dial.Minimum = 0;
						Dial.Maximum = 1;
						break;
					}
					splitData = Data.Split(' ');
					if (splitData.Length != 2)
					{
						//Must contain 2 parameters
						throw new InvalidDataException();
					}
					Double.TryParse(splitData[0], out Dial.Minimum);
					Double.TryParse(splitData[1], out Dial.Maximum);
					break;

			}
		}

		private static void ParseTriStateData(string Folder, string Command, string Data, ref Component TriState)
		{
			string[] splitData;
			switch (Command)
			{
				case "graphic":
					//Loads image
					TriState.TexturePath = OpenBveApi.Path.CombineFile(Folder, Data);
					break;
				case "position":
					//Places within the cab, then defines the size of a frame
					splitData = Data.Split(' ');
					if (splitData.Length != 4)
					{
						//Must contain 4 parameters
						throw new InvalidDataException();
					}
					//Top left X,Y
					//Then size H,W
					Double.TryParse(splitData[0], out TriState.Position.X);
					Double.TryParse(splitData[1], out TriState.Position.Y);
					Double.TryParse(splitData[2], out TriState.Size.X);
					Double.TryParse(splitData[3], out TriState.Size.Y);
					break;
				case "type":
					//Defines available subjects
					switch (Data.ToLowerInvariant())
					{
						case "direction tri_state":
							TriState.Subject = Subject.Direction;
							TriState.Units = "rev";
							break;
						//default:
						//	NOT IMPLEMENTED ALL VARIANTS YET
						//	throw new NotSupportedException(Data + " is not a supported subject for a Lever");
					}
					break;
				case "units":
					//Defines the units in which the subject is measured on the dial
					break;
				case "numframes":
					//Total number of frames, frames in H row, frames in V column
					splitData = Data.Split(' ');
					if (splitData.Length != 3)
					{
						//Must contain 4 parameters
						throw new InvalidDataException();
					}
					//Top left X,Y
					//Then size H,W
					int.TryParse(splitData[0], out TriState.TotalFrames);
					int.TryParse(splitData[1], out TriState.HorizontalFrames);
					int.TryParse(splitData[2], out TriState.VerticalFrames);
					break;

			}
		}

		private static void ParseLeverData(string Folder, string Command, string Data, ref Component Lever)
		{
			string[] splitData;
			switch (Command)
			{
				case "graphic":
					//Loads image
					Lever.TexturePath = OpenBveApi.Path.CombineFile(Folder, Data);
					break;
				case "position":
					//Places within the cab, then defines the size of a frame
					splitData = Data.Split(' ');
					if (splitData.Length != 4)
					{
						//Must contain 4 parameters
						throw new InvalidDataException();
					}
					//Top left X,Y
					//Then size H,W
					Double.TryParse(splitData[0], out Lever.Position.X);
					Double.TryParse(splitData[1], out Lever.Position.Y);
					Double.TryParse(splitData[2], out Lever.Size.X);
					Double.TryParse(splitData[3], out Lever.Size.Y);
					break;
				case "type":
					//Defines available subjects
					switch (Data.ToLowerInvariant())
					{
						case "throttle lever":
							Lever.Subject = Subject.PowerHandle;
							Lever.Units = "power";
							break;
						case "engine_brake lever":
							Lever.Subject = Subject.EngineBrakeHandle;
							Lever.Units = "brake";
							break;
						case "train_brake lever":
							Lever.Subject = Subject.TrainBrakeHandle;
							Lever.Units = "brake";
							break;
						//default:
						//	NOT IMPLEMENTED ALL VARIANTS YET
						//	throw new NotSupportedException(Data + " is not a supported subject for a Lever");
					}
					break;
				case "units":
					//Defines the units in which the subject is measured on the lever
					break;
				case "numframes":
					//Total number of frames, frames in H row, frames in V column
					splitData = Data.Split(' ');
					if (splitData.Length != 3)
					{
						//Must contain 4 parameters
						throw new InvalidDataException();
					}
					//Top left X,Y
					//Then size H,W
					int.TryParse(splitData[0], out Lever.TotalFrames);
					int.TryParse(splitData[1], out Lever.HorizontalFrames);
					int.TryParse(splitData[2], out Lever.VerticalFrames);
					break;

			}
		}

		private enum ComponentType
		{
			/// <summary>None</summary>
			None = 0,
			/// <summary>Dial based control</summary>
			Dial = 1,
			/// <summary>Lever based control</summary>
			Lever = 2,
			/// <summary>Tri-state based control</summary>
			TriState = 3

		}

		private enum Subject
		{
			Speedometer = 0,
			BrakePipe = 1,
			BrakeCylinder = 2,
			Ammeter = 3,
			Direction = 4,
			PowerHandle = 5,
			EngineBrakeHandle = 6,
			TrainBrakeHandle = 7
		}


		// get stack language from subject
		private static string GetStackLanguageFromSubject(TrainManager.Train Train, string Subject, string ErrorLocation)
		{
			System.Globalization.CultureInfo Culture = System.Globalization.CultureInfo.InvariantCulture;
			string Suffix = "";
			{
				// detect d# suffix
				int i;
				for (i = Subject.Length - 1; i >= 0; i--)
				{
					int a = char.ConvertToUtf32(Subject, i);
					if (a < 48 | a > 57) break;
				}
				if (i >= 0 & i < Subject.Length - 1)
				{
					if (Subject[i] == 'd' | Subject[i] == 'D')
					{
						int n;
						if (int.TryParse(Subject.Substring(i + 1), System.Globalization.NumberStyles.Integer, Culture, out n))
						{
							if (n == 0)
							{
								Suffix = " floor 10 mod";
							}
							else
							{
								string t0 = Math.Pow(10.0, (double)n).ToString(Culture);
								string t1 = Math.Pow(10.0, (double)-n).ToString(Culture);
								Suffix = " ~ " + t0 + " >= <> " + t1 + " * floor 10 mod 10 ?";
							}
							Subject = Subject.Substring(0, i);
							i--;
						}
					}
				}
			}
			// transform subject
			string Code;
			switch (Subject.ToLowerInvariant())
			{
				case "acc":
					Code = "acceleration";
					break;
				case "motor":
					Code = "accelerationmotor";
					break;
				case "true":
					Code = "1";
					break;
				case "kmph":
					Code = "speedometer abs 3.6 *";
					break;
				case "mph":
					Code = "speedometer abs 2.2369362920544 *";
					break;
				case "ms":
					Code = "speedometer abs";
					break;
				case "bc":
					Code = "brakecylinder 0.001 *";
					break;
				case "bc_psi":
					Code = "brakecylinder 0.000145038 *";
					break;
				case "mr":
					Code = "mainreservoir 0.001 *";
					break;
				case "sap":
					Code = "straightairpipe 0.001 *";
					break;
				case "bp":
					Code = "brakepipe 0.001 *";
					break;
				case "bp_psi":
					Code = "brakepipe 0.000145038 *";
					break;
				case "er":
					Code = "equalizingreservoir 0.001 *";
					break;
				case "door":
					Code = "1 doors -";
					break;
				case "csc":
					Code = "constSpeed";
					break;
				case "power":
					Code = "brakeNotchLinear 0 powerNotch ?";
					break;
				case "brake":
					Code = "brakeNotchLinear";
					break;
				case "rev":
					Code = "reverserNotch ++";
					break;
				case "hour":
					Code = "0.000277777777777778 time * 24 mod floor";
					break;
				case "min":
					Code = "0.0166666666666667 time * 60 mod floor";
					break;
				case "sec":
					Code = "time 60 mod floor";
					break;
				case "atc":
					Code = "271 pluginstate";
					break;
				default:
				{
					Code = "0";
					bool unsupported = true;
					if (Subject.StartsWith("ats", StringComparison.OrdinalIgnoreCase))
					{
						string a = Subject.Substring(3);
						int n; if (int.TryParse(a, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out n))
						{
							if (n >= 0 & n <= 255)
							{
								Code = n.ToString(Culture) + " pluginstate";
								unsupported = false;
							}
						}
					}
					else if (Subject.StartsWith("doorl", StringComparison.OrdinalIgnoreCase))
					{
						string a = Subject.Substring(5);
						int n; if (int.TryParse(a, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out n))
						{
							if (n >= 0 & n < Train.Cars.Length)
							{
								Code = n.ToString(Culture) + " leftdoorsindex ceiling";
								unsupported = false;
							}
							else
							{
								Code = "2";
								unsupported = false;
							}
						}
					}
					else if (Subject.StartsWith("doorr", StringComparison.OrdinalIgnoreCase))
					{
						string a = Subject.Substring(5);
						int n; if (int.TryParse(a, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out n))
						{
							if (n >= 0 & n < Train.Cars.Length)
							{
								Code = n.ToString(Culture) + " rightdoorsindex ceiling";
								unsupported = false;
							}
							else
							{
								Code = "2";
								unsupported = false;
							}
						}
					}
					if (unsupported)
					{
						Interface.AddMessage(Interface.MessageType.Error, false, "Invalid subject " + Subject + " encountered in " + ErrorLocation);
					}
				}
					break;
			}
			return Code + Suffix;
		}

		/// <summary>Creates a panel element</summary>
		/// <param name="Train">The train to add this panel element to</param>
		/// <param name="Left">The top-left X co-ordinate</param>
		/// <param name="Top">The top-left Y co-ordinate</param>
		/// <param name="Width">The element width</param>
		/// <param name="Height">The element height</param>
		/// <param name="RelativeRotationCenterX">The relative center of rotation (X-axis)</param>
		/// <param name="RelativeRotationCenterY">The relative center of rotation (Y-axis)</param>
		/// <param name="Distance"></param>
		/// <param name="PanelResolution"></param>
		/// <param name="PanelLeft"></param>
		/// <param name="PanelRight"></param>
		/// <param name="PanelTop"></param>
		/// <param name="PanelBottom"></param>
		/// <param name="PanelBitmapWidth"></param>
		/// <param name="PanelBitmapHeight"></param>
		/// <param name="PanelCenterX"></param>
		/// <param name="PanelCenterY"></param>
		/// <param name="PanelOriginX"></param>
		/// <param name="PanelOriginY"></param>
		/// <param name="DriverX"></param>
		/// <param name="DriverY"></param>
		/// <param name="DriverZ"></param>
		/// <param name="DaytimeTexture"></param>
		/// <param name="NighttimeTexture"></param>
		/// <param name="Color"></param>
		/// <param name="AddStateToLastElement"></param>
		/// <returns></returns>
		private static int CreateElement(TrainManager.Train Train, double Left, double Top, double Width, double Height, double RelativeRotationCenterX, double RelativeRotationCenterY, double Distance, double PanelResolution, double PanelLeft, double PanelRight, double PanelTop, double PanelBottom, double PanelBitmapWidth, double PanelBitmapHeight, double PanelCenterX, double PanelCenterY, double PanelOriginX, double PanelOriginY, double DriverX, double DriverY, double DriverZ, Textures.Texture DaytimeTexture, Textures.Texture NighttimeTexture, Color32 Color, bool AddStateToLastElement)
		{
			double WorldWidth, WorldHeight;
			if (Screen.Width >= Screen.Height)
			{
				WorldWidth = 2.0 * Math.Tan(0.5 * World.HorizontalViewingAngle) * EyeDistance;
				WorldHeight = WorldWidth / World.AspectRatio;
			}
			else
			{
				WorldHeight = 2.0 * Math.Tan(0.5 * World.VerticalViewingAngle) * EyeDistance / World.AspectRatio;
				WorldWidth = WorldHeight * World.AspectRatio;
			}
			double x0 = Left / PanelResolution;
			double x1 = (Left + Width) / PanelResolution;
			double y0 = (PanelBottom - Top) / PanelResolution * World.AspectRatio;
			double y1 = (PanelBottom - (Top + Height)) / PanelResolution * World.AspectRatio;
			double xd = 0.5 - PanelCenterX / PanelResolution;
			x0 += xd; x1 += xd;
			double yt = PanelBottom - PanelResolution / World.AspectRatio;
			double yd = (PanelCenterY - yt) / (PanelBottom - yt) - 0.5;
			y0 += yd; y1 += yd;
			x0 = (x0 - 0.5) * WorldWidth;
			x1 = (x1 - 0.5) * WorldWidth;
			y0 = (y0 - 0.5) * WorldHeight;
			y1 = (y1 - 0.5) * WorldHeight;
			double xm = x0 * (1.0 - RelativeRotationCenterX) + x1 * RelativeRotationCenterX;
			double ym = y0 * (1.0 - RelativeRotationCenterY) + y1 * RelativeRotationCenterY;
			Vector3[] v = new Vector3[4];
			v[0] = new Vector3(x0 - xm, y1 - ym, 0);
			v[1] = new Vector3(x0 - xm, y0 - ym, 0);
			v[2] = new Vector3(x1 - xm, y0 - ym, 0);
			v[3] = new Vector3(x1 - xm, y1 - ym, 0);
			World.Vertex t0 = new World.Vertex(v[0], new Vector2(0.0f, 1.0f));
			World.Vertex t1 = new World.Vertex(v[1], new Vector2(0.0f, 0.0f));
			World.Vertex t2 = new World.Vertex(v[2], new Vector2(1.0f, 0.0f));
			World.Vertex t3 = new World.Vertex(v[3], new Vector2(1.0f, 1.0f));
			ObjectManager.StaticObject Object = new ObjectManager.StaticObject();
			Object.Mesh.Vertices = new World.Vertex[] { t0, t1, t2, t3 };
			Object.Mesh.Faces = new World.MeshFace[] { new World.MeshFace(new int[] { 0, 1, 2, 3 }) };
			Object.Mesh.Materials = new World.MeshMaterial[1];
			Object.Mesh.Materials[0].Flags = (byte)(DaytimeTexture != null ? World.MeshMaterial.TransparentColorMask : 0);
			Object.Mesh.Materials[0].Color = Color;
			Object.Mesh.Materials[0].TransparentColor = new Color24(0, 0, 255);
			Object.Mesh.Materials[0].DaytimeTexture = DaytimeTexture;
			Object.Mesh.Materials[0].NighttimeTexture = NighttimeTexture;
			Object.Dynamic = true;
			// calculate offset
			Vector3 o;
			o.X = xm + DriverX;
			o.Y = ym + DriverY;
			o.Z = EyeDistance - Distance + DriverZ;
			// add object
			if (AddStateToLastElement)
			{
				int n = Train.Cars[Train.DriverCar].CarSections[0].Elements.Length - 1;
				int j = Train.Cars[Train.DriverCar].CarSections[0].Elements[n].States.Length;
				Array.Resize<ObjectManager.AnimatedObjectState>(ref Train.Cars[Train.DriverCar].CarSections[0].Elements[n].States, j + 1);
				Train.Cars[Train.DriverCar].CarSections[0].Elements[n].States[j].Position = o;
				Train.Cars[Train.DriverCar].CarSections[0].Elements[n].States[j].Object = Object;
				return n;
			}
			else
			{
				int n = Train.Cars[Train.DriverCar].CarSections[0].Elements.Length;
				Array.Resize<ObjectManager.AnimatedObject>(ref Train.Cars[Train.DriverCar].CarSections[0].Elements, n + 1);
				Train.Cars[Train.DriverCar].CarSections[0].Elements[n] = new ObjectManager.AnimatedObject();
				Train.Cars[Train.DriverCar].CarSections[0].Elements[n].States = new ObjectManager.AnimatedObjectState[1];
				Train.Cars[Train.DriverCar].CarSections[0].Elements[n].States[0].Position = o;
				Train.Cars[Train.DriverCar].CarSections[0].Elements[n].States[0].Object = Object;
				Train.Cars[Train.DriverCar].CarSections[0].Elements[n].CurrentState = 0;
				Train.Cars[Train.DriverCar].CarSections[0].Elements[n].ObjectIndex = ObjectManager.CreateDynamicObject();
				ObjectManager.Objects[Train.Cars[Train.DriverCar].CarSections[0].Elements[n].ObjectIndex] = ObjectManager.CloneObject(Object);
				return n;
			}
		}
	}
}
