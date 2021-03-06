﻿// ╔═════════════════════════════════════════════════════════════╗
// ║ TrainManager.cs for the Route Viewer                        ║
// ╠═════════════════════════════════════════════════════════════╣
// ║ This file cannot be used in the openBVE main program.       ║
// ║ The file from the openBVE main program cannot be used here. ║
// ╚═════════════════════════════════════════════════════════════╝

using OpenBveApi.Math;

namespace OpenBve {
	using System;

	internal static class TrainManager {

// Silence the absurd amount of unused variable warnings
#pragma warning disable 0649

		// structures
		internal struct Axle {
			internal TrackManager.TrackFollower Follower;
		}
		internal struct Coupler { }
		internal struct Section { }

		// cars
		internal struct Door {
			internal int Direction;
			internal double State;
		}
		internal struct AccelerationCurve {
			internal double StageZeroAcceleration;
			internal double StageOneSpeed;
			internal double StageOneAcceleration;
			internal double StageTwoSpeed;
			internal double StageTwoExponent;
		}
		internal enum CarBrakeType {
			ElectromagneticStraightAirBrake = 0,
			ElectricCommandBrake = 1,
			AutomaticAirBrake = 2
		}
		internal enum EletropneumaticBrakeType {
			None = 0,
			ClosingElectromagneticValve = 1,
			DelayFillingControl = 2
		}
		internal enum AirBrakeHandleState {
			Invalid = -1,
			Release = 0,
			Lap = 1,
			Service = 2,
		}
		internal struct AirBrakeHandle {
			internal AirBrakeHandleState Driver;
			internal AirBrakeHandleState Security;
			internal AirBrakeHandleState Actual;
			internal AirBrakeHandleState DelayedValue;
			internal double DelayedTime;
		}
		internal enum AirBrakeType { Main, Auxillary }
		internal struct CarAirBrake {
			internal AirBrakeType Type;
			internal bool AirCompressorEnabled;
			internal double AirCompressorMinimumPressure;
			internal double AirCompressorMaximumPressure;
			internal double AirCompressorRate;
			internal double MainReservoirCurrentPressure;
			internal double MainReservoirEqualizingReservoirCoefficient;
			internal double MainReservoirBrakePipeCoefficient;
			internal double EqualizingReservoirCurrentPressure;
			internal double EqualizingReservoirNormalPressure;
			internal double EqualizingReservoirServiceRate;
			internal double EqualizingReservoirEmergencyRate;
			internal double EqualizingReservoirChargeRate;
			internal double BrakePipeCurrentPressure;
			internal double BrakePipeNormalPressure;
			internal double BrakePipeChargeRate;
			internal double BrakePipeServiceRate;
			internal double BrakePipeEmergencyRate;
			internal double AuxillaryReservoirCurrentPressure;
			internal double AuxillaryReservoirMaximumPressure;
			internal double AuxillaryReservoirChargeRate;
			internal double AuxillaryReservoirBrakePipeCoefficient;
			internal double AuxillaryReservoirBrakeCylinderCoefficient;
			internal double BrakeCylinderCurrentPressure;
			internal double BrakeCylinderEmergencyMaximumPressure;
			internal double BrakeCylinderServiceMaximumPressure;
			internal double BrakeCylinderEmergencyChargeRate;
			internal double BrakeCylinderServiceChargeRate;
			internal double BrakeCylinderReleaseRate;
			internal double BrakeCylinderSoundPlayedForPressure;
			internal double StraightAirPipeCurrentPressure;
			internal double StraightAirPipeReleaseRate;
			internal double StraightAirPipeServiceRate;
			internal double StraightAirPipeEmergencyRate;
		}
		internal struct CarHoldBrake {
			internal double CurrentAccelerationOutput;
			internal double NextUpdateTime;
			internal double UpdateInterval;
		}
		internal struct CarConstSpeed {
			internal double CurrentAccelerationOutput;
			internal double NextUpdateTime;
			internal double UpdateInterval;
		}
		internal struct CarReAdhesionDevice {
			internal double UpdateInterval;
			internal double ApplicationFactor;
			internal double ReleaseInterval;
			internal double ReleaseFactor;
			internal double CurrentAccelerationOutput;
			internal double NextUpdateTime;
			internal double TimeStable;
		}
		internal struct CarSpecs {
			internal bool IsMotorCar;
			internal AccelerationCurve[] AccelerationCurves;
			internal double AccelerationCurvesMultiplier;
			internal double BrakeDecelerationAtServiceMaximumPressure;
			internal double BrakeControlSpeed;
			internal double MotorDeceleration;
			internal double Mass;
			internal double ExposedFrontalArea;
			internal double UnexposedFrontalArea;
			internal double CoefficientOfStaticFriction;
			internal double CoefficientOfRollingResistance;
			internal double AerodynamicDragCoefficient;
			internal double CenterOfGravityHeight;
			internal double CriticalTopplingAngle;
			internal double CurrentSpeed;
			internal double CurrentPerceivedSpeed;
			internal double CurrentAcceleration;
			internal double CurrentAccelerationOutput;
			internal bool CurrentMotorPower;
			internal bool CurrentMotorBrake;
			internal CarHoldBrake HoldBrake;
			internal CarConstSpeed ConstSpeed;
			internal CarReAdhesionDevice ReAdhesionDevice;
			internal CarBrakeType BrakeType;
			internal EletropneumaticBrakeType ElectropneumaticType;
			internal CarAirBrake AirBrake;
			internal Door[] Doors;
			internal double DoorOpenSpeed;
			internal double DoorCloseSpeed;
			internal bool AnticipatedLeftDoorsOpened;
			internal bool AnticipatedRightDoorsOpened;
			internal double CurrentRollDueToTopplingAngle;
			internal double CurrentRollDueToCantAngle;
			internal double CurrentRollDueToCantAngularSpeed;
			internal double CurrentRollShakeDirection;
			internal double CurrentPitchDueToAccelerationAngle;
			internal double CurrentPitchDueToAccelerationTrackPosition;
			internal double CurrentPitchDueToAccelerationSpeed;
		}
		internal struct CarBrightness {
			internal float PreviousBrightness;
			internal double PreviousTrackPosition;
			internal float NextBrightness;
			internal double NextTrackPosition;
		}
		internal struct Horn {
			internal CarSound Sound;
			internal bool Loop;
		}
		internal struct CarSound {
			/// <summary>The sound buffer to play</summary>
			internal Sounds.SoundBuffer Buffer;
			/// <summary>The source of the sound within the car</summary>
			internal Sounds.SoundSource Source;
			/// <summary>A Vector3 describing the position of the sound source</summary>
			internal Vector3 Position;
		}
		internal struct MotorSoundTableEntry {
			internal int SoundBufferIndex;
			internal float Pitch;
			internal float Gain;
		}
		internal struct MotorSoundTable {
			internal MotorSoundTableEntry[] Entries;
			internal int SoundBufferIndex;
			internal int SoundSourceIndex;
		}
		internal struct MotorSound {
			internal MotorSoundTable[] Tables;
			internal Vector3 Position;
			internal double SpeedConversionFactor;
			internal int SpeedDirection;
			internal const int MotorP1 = 0;
			internal const int MotorP2 = 1;
			internal const int MotorB1 = 2;
			internal const int MotorB2 = 3;
		}
		internal struct CarSounds {
			internal MotorSound Motor;
			internal CarSound Adjust;
			internal CarSound Air;
			internal CarSound AirHigh;
			internal CarSound AirZero;
			internal CarSound Ats;
			internal CarSound AtsCnt;
			internal CarSound Brake;
			internal CarSound BrakeHandleApply;
			internal CarSound BrakeHandleRelease;
			internal CarSound BrakeHandleMin;
			internal CarSound BrakeHandleMax;
			internal CarSound CpEnd;
			internal CarSound CpLoop;
			internal bool CpLoopStarted;
			internal CarSound CpStart;
			internal double CpStartTimeStarted;
			internal CarSound Ding;
			internal CarSound DoorCloseL;
			internal CarSound DoorCloseR;
			internal CarSound DoorOpenL;
			internal CarSound DoorOpenR;
			internal CarSound Eb;
			internal CarSound EmrBrake;
			internal CarSound[] Flange;
			internal double[] FlangeVolume;
			internal CarSound Halt;
			internal Horn[] Horns;
			internal CarSound Loop;
			internal CarSound MasterControllerUp;
			internal CarSound MasterControllerDown;
			internal CarSound MasterControllerMin;
			internal CarSound MasterControllerMax;
			internal CarSound PilotLampOn;
			internal CarSound PilotLampOff;
			internal CarSound PointFrontAxle;
			internal CarSound PointRearAxle;
			internal CarSound Rub;
			internal CarSound ReverserOn;
			internal CarSound ReverserOff;
			internal CarSound[] Run;
			internal double[] RunVolume;
			internal CarSound SpringL;
			internal CarSound SpringR;
			internal CarSound ToAtc;
			internal CarSound ToAts;
			internal CarSound[] Plugin;
			internal int FrontAxleRunIndex;
			internal int RearAxleRunIndex;
			internal int FrontAxleFlangeIndex;
			internal int RearAxleFlangeIndex;
			internal double FlangePitch;
			internal double SpringPlayedAngle;
		}
		internal struct Car {
			internal double Width;
			internal double Height;
			internal double Length;
			internal Axle FrontAxle;
			internal Axle RearAxle;
			internal double FrontAxlePosition;
			internal double RearAxlePosition;
			internal Vector3 Up;
			internal Section[] Sections;
			internal int CurrentSection;
			internal double DriverX;
			internal double DriverY;
			internal double DriverZ;
			internal double DriverYaw;
			internal double DriverPitch;
			internal CarSpecs Specs;
			internal CarSounds Sounds;
			internal bool CurrentlyVisible;
			internal bool Derailed;
			internal bool Topples;
			internal CarBrightness Brightness;

