﻿using System;
using OpenBve.BrakeSystems;
using OpenBveApi;
using OpenBveApi.Math;

namespace OpenBve {
	internal static class SoundCfgParser {

		/// <summary>Parses the sound configuration file for a train</summary>
		/// <param name="TrainPath">The absolute on-disk path to the train's folder</param>
		/// <param name="Encoding">The train's text encoding</param>
		/// <param name="Train">The train to which to apply the new sound configuration</param>
		internal static void ParseSoundConfig(string TrainPath, System.Text.Encoding Encoding, TrainManager.Train Train) {
			string FileName = OpenBveApi.Path.CombineFile(TrainPath, "sound.cfg");
			if (System.IO.File.Exists(FileName)) {
				LoadBve4Sounds(FileName, TrainPath, Encoding, Train);
			} else {
				LoadBve2Sounds(TrainPath, Train);
			}
		}
		
		/// <summary>Loads the default ATS plugin sound set</summary>
		/// <param name="train">The train</param>
		/// <param name="trainFolder">The absolute on-disk path to the train's folder</param>
		internal static void LoadDefaultATSSounds(TrainManager.Train train, string trainFolder) {
			Vector3 position = new Vector3(train.Cars[train.DriverCar].DriverPosition.X, train.Cars[train.DriverCar].DriverPosition.Y, train.Cars[train.DriverCar].DriverPosition.Z + 1.0);
			const double radius = 2.0;
			train.Cars[train.DriverCar].Sounds.Plugin = new TrainManager.CarSound[] {
				TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, "ats.wav"), position, radius),
				TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, "atscnt.wav"), position, radius),
				TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, "ding.wav"), position, radius),
				TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, "toats.wav"), position, radius),
				TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, "toatc.wav"), position, radius),
				TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, "eb.wav"), position, radius)
			};
		}

		/// <summary>Initializes a train with the default (empty) set of car sounds</summary>
		/// <param name="train">The train</param>
		internal static void InitializeCarSounds(TrainManager.Train train) {
			// initialize
			for (int i = 0; i < train.Cars.Length; i++) {
				train.Cars[i].Sounds.Run = new TrainManager.CarSound[] { };
				train.Cars[i].Sounds.Flange = new TrainManager.CarSound[] { };
				train.Cars[i].Sounds.Adjust = TrainManager.CarSound.Empty;
				train.Cars[i].Sounds.Brake = TrainManager.CarSound.Empty;
				train.Cars[i].Sounds.BrakeHandleApply = TrainManager.CarSound.Empty;
				train.Cars[i].Sounds.BrakeHandleMin = TrainManager.CarSound.Empty;
				train.Cars[i].Sounds.BrakeHandleMax = TrainManager.CarSound.Empty;
				train.Cars[i].Sounds.BrakeHandleRelease = TrainManager.CarSound.Empty;
				train.Cars[i].Sounds.BreakerResume = TrainManager.CarSound.Empty;
				train.Cars[i].Sounds.BreakerResumeOrInterrupt = TrainManager.CarSound.Empty;
				train.Cars[i].Sounds.DoorCloseL = TrainManager.CarSound.Empty;
				train.Cars[i].Sounds.DoorCloseR = TrainManager.CarSound.Empty;
				train.Cars[i].Sounds.DoorOpenL = TrainManager.CarSound.Empty;
				train.Cars[i].Sounds.DoorOpenR = TrainManager.CarSound.Empty;
				train.Cars[i].Sounds.EmrBrake = TrainManager.CarSound.Empty;
				train.Cars[i].Sounds.Flange = new TrainManager.CarSound[] { };
				train.Cars[i].Sounds.FlangeVolume = new double[] { };
				train.Cars[i].Sounds.Halt = TrainManager.CarSound.Empty;
				train.Cars[i].Horns = new TrainManager.Horn[]
				{
					new TrainManager.Horn(), 
					new TrainManager.Horn(), 
					new TrainManager.Horn()
				};
				train.Cars[i].Sounds.Loop = TrainManager.CarSound.Empty;
				train.Cars[i].Sounds.MasterControllerUp = TrainManager.CarSound.Empty;
				train.Cars[i].Sounds.MasterControllerDown = TrainManager.CarSound.Empty;
				train.Cars[i].Sounds.MasterControllerMin = TrainManager.CarSound.Empty;
				train.Cars[i].Sounds.MasterControllerMax = TrainManager.CarSound.Empty;
				train.Cars[i].Sounds.PilotLampOn = TrainManager.CarSound.Empty;
				train.Cars[i].Sounds.PilotLampOff = TrainManager.CarSound.Empty;
				train.Cars[i].FrontAxle.PointSounds = new TrainManager.CarSound[] { };
				train.Cars[i].RearAxle.PointSounds = new TrainManager.CarSound[] { };
				train.Cars[i].Sounds.ReverserOn = TrainManager.CarSound.Empty;
				train.Cars[i].Sounds.ReverserOff = TrainManager.CarSound.Empty;
				train.Cars[i].Sounds.Rub = TrainManager.CarSound.Empty;
				train.Cars[i].Sounds.Run = new TrainManager.CarSound[] { };
				train.Cars[i].Sounds.RunVolume = new double[] { };
				train.Cars[i].Sounds.SpringL = TrainManager.CarSound.Empty;
				train.Cars[i].Sounds.SpringR = TrainManager.CarSound.Empty;
				train.Cars[i].Sounds.Plugin = new TrainManager.CarSound[] { };
			}
		}

		/// <summary>Loads the sound set for a BVE2 based train</summary>
		/// <param name="train">The train</param>
		/// <param name="trainFolder">The absolute on-disk path to the train's folder</param>
		private static void LoadBve2Sounds(string trainFolder, TrainManager.Train train) {
			// set sound positions and radii
			Vector3 front = new Vector3(0.0, 0.0, 0.5 * train.Cars[train.DriverCar].Length);
			Vector3 center = new Vector3(0.0, 0.0, 0.0);
			Vector3 left = new Vector3(-1.3, 0.0, 0.0);
			Vector3 right = new Vector3(1.3, 0.0, 0.0);
			Vector3 cab = new Vector3(-train.Cars[train.DriverCar].DriverPosition.X, train.Cars[train.DriverCar].DriverPosition.Y, train.Cars[train.DriverCar].DriverPosition.Z - 0.5);
			Vector3 panel = new Vector3(train.Cars[train.DriverCar].DriverPosition.X, train.Cars[train.DriverCar].DriverPosition.Y, train.Cars[train.DriverCar].DriverPosition.Z + 1.0);
			const double large = 30.0;
			const double medium = 10.0;
			const double small = 5.0;
			const double tiny = 2.0;
			InitializeCarSounds(train);
			// load sounds for driver's car
			train.Cars[train.DriverCar].Sounds.Adjust = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, "Adjust.wav"), panel, tiny);
			train.Cars[train.DriverCar].Sounds.Brake = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, "Brake.wav"), center, small);
			train.Cars[train.DriverCar].Sounds.Halt = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, "Halt.wav"), cab, tiny);
			train.Cars[train.DriverCar].Horns[0].LoopSound = TryLoadSoundBuffer(OpenBveApi.Path.CombineFile(trainFolder, "Klaxon0.wav"), large);
			train.Cars[train.DriverCar].Horns[0].Loop = false;
			train.Cars[train.DriverCar].Horns[0].SoundPosition = front;
			if (train.Cars[train.DriverCar].Horns[0].LoopSound == null) {
				train.Cars[train.DriverCar].Horns[0].LoopSound = TryLoadSoundBuffer(OpenBveApi.Path.CombineFile(trainFolder, "Klaxon.wav"), large);
			}
			train.Cars[train.DriverCar].Horns[1].LoopSound = TryLoadSoundBuffer(OpenBveApi.Path.CombineFile(trainFolder, "Klaxon1.wav"), large);
			train.Cars[train.DriverCar].Horns[1].Loop = false;
			train.Cars[train.DriverCar].Horns[1].SoundPosition = front;
			train.Cars[train.DriverCar].Horns[2].LoopSound = TryLoadSoundBuffer(OpenBveApi.Path.CombineFile(trainFolder, "Klaxon2.wav"), medium);
			train.Cars[train.DriverCar].Horns[2].Loop = true;
			train.Cars[train.DriverCar].Horns[2].SoundPosition = front;
			train.Cars[train.DriverCar].Sounds.PilotLampOn = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, "Leave.wav"), cab, tiny);
			train.Cars[train.DriverCar].Sounds.PilotLampOff = TrainManager.CarSound.Empty;
			// load sounds for all cars
			for (int i = 0; i < train.Cars.Length; i++) {
				Vector3 frontaxle = new Vector3(0.0, 0.0, train.Cars[i].FrontAxle.Position);
				Vector3 rearaxle = new Vector3(0.0, 0.0, train.Cars[i].RearAxle.Position);
				//Air brake sounds
				train.Cars[i].Specs.AirBrake.AirSoundPosition = center;
				train.Cars[i].Specs.AirBrake.AirNormal = TryLoadSoundBuffer(OpenBveApi.Path.CombineFile(trainFolder, "Air.wav"), small);
				train.Cars[i].Specs.AirBrake.AirHigh = TryLoadSoundBuffer(OpenBveApi.Path.CombineFile(trainFolder, "AirHigh.wav"), small);
				train.Cars[i].Specs.AirBrake.AirZero = TryLoadSoundBuffer(OpenBveApi.Path.CombineFile(trainFolder, "AirZero.wav"), small);
				if (train.Cars[i].Specs.AirBrake.Type == AirBrake.BrakeType.Main) {
					train.Cars[i].Specs.AirBrake.Compressor.EndSound = TryLoadSoundBuffer(OpenBveApi.Path.CombineFile(trainFolder, "CpEnd.wav"), medium);
					train.Cars[i].Specs.AirBrake.Compressor.LoopSound = TryLoadSoundBuffer(OpenBveApi.Path.CombineFile(trainFolder, "CpLoop.wav"), medium);
					train.Cars[i].Specs.AirBrake.Compressor.StartSound = TryLoadSoundBuffer(OpenBveApi.Path.CombineFile(trainFolder, "CpStart.wav"), medium);
					train.Cars[i].Specs.AirBrake.Compressor.SoundPosition = center;
				}
				train.Cars[i].Sounds.BreakerResume = TrainManager.CarSound.Empty;
				train.Cars[i].Sounds.BreakerResumeOrInterrupt = TrainManager.CarSound.Empty;
				train.Cars[i].Sounds.BreakerResumed = false;
				train.Cars[i].Sounds.DoorCloseL = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, "DoorClsL.wav"), left, small);
				train.Cars[i].Sounds.DoorCloseR = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, "DoorClsR.wav"), right, small);
				if (train.Cars[i].Sounds.DoorCloseL.Buffer == null) {
					train.Cars[i].Sounds.DoorCloseL = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, "DoorCls.wav"), left, small);
				}
				if (train.Cars[i].Sounds.DoorCloseR.Buffer == null) {
					train.Cars[i].Sounds.DoorCloseR = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, "DoorCls.wav"), right, small);
				}
				train.Cars[i].Sounds.DoorOpenL = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, "DoorOpnL.wav"), left, small);
				train.Cars[i].Sounds.DoorOpenR = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, "DoorOpnR.wav"), right, small);
				if (train.Cars[i].Sounds.DoorOpenL.Buffer == null) {
					train.Cars[i].Sounds.DoorOpenL = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, "DoorOpn.wav"), left, small);
				}
				if (train.Cars[i].Sounds.DoorOpenR.Buffer == null) {
					train.Cars[i].Sounds.DoorOpenR = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, "DoorOpn.wav"), right, small);
				}
				train.Cars[i].Sounds.EmrBrake = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, "EmrBrake.wav"), center, medium);
				train.Cars[i].Sounds.Flange = TryLoadSoundArray(trainFolder, "Flange", ".wav", center, medium);
				train.Cars[i].Sounds.FlangeVolume = new double[train.Cars[i].Sounds.Flange.Length];
				train.Cars[i].Sounds.Loop = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, "Loop.wav"), center, medium);
				train.Cars[i].FrontAxle.PointSounds = new TrainManager.CarSound[]
				{
					TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, "Point.wav"), frontaxle, small)
				};
				train.Cars[i].RearAxle.PointSounds = new TrainManager.CarSound[]
				{
					TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, "Point.wav"), rearaxle, small)
				};
				train.Cars[i].Sounds.Rub = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, "Rub.wav"), center, medium);
				train.Cars[i].Sounds.Run = TryLoadSoundArray(trainFolder, "Run", ".wav", center, medium);
				train.Cars[i].Sounds.RunVolume = new double[train.Cars[i].Sounds.Run.Length];
				train.Cars[i].Sounds.SpringL = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, "SpringL.wav"), left, small);
				train.Cars[i].Sounds.SpringR = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, "SpringR.wav"), right, small);
				// motor sound
				if (train.Cars[i].Specs.IsMotorCar) {
					System.Globalization.CultureInfo Culture = System.Globalization.CultureInfo.InvariantCulture;
					train.Cars[i].Sounds.Motor.Position = center;
					for (int j = 0; j < train.Cars[i].Sounds.Motor.Tables.Length; j++) {
						for (int k = 0; k < train.Cars[i].Sounds.Motor.Tables[j].Entries.Length; k++) {
							int idx = train.Cars[i].Sounds.Motor.Tables[j].Entries[k].SoundIndex;
							if (idx >= 0) {
								TrainManager.CarSound snd = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, "Motor" + idx.ToString(Culture) + ".wav"), center, medium);
								train.Cars[i].Sounds.Motor.Tables[j].Entries[k].Buffer = snd.Buffer;
							}
						}
					}
				}
			}
		}

		/// <summary>Loads the sound set for a BVE4 or openBVE sound.cfg based train</summary>
		/// <param name="Encoding">The text encoding for the sound.cfg file</param>
		/// <param name="train">The train</param>
		/// <param name="FileName">The absolute on-disk path to the sound.cfg file</param>
		/// <param name="trainFolder">The absolute on-disk path to the train's folder</param>
		private static void LoadBve4Sounds(string FileName, string trainFolder, System.Text.Encoding Encoding, TrainManager.Train train) {
			//Default sound positions and radii

			//3D center of the car
			Vector3 center = new Vector3(0.0, 0.0, 0.0);
			//Positioned to the left of the car, but centered Y & Z
			Vector3 left = new Vector3(-1.3, 0.0, 0.0);
			//Positioned to the right of the car, but centered Y & Z
			Vector3 right = new Vector3(1.3, 0.0, 0.0);
			//Positioned at the front of the car, centered X and Y
			Vector3 front = new Vector3(0.0, 0.0, 0.5 * train.Cars[train.DriverCar].Length);
			//Positioned at the position of the panel / 3D cab (Remember that the panel is just an object in the world...)
			Vector3 panel = new Vector3(train.Cars[train.DriverCar].DriverPosition.X, train.Cars[train.DriverCar].DriverPosition.Y, train.Cars[train.DriverCar].DriverPosition.Z + 1.0);

			//Radius at which the sound is audible at full volume, presumably in m
			//TODO: All radii are much too small in external mode, but we can't change them by default.....
			double large = 30.0;
			double medium = 10.0;
			double small = 5.0;
			double tiny = 2.0;

			InitializeCarSounds(train);
			// parse configuration file
			System.Globalization.CultureInfo Culture = System.Globalization.CultureInfo.InvariantCulture;
			string[] Lines = System.IO.File.ReadAllLines(FileName, Encoding);
			for (int i = 0; i < Lines.Length; i++) {
				int j = Lines[i].IndexOf(';');
				if (j >= 0) {
					Lines[i] = Lines[i].Substring(0, j).Trim();
				} else {
					Lines[i] = Lines[i].Trim();
				}
			}
			if (Lines.Length < 1 || string.Compare(Lines[0], "version 1.0", StringComparison.OrdinalIgnoreCase) != 0) {
				Interface.AddMessage(Interface.MessageType.Error, false, "Invalid file format encountered in " + FileName + ". The first line is expected to be \"Version 1.0\".");
			}
			string[] MotorFiles = new string[] { };
			double invfac = Lines.Length == 0 ? Loading.TrainProgressCurrentWeight : Loading.TrainProgressCurrentWeight / (double)Lines.Length;
			for (int i = 0; i < Lines.Length; i++) {
				Loading.TrainProgress = Loading.TrainProgressCurrentSum + invfac * (double)i;
				if ((i & 7) == 0) {
					System.Threading.Thread.Sleep(1);
					if (Loading.Cancel) return;
				}
				switch (Lines[i].ToLowerInvariant()) {
					case "[run]":
						i++; while (i < Lines.Length && !Lines[i].StartsWith("[", StringComparison.Ordinal)) {
							int j = Lines[i].IndexOf("=", StringComparison.Ordinal);
							if (j >= 0) {
								string a = Lines[i].Substring(0, j).TrimEnd();
								string b = Lines[i].Substring(j + 1).TrimStart();
								int k;
								if (!int.TryParse(a, System.Globalization.NumberStyles.Integer, Culture, out k)) {
									Interface.AddMessage(Interface.MessageType.Error, false, "Invalid index appeared at line " + (i + 1).ToString(Culture) + " in file " + FileName);
								} else if (b.Length == 0 || Path.ContainsInvalidChars(b)) {
									Interface.AddMessage(Interface.MessageType.Error, false, "FileName contains illegal characters or is empty at line " + (i + 1).ToString(Culture) + " in file " + FileName);
								} else {
									if (k >= 0) {
										for (int c = 0; c < train.Cars.Length; c++) {
											int n = train.Cars[c].Sounds.Run.Length;
											if (k >= n) {
												Array.Resize<TrainManager.CarSound>(ref train.Cars[c].Sounds.Run, k + 1);
												for (int h = n; h < k; h++) {
													train.Cars[c].Sounds.Run[h] = TrainManager.CarSound.Empty;
												}
											}
											train.Cars[c].Sounds.Run[k] = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, b), center, medium);
										}
									} else {
										Interface.AddMessage(Interface.MessageType.Error, false, "Index must be greater or equal to zero at line " + (i + 1).ToString(Culture) + " in file " + FileName);
									}
								}
							} i++;
						} i--; break;
					case "[flange]":
						i++; while (i < Lines.Length && !Lines[i].StartsWith("[", StringComparison.Ordinal)) {
							int j = Lines[i].IndexOf("=", StringComparison.Ordinal);
							if (j >= 0) {
								string a = Lines[i].Substring(0, j).TrimEnd();
								string b = Lines[i].Substring(j + 1).TrimStart();
								int k;
								if (!int.TryParse(a, System.Globalization.NumberStyles.Integer, Culture, out k)) {
									Interface.AddMessage(Interface.MessageType.Error, false, "Invalid index appeared at line " + (i + 1).ToString(Culture) + " in file " + FileName);
								} else if (b.Length == 0 || Path.ContainsInvalidChars(b)) {
									Interface.AddMessage(Interface.MessageType.Error, false, "FileName contains illegal characters or is empty at line " + (i + 1).ToString(Culture) + " in file " + FileName);
								} else {
									if (k >= 0) {
										for (int c = 0; c < train.Cars.Length; c++) {
											int n = train.Cars[c].Sounds.Flange.Length;
											if (k >= n) {
												Array.Resize<TrainManager.CarSound>(ref train.Cars[c].Sounds.Flange, k + 1);
												for (int h = n; h < k; h++) {
													train.Cars[c].Sounds.Flange[h] = TrainManager.CarSound.Empty;
												}
											}
											train.Cars[c].Sounds.Flange[k] = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, b), center, medium);
										}
									} else {
										Interface.AddMessage(Interface.MessageType.Error, false, "Index must be greater or equal to zero at line " + (i + 1).ToString(Culture) + " in file " + FileName);
									}
								}
							} i++;
						} i--; break;
					case "[motor]":
						i++; while (i < Lines.Length && !Lines[i].StartsWith("[", StringComparison.Ordinal)) {
							int j = Lines[i].IndexOf("=", StringComparison.Ordinal);
							if (j >= 0) {
								string a = Lines[i].Substring(0, j).TrimEnd();
								string b = Lines[i].Substring(j + 1).TrimStart();
								int k;
								if (!int.TryParse(a, System.Globalization.NumberStyles.Integer, Culture, out k)) {
									Interface.AddMessage(Interface.MessageType.Error, false, "Invalid index appeared at line " + (i + 1).ToString(Culture) + " in file " + FileName);
								} else if (b.Length == 0 || Path.ContainsInvalidChars(b)) {
									Interface.AddMessage(Interface.MessageType.Error, false, "FileName contains illegal characters or is empty at line " + (i + 1).ToString(Culture) + " in file " + FileName);
								} else {
									if (k >= 0) {
										if (k >= MotorFiles.Length) {
											Array.Resize<string>(ref MotorFiles, k + 1);
										}
										MotorFiles[k] = OpenBveApi.Path.CombineFile(trainFolder, b);
										if (!System.IO.File.Exists(MotorFiles[k])) {
											Interface.AddMessage(Interface.MessageType.Error, true, "File " + MotorFiles[k] + " does not exist at line " + (i + 1).ToString(Culture) + " in file " + FileName);
											MotorFiles[k] = null;
										}
									} else {
										Interface.AddMessage(Interface.MessageType.Error, false, "Index is invalid at line " + (i + 1).ToString(Culture) + " in file " + FileName);
									}
								}
							} i++;
						} i--; break;
					case "[switch]":
						i++; while (i < Lines.Length && !Lines[i].StartsWith("[", StringComparison.Ordinal)) {
							int j = Lines[i].IndexOf("=", StringComparison.Ordinal);
							if (j >= 0) {
								string a = Lines[i].Substring(0, j).TrimEnd();
								string b = Lines[i].Substring(j + 1).TrimStart();
								int runIndex;
								if (b.Length == 0 || Path.ContainsInvalidChars(b)) {
									Interface.AddMessage(Interface.MessageType.Error, false, "FileName contains illegal characters or is empty at line " + (i + 1).ToString(Culture) + " in file " + FileName);
								} else if (NumberFormats.TryParseIntVb6(a, out runIndex)) {
									
									for (int c = 0; c < train.Cars.Length; c++) {
										int n = train.Cars[c].FrontAxle.PointSounds.Length;
										if (runIndex >= n)
										{
											Array.Resize<TrainManager.CarSound>(ref train.Cars[c].FrontAxle.PointSounds, runIndex + 1);
											Array.Resize<TrainManager.CarSound>(ref train.Cars[c].RearAxle.PointSounds, runIndex + 1);
											for (int h = n; h < runIndex; h++)
											{
												train.Cars[c].FrontAxle.PointSounds[h] = TrainManager.CarSound.Empty;
												train.Cars[c].RearAxle.PointSounds[h] = TrainManager.CarSound.Empty;
											}
										}
										Vector3 frontaxle = new Vector3(0.0, 0.0, train.Cars[c].FrontAxle.Position);
										Vector3 rearaxle = new Vector3(0.0, 0.0, train.Cars[c].RearAxle.Position);
										train.Cars[c].FrontAxle.PointSounds[runIndex] = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, b), frontaxle, small);
										train.Cars[c].RearAxle.PointSounds[runIndex] = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, b), rearaxle, small);
									}
								} else {
									Interface.AddMessage(Interface.MessageType.Warning, false, "Unsupported index " + a + " encountered at line " + (i + 1).ToString(Culture) + " in file " + FileName);
								}
							} i++;
						} i--; break;
					case "[brake]":
						i++; while (i < Lines.Length && !Lines[i].StartsWith("[", StringComparison.Ordinal)) {
							int j = Lines[i].IndexOf("=", StringComparison.Ordinal);
							if (j >= 0) {
								string a = Lines[i].Substring(0, j).TrimEnd();
								string b = Lines[i].Substring(j + 1).TrimStart();
								if (b.Length == 0 || Path.ContainsInvalidChars(b)) {
									Interface.AddMessage(Interface.MessageType.Error, false, "FileName contains illegal characters or is empty at line " + (i + 1).ToString(Culture) + " in file " + FileName);
								} else {
									switch (a.ToLowerInvariant()) {
										case "bc release high":
											for (int c = 0; c < train.Cars.Length; c++) {
												train.Cars[c].Specs.AirBrake.AirSoundPosition = center;
												train.Cars[c].Specs.AirBrake.AirHigh = TryLoadSoundBuffer(OpenBveApi.Path.CombineFile(trainFolder, b), small);
											} break;
										case "bc release":
											for (int c = 0; c < train.Cars.Length; c++) {
												train.Cars[c].Specs.AirBrake.AirSoundPosition = center;
												train.Cars[c].Specs.AirBrake.AirNormal = TryLoadSoundBuffer(OpenBveApi.Path.CombineFile(trainFolder, b), small);
											} break;
										case "bc release full":
											for (int c = 0; c < train.Cars.Length; c++) {
												train.Cars[c].Specs.AirBrake.AirSoundPosition = center;
												train.Cars[c].Specs.AirBrake.AirZero = TryLoadSoundBuffer(OpenBveApi.Path.CombineFile(trainFolder, b), small);
											} break;
										case "emergency":
											for (int c = 0; c < train.Cars.Length; c++) {
												train.Cars[c].Sounds.EmrBrake = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, b), center, medium);
											} break;
										case "bp decomp":
											for (int c = 0; c < train.Cars.Length; c++) {
												train.Cars[c].Sounds.Brake = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, b), center, small);
											} break;
										default:
											Interface.AddMessage(Interface.MessageType.Warning, false, "Unsupported key " + a + " encountered at line " + (i + 1).ToString(Culture) + " in file " + FileName);
											break;
									}
								}
							} i++;
						} i--; break;
					case "[compressor]":
						i++; while (i < Lines.Length && !Lines[i].StartsWith("[", StringComparison.Ordinal)) {
							int j = Lines[i].IndexOf("=", StringComparison.Ordinal);
							if (j >= 0) {
								string a = Lines[i].Substring(0, j).TrimEnd();
								string b = Lines[i].Substring(j + 1).TrimStart();
								if (b.Length == 0 || Path.ContainsInvalidChars(b)) {
									Interface.AddMessage(Interface.MessageType.Error, false, "FileName contains illegal characters or is empty at line " + (i + 1).ToString(Culture) + " in file " + FileName);
								} else {
									for (int c = 0; c < train.Cars.Length; c++) {
										if (train.Cars[c].Specs.AirBrake.Type == BrakeSystems.AirBrake.BrakeType.Main) {
											train.Cars[c].Specs.AirBrake.Compressor.SoundPosition = center;
											switch (a.ToLowerInvariant()) {
												case "attack":
													train.Cars[c].Specs.AirBrake.Compressor.StartSound = TryLoadSoundBuffer(OpenBveApi.Path.CombineFile(trainFolder, b), medium);
													break;
												case "loop":
													train.Cars[c].Specs.AirBrake.Compressor.LoopSound = TryLoadSoundBuffer(OpenBveApi.Path.CombineFile(trainFolder, b), medium);
													break;
												case "release":
													train.Cars[c].Specs.AirBrake.Compressor.EndSound = TryLoadSoundBuffer(OpenBveApi.Path.CombineFile(trainFolder, b), medium);
													break;
												default:
													Interface.AddMessage(Interface.MessageType.Warning, false, "Unsupported key " + a + " encountered at line " + (i + 1).ToString(Culture) + " in file " + FileName);
													break;
											}
										}
									}
								}
							} i++;
						} i--; break;
					case "[suspension]":
						i++; while (i < Lines.Length && !Lines[i].StartsWith("[", StringComparison.Ordinal)) {
							int j = Lines[i].IndexOf("=", StringComparison.Ordinal);
							if (j >= 0) {
								string a = Lines[i].Substring(0, j).TrimEnd();
								string b = Lines[i].Substring(j + 1).TrimStart();
								if (b.Length == 0 || Path.ContainsInvalidChars(b)) {
									Interface.AddMessage(Interface.MessageType.Error, false, "FileName contains illegal characters or is empty at line " + (i + 1).ToString(Culture) + " in file " + FileName);
								} else {
									switch (a.ToLowerInvariant()) {
										case "left":
											for (int c = 0; c < train.Cars.Length; c++) {
												train.Cars[c].Sounds.SpringL = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, b), left, small);
											} break;
										case "right":
											for (int c = 0; c < train.Cars.Length; c++) {
												train.Cars[c].Sounds.SpringR = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, b), right, small);
											} break;
										default:
											Interface.AddMessage(Interface.MessageType.Warning, false, "Unsupported key " + a + " encountered at line " + (i + 1).ToString(Culture) + " in file " + FileName);
											break;
									}
								}
							} i++;
						} i--; break;
					case "[horn]":
						i++;
						while (i < Lines.Length && !Lines[i].StartsWith("[", StringComparison.Ordinal)) {
							int j = Lines[i].IndexOf("=", StringComparison.Ordinal);
							if (j >= 0) {
								string a = Lines[i].Substring(0, j).TrimEnd();
								string b = Lines[i].Substring(j + 1).TrimStart();
								if (b.Length == 0 || Path.ContainsInvalidChars(b)) {
									Interface.AddMessage(Interface.MessageType.Error, false, "FileName contains illegal characters or is empty at line " + (i + 1).ToString(Culture) + " in file " + FileName);
								} else {
									switch (a.ToLowerInvariant()) {
										//PRIMARY HORN (Enter)
										case "primarystart":
											train.Cars[train.DriverCar].Horns[0].StartSound = TryLoadSoundBuffer(OpenBveApi.Path.CombineFile(trainFolder, b), large);
											train.Cars[train.DriverCar].Horns[0].SoundPosition = front;
											train.Cars[train.DriverCar].Horns[0].StartEndSounds = true;
											break;
										case "primaryend":
										case "primaryrelease":
											train.Cars[train.DriverCar].Horns[0].EndSound = TryLoadSoundBuffer(OpenBveApi.Path.CombineFile(trainFolder, b), large);
											train.Cars[train.DriverCar].Horns[0].SoundPosition = front;
											train.Cars[train.DriverCar].Horns[0].StartEndSounds = true;
											break;
										case "primaryloop":
										case "primary":
											train.Cars[train.DriverCar].Horns[0].LoopSound = TryLoadSoundBuffer(OpenBveApi.Path.CombineFile(trainFolder, b), large);
											train.Cars[train.DriverCar].Horns[0].SoundPosition = front;
											train.Cars[train.DriverCar].Horns[0].Loop = false;
											break;
										//SECONDARY HORN (Numpad Enter)
										case "secondarystart":
											train.Cars[train.DriverCar].Horns[1].StartSound = TryLoadSoundBuffer(OpenBveApi.Path.CombineFile(trainFolder, b), large);
											train.Cars[train.DriverCar].Horns[1].SoundPosition = front;
											train.Cars[train.DriverCar].Horns[1].StartEndSounds = true;
											break;
										case "secondaryend":
										case "secondaryrelease":
											train.Cars[train.DriverCar].Horns[1].EndSound = TryLoadSoundBuffer(OpenBveApi.Path.CombineFile(trainFolder, b), large);
											train.Cars[train.DriverCar].Horns[1].SoundPosition = front;
											train.Cars[train.DriverCar].Horns[1].StartEndSounds = true;
											break;
										case "secondaryloop":
										case "secondary":
											train.Cars[train.DriverCar].Horns[1].LoopSound = TryLoadSoundBuffer(OpenBveApi.Path.CombineFile(trainFolder, b), large);
											train.Cars[train.DriverCar].Horns[1].SoundPosition = front;
											train.Cars[train.DriverCar].Horns[1].Loop = false;
											break;
										//MUSIC HORN
										case "musicstart":
											train.Cars[train.DriverCar].Horns[2].StartSound = TryLoadSoundBuffer(OpenBveApi.Path.CombineFile(trainFolder, b), medium);
											train.Cars[train.DriverCar].Horns[2].SoundPosition = front;
											train.Cars[train.DriverCar].Horns[2].StartEndSounds = true;
											break;
										case "musicend":
										case "musicrelease":
											train.Cars[train.DriverCar].Horns[2].EndSound = TryLoadSoundBuffer(OpenBveApi.Path.CombineFile(trainFolder, b), medium);
											train.Cars[train.DriverCar].Horns[2].SoundPosition = front;
											train.Cars[train.DriverCar].Horns[2].StartEndSounds = true;
											break;
										case "musicloop":
										case "music":
											train.Cars[train.DriverCar].Horns[2].LoopSound = TryLoadSoundBuffer(OpenBveApi.Path.CombineFile(trainFolder, b), medium);
											train.Cars[train.DriverCar].Horns[2].SoundPosition = front;
											train.Cars[train.DriverCar].Horns[2].Loop = true;
											break;
										default:
											Interface.AddMessage(Interface.MessageType.Warning, false, "Unsupported key " + a + " encountered at line " + (i + 1).ToString(Culture) + " in file " + FileName);
											break;
									}
								}
							} i++;
						} i--; break;
					case "[door]":
						i++; while (i < Lines.Length && !Lines[i].StartsWith("[", StringComparison.Ordinal)) {
							int j = Lines[i].IndexOf("=", StringComparison.Ordinal);
							if (j >= 0) {
								string a = Lines[i].Substring(0, j).TrimEnd();
								string b = Lines[i].Substring(j + 1).TrimStart();
								if (b.Length == 0 || Path.ContainsInvalidChars(b)) {
									Interface.AddMessage(Interface.MessageType.Error, false, "FileName contains illegal characters or is empty at line " + (i + 1).ToString(Culture) + " in file " + FileName);
								} else {
									switch (a.ToLowerInvariant()) {
										case "open left":
											for (int c = 0; c < train.Cars.Length; c++) {
												train.Cars[c].Sounds.DoorOpenL = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, b), left, small);
											} break;
										case "open right":
											for (int c = 0; c < train.Cars.Length; c++) {
												train.Cars[c].Sounds.DoorOpenR = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, b), left, small);
											} break;
										case "close left":
											for (int c = 0; c < train.Cars.Length; c++) {
												train.Cars[c].Sounds.DoorCloseL = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, b), left, small);
											} break;
										case "close right":
											for (int c = 0; c < train.Cars.Length; c++) {
												train.Cars[c].Sounds.DoorCloseR = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, b), left, small);
											} break;
										default:
											Interface.AddMessage(Interface.MessageType.Warning, false, "Unsupported key " + a + " encountered at line " + (i + 1).ToString(Culture) + " in file " + FileName);
											break;
									}
								}
							} i++;
						} i--; break;
					case "[ats]":
						i++; while (i < Lines.Length && !Lines[i].StartsWith("[", StringComparison.Ordinal)) {
							int j = Lines[i].IndexOf("=", StringComparison.Ordinal);
							if (j >= 0) {
								string a = Lines[i].Substring(0, j).TrimEnd();
								string b = Lines[i].Substring(j + 1).TrimStart();
								if (b.Length == 0 || Path.ContainsInvalidChars(b)) {
									Interface.AddMessage(Interface.MessageType.Error, false, "FileName contains illegal characters or is empty at line " + (i + 1).ToString(Culture) + " in file " + FileName);
								} else {
									int k;
									if (!int.TryParse(a, System.Globalization.NumberStyles.Integer, Culture, out k)) {
										Interface.AddMessage(Interface.MessageType.Error, false, "Invalid index appeared at line " + (i + 1).ToString(Culture) + " in file " + FileName);
									} else {
										if (k >= 0) {
											int n = train.Cars[train.DriverCar].Sounds.Plugin.Length;
											if (k >= n) {
												Array.Resize<TrainManager.CarSound>(ref train.Cars[train.DriverCar].Sounds.Plugin, k + 1);
												for (int h = n; h < k; h++) {
													train.Cars[train.DriverCar].Sounds.Plugin[h] = TrainManager.CarSound.Empty;
												}
											}
											train.Cars[train.DriverCar].Sounds.Plugin[k] = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, b), panel, tiny);
										} else {
											Interface.AddMessage(Interface.MessageType.Warning, false, "Index must be greater or equal to zero at line " + (i + 1).ToString(Culture) + " in file " + FileName);
										}
									}
								}
							} i++;
						} i--; break;
					case "[buzzer]":
						i++; while (i < Lines.Length && !Lines[i].StartsWith("[", StringComparison.Ordinal)) {
							int j = Lines[i].IndexOf("=", StringComparison.Ordinal);
							if (j >= 0) {
								string a = Lines[i].Substring(0, j).TrimEnd();
								string b = Lines[i].Substring(j + 1).TrimStart();
								if (b.Length == 0 || Path.ContainsInvalidChars(b)) {
									Interface.AddMessage(Interface.MessageType.Error, false, "FileName contains illegal characters or is empty at line " + (i + 1).ToString(Culture) + " in file " + FileName);
								} else {
									switch (a.ToLowerInvariant()) {
										case "correct":
											train.Cars[train.DriverCar].Sounds.Adjust = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, b), panel, tiny);
											break;
										default:
											Interface.AddMessage(Interface.MessageType.Warning, false, "Unsupported key " + a + " encountered at line " + (i + 1).ToString(Culture) + " in file " + FileName);
											break;
									}
								}
							} i++;
						} i--; break;
					case "[pilot lamp]":
						i++; while (i < Lines.Length && !Lines[i].StartsWith("[", StringComparison.Ordinal)) {
							int j = Lines[i].IndexOf("=", StringComparison.Ordinal);
							if (j >= 0) {
								string a = Lines[i].Substring(0, j).TrimEnd();
								string b = Lines[i].Substring(j + 1).TrimStart();
								if (b.Length == 0 || Path.ContainsInvalidChars(b)) {
									Interface.AddMessage(Interface.MessageType.Error, false, "FileName contains illegal characters or is empty at line " + (i + 1).ToString(Culture) + " in file " + FileName);
								} else {
									switch (a.ToLowerInvariant()) {
										case "on":
											train.Cars[train.DriverCar].Sounds.PilotLampOn = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, b), panel, tiny);
											break;
										case "off":
											train.Cars[train.DriverCar].Sounds.PilotLampOff = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, b), panel, tiny);
											break;
										default:
											Interface.AddMessage(Interface.MessageType.Warning, false, "Unsupported key " + a + " encountered at line " + (i + 1).ToString(Culture) + " in file " + FileName);
											break;
									}
								}
							} i++;
						} i--; break;
					case "[brake handle]":
						i++; while (i < Lines.Length && !Lines[i].StartsWith("[", StringComparison.Ordinal)) {
							int j = Lines[i].IndexOf("=", StringComparison.Ordinal);
							if (j >= 0) {
								string a = Lines[i].Substring(0, j).TrimEnd();
								string b = Lines[i].Substring(j + 1).TrimStart();
								if (b.Length == 0 || Path.ContainsInvalidChars(b)) {
									Interface.AddMessage(Interface.MessageType.Error, false, "FileName contains illegal characters or is empty at line " + (i + 1).ToString(Culture) + " in file " + FileName);
								} else {
									switch (a.ToLowerInvariant()) {
										case "apply":
											train.Cars[train.DriverCar].Sounds.BrakeHandleApply = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, b), panel, tiny);
											break;
										case "release":
											train.Cars[train.DriverCar].Sounds.BrakeHandleRelease = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, b), panel, tiny);
											break;
										case "min":
											train.Cars[train.DriverCar].Sounds.BrakeHandleMin = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, b), panel, tiny);
											break;
										case "max":
											train.Cars[train.DriverCar].Sounds.BrakeHandleMax = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, b), panel, tiny);
											break;
										default:
											Interface.AddMessage(Interface.MessageType.Warning, false, "Unsupported key " + a + " encountered at line " + (i + 1).ToString(Culture) + " in file " + FileName);
											break;
									}
								}
							} i++;
						} i--; break;
					case "[master controller]":
						i++; while (i < Lines.Length && !Lines[i].StartsWith("[", StringComparison.Ordinal)) {
							int j = Lines[i].IndexOf("=", StringComparison.Ordinal);
							if (j >= 0) {
								string a = Lines[i].Substring(0, j).TrimEnd();
								string b = Lines[i].Substring(j + 1).TrimStart();
								if (b.Length == 0 || Path.ContainsInvalidChars(b)) {
									Interface.AddMessage(Interface.MessageType.Error, false, "FileName contains illegal characters or is empty at line " + (i + 1).ToString(Culture) + " in file " + FileName);
								} else {
									switch (a.ToLowerInvariant()) {
										case "up":
											train.Cars[train.DriverCar].Sounds.MasterControllerUp = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, b), panel, tiny);
											break;
										case "down":
											train.Cars[train.DriverCar].Sounds.MasterControllerDown = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, b), panel, tiny);
											break;
										case "min":
											train.Cars[train.DriverCar].Sounds.MasterControllerMin = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, b), panel, tiny);
											break;
										case "max":
											train.Cars[train.DriverCar].Sounds.MasterControllerMax = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, b), panel, tiny);
											break;
										default:
											Interface.AddMessage(Interface.MessageType.Warning, false, "Unsupported key " + a + " encountered at line " + (i + 1).ToString(Culture) + " in file " + FileName);
											break;
									}
								}
							} i++;
						} i--; break;
					case "[reverser]":
						i++; while (i < Lines.Length && !Lines[i].StartsWith("[", StringComparison.Ordinal)) {
							int j = Lines[i].IndexOf("=", StringComparison.Ordinal);
							if (j >= 0) {
								string a = Lines[i].Substring(0, j).TrimEnd();
								string b = Lines[i].Substring(j + 1).TrimStart();
								if (b.Length == 0 || Path.ContainsInvalidChars(b)) {
									Interface.AddMessage(Interface.MessageType.Error, false, "FileName contains illegal characters or is empty at line " + (i + 1).ToString(Culture) + " in file " + FileName);
								} else {
									switch (a.ToLowerInvariant()) {
										case "on":
											train.Cars[train.DriverCar].Sounds.ReverserOn = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, b), panel, tiny);
											break;
										case "off":
											train.Cars[train.DriverCar].Sounds.ReverserOff = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, b), panel, tiny);
											break;
										default:
											Interface.AddMessage(Interface.MessageType.Warning, false, "Unsupported key " + a + " encountered at line " + (i + 1).ToString(Culture) + " in file " + FileName);
											break;
									}
								}
							} i++;
						} i--; break;
					case "[breaker]":
						i++; while (i < Lines.Length && !Lines[i].StartsWith("[", StringComparison.Ordinal)) {
							int j = Lines[i].IndexOf("=", StringComparison.Ordinal);
							if (j >= 0) {
								string a = Lines[i].Substring(0, j).TrimEnd();
								string b = Lines[i].Substring(j + 1).TrimStart();
								if (b.Length == 0 || Path.ContainsInvalidChars(b)) {
									Interface.AddMessage(Interface.MessageType.Error, false, "FileName contains illegal characters or is empty at line " + (i + 1).ToString(Culture) + " in file " + FileName);
								} else {
									switch (a.ToLowerInvariant()) {
										case "on":
											train.Cars[train.DriverCar].Sounds.BreakerResume = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, b), panel, small);
											break;
										case "off":
											train.Cars[train.DriverCar].Sounds.BreakerResumeOrInterrupt = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, b), panel, small);
											break;
										default:
											Interface.AddMessage(Interface.MessageType.Warning, false, "Unsupported key " + a + " encountered at line " + (i + 1).ToString(Culture) + " in file " + FileName);
											break;
									}
								}
							} i++;
						} i--; break;
					case "[others]":
						i++; while (i < Lines.Length && !Lines[i].StartsWith("[", StringComparison.Ordinal)) {
							int j = Lines[i].IndexOf("=", StringComparison.Ordinal);
							if (j >= 0) {
								string a = Lines[i].Substring(0, j).TrimEnd();
								string b = Lines[i].Substring(j + 1).TrimStart();
								if (b.Length == 0 || Path.ContainsInvalidChars(b)) {
									Interface.AddMessage(Interface.MessageType.Error, false, "FileName contains illegal characters or is empty at line " + (i + 1).ToString(Culture) + " in file " + FileName);
								} else {
									switch (a.ToLowerInvariant()) {
										case "noise":
											for (int c = 0; c < train.Cars.Length; c++) {
												if (train.Cars[c].Specs.IsMotorCar | c == train.DriverCar) {
													train.Cars[c].Sounds.Loop = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, b), center, medium);
												}
											} break;
										case "shoe":
											for (int c = 0; c < train.Cars.Length; c++) {
												train.Cars[c].Sounds.Rub = TryLoadSound(OpenBveApi.Path.CombineFile(trainFolder, b), center, medium);
											} break;
										default:
											Interface.AddMessage(Interface.MessageType.Warning, false, "Unsupported key " + a + " encountered at line " + (i + 1).ToString(Culture) + " in file " + FileName);
											break;
									}
								}
							} i++;
						} i--; break;
				}
			}
			for (int i = 0; i < train.Cars.Length; i++) {
				train.Cars[i].Sounds.RunVolume = new double[train.Cars[i].Sounds.Run.Length];
				train.Cars[i].Sounds.FlangeVolume = new double[train.Cars[i].Sounds.Flange.Length];
			}
			// motor sound
			for (int c = 0; c < train.Cars.Length; c++) {
				if (train.Cars[c].Specs.IsMotorCar) {
					train.Cars[c].Sounds.Motor.Position = center;
					for (int i = 0; i < train.Cars[c].Sounds.Motor.Tables.Length; i++) {
						train.Cars[c].Sounds.Motor.Tables[i].Buffer = null;
						train.Cars[c].Sounds.Motor.Tables[i].Source = null;
						for (int j = 0; j < train.Cars[c].Sounds.Motor.Tables[i].Entries.Length; j++) {
							int index = train.Cars[c].Sounds.Motor.Tables[i].Entries[j].SoundIndex;
							if (index >= 0 && index < MotorFiles.Length && MotorFiles[index] != null) {
								train.Cars[c].Sounds.Motor.Tables[i].Entries[j].Buffer = Sounds.RegisterBuffer(MotorFiles[index], medium);
							}
						}
					}
				}
			}
		}
		/// <summary>Loads a sound file into a sound buffer</summary>
		/// <param name="FileName">The sound to load</param>
		/// <param name="Radius">The sound radius</param>
		/// <returns>The new sound buffer</returns>
		private static Sounds.SoundBuffer TryLoadSoundBuffer(string FileName, double Radius)
		{
			if (FileName != null)
			{
				if (System.IO.File.Exists(FileName))
				{
					try
					{
						return Sounds.RegisterBuffer(FileName, Radius);
					}
					catch
					{
						return null;
					}
				}
			}
			return null;
		}

		/// <summary>Attempts to load a sound file into a car-sound</summary>
		/// <param name="FileName">The sound to load</param>
		/// <param name="Position">The position that the sound is emitted from within the car</param>
		/// <param name="Radius">The sound radius</param>
		/// <returns>The new car sound, or an empty car sound if load fails</returns>
		private static TrainManager.CarSound TryLoadSound(string FileName, Vector3 Position, double Radius) {
			TrainManager.CarSound s = TrainManager.CarSound.Empty;
			s.Position = Position;
			s.Source = null;
			if (FileName != null) {
				if (System.IO.File.Exists(FileName)) {
					s.Buffer = Sounds.RegisterBuffer(FileName, Radius);
				}
			}
			return s;
		}

		/// <summary>Attempts to load an array of sound files into a car-sound array</summary>
		/// <param name="Folder">The folder the sound files are located in</param>
		/// <param name="FileStart">The first sound file</param>
		/// <param name="FileEnd">The last sound file</param>
		/// <param name="Position">The position the sound is to be emitted from within the car</param>
		/// <param name="Radius">The sound radius</param>
		/// <returns>The new car sound array</returns>
		private static TrainManager.CarSound[] TryLoadSoundArray(string Folder, string FileStart, string FileEnd, Vector3 Position, double Radius) {
			System.Globalization.CultureInfo Culture = System.Globalization.CultureInfo.InvariantCulture;
			TrainManager.CarSound[] Sounds = { };
			if (!System.IO.Directory.Exists(Folder))
			{
				//Detect whether the given folder exists before attempting to load from it
				return Sounds;
			}
			string[] Files = System.IO.Directory.GetFiles(Folder);
			for (int i = 0; i < Files.Length; i++) {
				string a = System.IO.Path.GetFileName(Files[i]);
				if (a == null) return Sounds;
				if (a.Length > FileStart.Length + FileEnd.Length) {
					if (a.StartsWith(FileStart, StringComparison.OrdinalIgnoreCase) & a.EndsWith(FileEnd, StringComparison.OrdinalIgnoreCase)) {
						string b = a.Substring(FileStart.Length, a.Length - FileEnd.Length - FileStart.Length);
						int n; if (int.TryParse(b, System.Globalization.NumberStyles.Integer, Culture, out n)) {
							if (n >= 0) {
								int m = Sounds.Length;
								if (n >= m) {
									Array.Resize<TrainManager.CarSound>(ref Sounds, n + 1);
									for (int j = m; j < n; j++) {
										Sounds[j] = TrainManager.CarSound.Empty;
										Sounds[j].Source = null;
									}
								}
								Sounds[n] = TryLoadSound(Files[i], Position, Radius);
							}
						}
					}
				}
			}
			return Sounds;
		}

	}
}