			internal void CreateWorldCoordinates(double CarX, double CarY, double CarZ, out double PositionX, out double PositionY, out double PositionZ, out double DirectionX, out double DirectionY, out double DirectionZ)
			{
				DirectionX = FrontAxle.Follower.WorldPosition.X - RearAxle.Follower.WorldPosition.X;
				DirectionY = FrontAxle.Follower.WorldPosition.Y - RearAxle.Follower.WorldPosition.Y;
				DirectionZ = FrontAxle.Follower.WorldPosition.Z - RearAxle.Follower.WorldPosition.Z;
				double t = DirectionX * DirectionX + DirectionY * DirectionY + DirectionZ * DirectionZ;
				if (t != 0.0)
				{
					t = 1.0 / Math.Sqrt(t);
					DirectionX *= t; DirectionY *= t; DirectionZ *= t;
					double ux = Up.X;
					double uy = Up.Y;
					double uz = Up.Z;
					double sx = DirectionZ * uy - DirectionY * uz;
					double sy = DirectionX * uz - DirectionZ * ux;
					double sz = DirectionY * ux - DirectionX * uy;
					double rx = 0.5 * (FrontAxle.Follower.WorldPosition.X + RearAxle.Follower.WorldPosition.X);
					double ry = 0.5 * (FrontAxle.Follower.WorldPosition.Y + RearAxle.Follower.WorldPosition.Y);
					double rz = 0.5 * (FrontAxle.Follower.WorldPosition.Z + RearAxle.Follower.WorldPosition.Z);
					PositionX = rx + sx * CarX + ux * CarY + DirectionX * CarZ;
					PositionY = ry + sy * CarX + uy * CarY + DirectionY * CarZ;
					PositionZ = rz + sz * CarX + uz * CarY + DirectionZ * CarZ;
				}
				else
				{
					PositionX = FrontAxle.Follower.WorldPosition.X;
					PositionY = FrontAxle.Follower.WorldPosition.Y;
					PositionZ = FrontAxle.Follower.WorldPosition.Z;
					DirectionX = 0.0;
					DirectionY = 1.0;
					DirectionZ = 0.0;
				}
			}
		}

		// train
		internal struct HandleChange {
			internal int Value;
			internal double Time;
		}
		internal struct PowerHandle {
			internal int Driver;
			internal int Security;
			internal int Actual;
			internal HandleChange[] DelayedChanges;
		}
		internal struct BrakeHandle {
			internal int Driver;
			internal int Security;
			internal int Actual;
			internal HandleChange[] DelayedChanges;
		}
		internal struct EmergencyHandle {
			internal bool Driver;
			internal bool Security;
			internal bool Actual;
			internal double ApplicationTime;
		}
		internal struct ReverserHandle {
			internal int Driver;
			internal int Actual;
		}
		internal struct HoldBrakeHandle {
			internal bool Driver;
			internal bool Actual;
		}
		// train security
		internal enum SafetyState {
			Normal = 0,
			Initialization = 1,
			Ringing = 2,
			Emergency = 3,
			Pattern = 4,
			Service = 5
		}
		internal enum SafetySystem {
			Plugin = -1,
			None = 0,
			AtsSn = 1,
			AtsP = 2,
			Atc = 3
		}
		internal struct Ats {
			internal double Time;
			internal bool AtsPAvailable;
			internal double AtsPDistance;
			internal double AtsPTemporarySpeed;
			internal double AtsPPermanentSpeed;
			internal bool AtsPOverride;
			internal double AtsPOverrideTime;
		}
		internal struct Atc {
			internal bool Available;
			internal bool Transmitting;
			internal bool AutomaticSwitch;
			internal double SpeedRestriction;
		}
		internal struct Eb {
			internal bool Available;
			internal SafetyState BellState;
			internal double Time;
			internal bool Reset;
		}
		internal struct TrainPendingTransponder {
			internal TrackManager.TransponderType Type;
			internal bool SwitchSubsystem;
			internal int OptionalInteger;
			internal double OptionalFloat;
			internal int SectionIndex;
		}
		internal struct TrainSafety {
			internal SafetySystem Mode;
			internal SafetySystem ModeChange;
			internal SafetyState State;
			internal TrainPendingTransponder[] PendingTransponders;
			internal Ats Ats;
			internal Atc Atc;
			internal Eb Eb;
		}
		// train specs
		internal enum PassAlarmType {
			None = 0,
			Single = 1,
			Loop = 2
		}
		internal struct TrainAirBrake {
			internal AirBrakeHandle Handle;
		}
		internal struct TrainSpecs {
//			internal double TotalMass;
			internal ReverserHandle CurrentReverser;
			internal double CurrentAverageSpeed;
//			internal double CurrentAverageAcceleration;
//			internal double CurrentAverageJerk;
//			internal double CurrentAirPressure;
//			internal double CurrentAirDensity;
//			internal double CurrentAirTemperature;
//			internal double CurrentElevation;
//			internal bool SingleHandle;
//			internal int PowerNotchReduceSteps;
			internal int MaximumPowerNotch;
			internal PowerHandle CurrentPowerNotch;
			internal int MaximumBrakeNotch;
			internal BrakeHandle CurrentBrakeNotch;
			internal EmergencyHandle CurrentEmergencyBrake;
			internal bool HasHoldBrake;
			internal HoldBrakeHandle CurrentHoldBrake;
			internal bool HasConstSpeed;
			internal bool CurrentConstSpeed;
			internal TrainSafety Safety;
			internal TrainAirBrake AirBrake;
//			internal double DelayPowerStart;
//			internal double DelayPowerStop;
//			internal double DelayBrakeStart;
//			internal double DelayBrakeEnd;
//			internal double DelayServiceBrake;
//			internal double DelayEmergencyBrake;
//			internal PassAlarmType PassAlarm;
		}
		// train
		internal enum TrainState {
			Pending = 0, Available = 1, Disposed = 2, Bogus = 3
		}
		internal enum TrainStopState {
			Pending = 0, Boarding = 1, Completed = 2
		}
		internal class Train {
			//internal int TrainIndex;
			internal TrainState State;
			//internal bool Disposed;
			//internal bool IsBogusTrain;
			internal Car[] Cars;
			internal int Destination;
			//internal Coupler[] Couplers;
			internal int DriverCar;
			internal TrainSpecs Specs;
			//internal TrainPassengers Passengers;
			//internal int Station;
			//internal bool StationFrontCar;
			//internal bool StationRearCar;
			//internal TrainStopState StationState;
			//internal double StationArrivalTime;
			//internal double StationDepartureTime;
			//internal bool StationDepartureSoundPlayed;
			//internal bool StationAdjust;
			//internal double StationStopDifference;
			//internal double[] RouteLimits;
			//internal double CurrentRouteLimit;
			//internal double CurrentSectionLimit;
			internal int CurrentSectionIndex;
			//internal double PretrainAheadTimetable;
			//internal double InternalTimerTimeElapsed;
		}

#pragma warning restore 0649

		// trains
		internal static Train[] Trains = new Train[] { };
		internal static Train PlayerTrain = new Train();

		// ================================

		// create world coordinates
		internal static void CreateWorldCoordinates(Train Train, int CarIndex, double relx, double rely, double relz, out double posx, out double posy, out double posz, out double dirx, out double diry, out double dirz) {
			posx = 0.0; posy = 0.0; posz = 0.0;
			dirx = 0.0; diry = 0.0; dirz = 1.0;
		}

	}
}