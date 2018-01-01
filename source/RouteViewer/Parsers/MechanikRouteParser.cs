using System;
using System.Collections.Generic;
using OpenBveApi.Math;
using OpenBveApi.Colors;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;

namespace OpenBve
{
	using OpenBveApi;

	internal class MechanikRouteParser
	{
		private class Block
		{
			internal double StartingTrackPosition;
			internal List<RouteObject> Objects = new List<RouteObject>();

			internal Block(double TrackPosition)
			{
				this.StartingTrackPosition = TrackPosition;
			}
		}

		internal class RouteObject
		{
			internal int objectIndex;
			internal Vector3 Position;

			internal RouteObject(int index, Vector3 position)
			{
				this.objectIndex = index;
				this.Position = position;
			}
		}

		private class RouteData
		{
			internal List<Block> Blocks = new List<Block>();

			internal int FindBlock(double TrackPosition)
			{
				for (int i = 0; i < this.Blocks.Count; i++)
				{
					if (this.Blocks[i].StartingTrackPosition == TrackPosition)
					{
						return i;
					}
				}
				this.Blocks.Add(new Block(TrackPosition));
				return this.Blocks.Count - 1;
			}

			internal void CreateMissingBlocks()
			{
				for (int i = 0; i < this.Blocks.Count -1; i++)
				{
					while (true)
					{
						if (this.Blocks[i + 1].StartingTrackPosition - this.Blocks[i].StartingTrackPosition > 25)
						{
							this.Blocks.Insert(i + 1, new Block(this.Blocks[i].StartingTrackPosition + 25));
							i++;
						}
						else
						{
							break;
						}
					}
				}
			}
		}

		private static RouteData currentRouteData;
		private static List<MechanikObject> AvailableObjects = new List<MechanikObject>();
		private static List<MechanikTexture> AvailableTextures = new List<MechanikTexture>();
		private static string RouteFolder;

		internal static void ParseRoute(string routeFile)
		{
			if (!System.IO.File.Exists(routeFile))
			{
				return;
			}
			AvailableObjects = new List<MechanikObject>();
			AvailableTextures = new List<MechanikTexture>();
			currentRouteData = new RouteData();
			//Load texture list
			RouteFolder = System.IO.Path.GetDirectoryName(routeFile);
			string tDat = OpenBveApi.Path.CombineFile(RouteFolder, "tekstury.dat");
			if (!System.IO.File.Exists(tDat))
			{
				return;
			}
			LoadTextureList(tDat);
			double previousTrackPosition = 0;
			string[] routeLines = System.IO.File.ReadAllLines(routeFile);
			double yOffset = 0.0;
			for (int i = 0; i < routeLines.Length; i++)
			{
				int j = routeLines[i].IndexOf(@"//", StringComparison.Ordinal);
				if (j != -1)
				{
					//Split out comments
					routeLines[i] = routeLines[i].Substring(j, routeLines[i].Length - j);
				}
				if (String.IsNullOrWhiteSpace(routeLines[i]))
				{
					continue;
				}
				routeLines[i] = Regex.Replace(routeLines[i], @"\s+", " ");
				string[] Arguments = routeLines[i].Trim().Split(null);
				double trackPosition, scaleFactor;
				int Idx, blockIndex, textureIndex;
				switch (Arguments[0].ToLowerInvariant())
				{
					case "'s":
						/*
						 * PERPENDICULAR PLANE OBJECTS
						 * => Track Position
						 * => Top Left X
						 * =>          Y
						 * =>          Z
						 * => Scale factor (200px in image == 1m at factor 1)
						 *
						 */
						double X, Y, Z;
						if (!double.TryParse(Arguments[1], out trackPosition))
						{
							//Add message
							continue;
						}
						if (!double.TryParse(Arguments[2], out X))
						{
							//Add message
							continue;
						}
						if (!double.TryParse(Arguments[3], out Y))
						{
							//Add message
							continue;
						}
						Y = -Y;
						if (!double.TryParse(Arguments[4], out Z))
						{
							//Add message
							continue;
						}
						if (!double.TryParse(Arguments[5], out scaleFactor))
						{
							//Add message
							continue;
						}
						if (!int.TryParse(Arguments[6], out textureIndex))
						{
							continue;
						}
						//Divide the track position, X and Y by 200, as Mechanik uses a 200px per meter scale factor
						trackPosition /= 200.0;
						Vector3 topLeft = new Vector3(X / 200, Y / 200, Z / 200);
						if (textureIndex == 50)
						{
							int t = 0;
							t++;
						}
						Idx = CreatePerpendicularPlane(tDat, topLeft, scaleFactor, textureIndex, false);
						blockIndex = currentRouteData.FindBlock(trackPosition);
						
						currentRouteData.Blocks[blockIndex].Objects.Add(new RouteObject(Idx, new Vector3(0,0,0)));
						break;
					case "#t":
					case "#t_p":
					case "#t_prz":
						yOffset -= 0.001;
						/*
						 * HORIZONTAL PLANE OBJECTS
						 * => Track Position
						 * => Number of Points (3,4, or 5)
						 * => 5x Point declarations (X,Y,Z)
						 * => 6 unused
						 * => Point for beginning of texture (?Top left equivilant?)
						 * => Wrap W (?)
						 * => Wrap H (?)
						 * => Texture scale (Number of repetitions?)
						 * => Texture IDX
						 * => Furthest point: Determines when this vanishes (When the cab passes?)
						 */
						int numPoints, firstPoint;
						if (!double.TryParse(Arguments[1], out trackPosition))
						{
							//Add message
							continue;
						}
						if (!int.TryParse(Arguments[2], out numPoints))
						{
							//Add message
							continue;
						}
						if (numPoints < 3 || numPoints > 5)
						{
							//Add message
							continue;
						}
						int v = 0;
						List<Vector3> points = new List<Vector3>();
						Vector3 currentPoint = new Vector3();
						double offset = 0;
						for (int p = 3; p < 24; p++)
						{
							switch (v)
							{
								case 0:
									if (!double.TryParse(Arguments[p], out currentPoint.X))
									{
										//Add message
									}
									currentPoint.X /= 200;
									break;
								case 1:
									if (!double.TryParse(Arguments[p], out currentPoint.Y))
									{
										//Add message
									}
									currentPoint.Y /= 200;
									currentPoint.Y = -currentPoint.Y;
									currentPoint.Y += yOffset;
									break;
								case 2:
									if (!double.TryParse(Arguments[p], out currentPoint.Z))
									{
										//Add message
									}
									currentPoint.Z /= 200;
									if (points.Count > 0 && points.Count < numPoints)
									{
										if (points.Count == 1)
										{
											offset = Math.Min(currentPoint.Z, points[points.Count - 1].Z);
										}
										else
										{
											offset = Math.Min(currentPoint.Z, offset);
										}
									}
									break;
							}
							if (v < 2)
							{
								v++;
							}
							else
							{
								points.Add(currentPoint);
								v = 0;
							}
						}
						if (!int.TryParse(Arguments[24], out firstPoint))
						{
							//Add message
							continue;
						}
						if (firstPoint != 0)
						{
							firstPoint -= 1;
						}
						double sx;
						double sy;
						if (!double.TryParse(Arguments[26], out sx))
						{
							//Add message
							continue;
						}
						if (!double.TryParse(Arguments[25], out sy))
						{
							//Add message
							continue;
						}
						if (!double.TryParse(Arguments[27], out scaleFactor))
						{
							//Add message
							continue;
						}
						if (!int.TryParse(Arguments[28], out textureIndex))
						{
							//Add message
							continue;
						}
						List<Vector3> sortedPoints = new List<Vector3>();
						/*
						 * Pull out the points making up our face
						 */
						for (int k = 0; k < numPoints; k++)
						{
							sortedPoints.Add(points[k]);
						}
						trackPosition /= 200.0;

						switch (Arguments[0].ToLowerInvariant())
						{
							case "#t_prz":
								//FREE, transparent
								Idx = CreateHorizontalObject(tDat, sortedPoints, firstPoint, scaleFactor, sx, sy, textureIndex, true, false);
								break;
							case "#t_p":
								//HORIZONTAL, no transparency
								Idx = CreateHorizontalObject(tDat, sortedPoints, firstPoint, scaleFactor, sx, sy, textureIndex, false, true);
								break;
							case "#t":
								Idx = CreateHorizontalObject(tDat, sortedPoints, firstPoint, scaleFactor, sx, sy, textureIndex, false, false);
								break;
							default:
								//never hit
								Idx = -1;
								break;
						}
						
						blockIndex = currentRouteData.FindBlock(trackPosition);
						
						currentRouteData.Blocks[blockIndex].Objects.Add(new RouteObject(Idx, new Vector3(0, 0, 0)));
						break;
				}

			}
			currentRouteData.Blocks.Sort((x, y) => x.StartingTrackPosition.CompareTo(y.StartingTrackPosition));
			currentRouteData.CreateMissingBlocks();
			ProcessRoute();
		}

		private static void ProcessRoute()
		{
			int bt = -1;
			string f = OpenBveApi.Path.CombineFile(RouteFolder, "obloczki.bmp");
			if (System.IO.File.Exists(f))
			{
				bt = TextureManager.RegisterTexture(f, new Color24(0, 0, 0), 0, TextureManager.TextureWrapMode.Repeat, TextureManager.TextureWrapMode.ClampToEdge, false);
			}
			World.CurrentBackground = new World.Background(bt, 2, false);
			World.TargetBackground = World.CurrentBackground;
			Vector3 Position = new Vector3(0.0, 0.0, 0.0);
			Vector2 Direction = new Vector2(0.0, 1.0);
			TrackManager.CurrentTrack = new TrackManager.Track();
			TrackManager.CurrentTrack.Elements = new TrackManager.TrackElement[] { };
			double CurrentSpeedLimit = double.PositiveInfinity;
			int CurrentRunIndex = 0;
			int CurrentFlangeIndex = 0;
			TrackManager.CurrentTrack.Elements = new TrackManager.TrackElement[256];
			int CurrentTrackLength = 0;
			int PreviousFogElement = -1;
			int PreviousFogEvent = -1;
			for (int i = 0; i < currentRouteData.Blocks.Count; i++)
			{
				double StartingDistance = (double)i * 25;
				double EndingDistance = StartingDistance + 25;	
				// normalize
				World.Normalize(ref Direction.X, ref Direction.Y);
				// track
				TrackManager.TrackElement WorldTrackElement = new TrackManager.TrackElement(currentRouteData.Blocks[i].StartingTrackPosition);

				int n = CurrentTrackLength;
				if (n >= TrackManager.CurrentTrack.Elements.Length)
				{
					Array.Resize<TrackManager.TrackElement>(ref TrackManager.CurrentTrack.Elements, TrackManager.CurrentTrack.Elements.Length << 1);
				}
				TrackManager.CurrentTrack.Elements[n] = WorldTrackElement;
				TrackManager.CurrentTrack.Elements[n].WorldPosition = Position;
				TrackManager.CurrentTrack.Elements[n].WorldDirection = Vector3.GetVector3(Direction, 0);
				TrackManager.CurrentTrack.Elements[n].WorldSide = new Vector3(Direction.Y, 0.0, -Direction.X);
				TrackManager.CurrentTrack.Elements[n].WorldUp = Vector3.Cross(TrackManager.CurrentTrack.Elements[n].WorldDirection, TrackManager.CurrentTrack.Elements[n].WorldSide);
				CurrentTrackLength++;
				
				double TrackYaw = Math.Atan2(Direction.X, Direction.Y);
				World.Transformation GroundTransformation = new World.Transformation(TrackYaw, 0.0, 0.0);
				World.Transformation TrackTransformation = new World.Transformation(TrackYaw, 0.0, 0.0);
				World.Transformation NullTransformation = new World.Transformation(0.0, 0.0, 0.0);
				// curves
				double a = 0.0;
				double c = 25.0;
				double h = 0.0;
				if (WorldTrackElement.CurveRadius != 0.0)
				{
					double d = 25.0;
					double r = WorldTrackElement.CurveRadius;
					double b = d / Math.Abs(r);
					c = Math.Sqrt(2.0 * r * r * (1.0 - Math.Cos(b)));
					a = 0.5 * (double)Math.Sign(r) * b;
					World.Rotate(ref Direction, Math.Cos(-a), Math.Sin(-a));
				}
				for (int j = 0; j < currentRouteData.Blocks[i].Objects.Count; j++)
				{
					World.Transformation RailTransformation = new World.Transformation(TrackTransformation, 0.0, 0.0, 0.0);
					ObjectManager.CreateObject(AvailableObjects[currentRouteData.Blocks[i].Objects[j].objectIndex].Object, Position, NullTransformation, NullTransformation, ObjectManager.ObjectDisposalMode.Mechanik, StartingDistance, EndingDistance, 100, StartingDistance);
				}
				// finalize block
				Position.X += Direction.X * c;
				Position.Y += h;
				Position.Z += Direction.Y * c;
				if (a != 0.0)
				{
					World.Rotate(ref Direction, Math.Cos(-a), Math.Sin(-a));
				}
			}
			Array.Resize<TrackManager.TrackElement>(ref TrackManager.CurrentTrack.Elements, CurrentTrackLength);
			TrackManager.CurrentTrack.Elements[CurrentTrackLength -1].Events = new TrackManager.GeneralEvent[] { new TrackManager.TrackEndEvent(25) };
		}

		private static int CreateHorizontalObject(string TdatPath, List<Vector3> Points, int firstPoint, double scaleFactor, double sx, double sy, int textureIndex, bool transparent, bool horizontal)
		{
			MechanikTexture t = new MechanikTexture();
			for (int i = 0; i < AvailableTextures.Count; i++)
			{
				if (AvailableTextures[i].TextureIndex == textureIndex)
				{
					t = AvailableTextures[i];
				}
			}
			MechanikObject o = new MechanikObject();
			o.TopLeft = new Vector3();
			o.TextureIndex = textureIndex;
			List<string> s = new List<string>();
			s.Add("[MeshBuilder]");
			Vector3 min = new Vector3(Points[0].X, Points[0].Y, Points[0].Z);
			Vector3 max = new Vector3(Points[0].X, Points[0].Y, Points[0].Z);
			for (int i = 0; i < Points.Count; i++)
			{
				s.Add("Vertex "+ Points[i]);
				if (Points[i].X > min.X)
				{
					min.X = Points[i].X;
					
				}
				if (Points[i].X < max.X)
				{
					max.X = Points[i].X;
				}
				if (Points[i].Y > min.Y)
				{
					min.Y = Points[i].Y;
				}
				if (Points[i].Y < max.Y)
				{
					max.Y = Points[i].Y;
				}
				if (Points[i].Z > min.Z)
				{
					min.Z = Points[i].Z;
				}
				if (Points[i].Z < max.Z)
				{
					max.Z = Points[i].Z;
				}
			}
			//size of face
			//BUG: Doesn't take into account diagonal faces or anything of that nature
			Vector3 faceSize = max - min;
			string f = "Face2 ";
			for (int i = 0; i < Points.Count; i++)
			{
				f += i + ",";
			}
			s.Add(f);
			s.Add("[Texture]");
			s.Add("Load " + t.Texture);
			//TEMP
			double tc1 = sx, tc2 = sy;
			if (horizontal)
			{
				tc1 = faceSize.X / (t.Width * sy * scaleFactor);
				tc2 = faceSize.Z / (t.Height * sx * scaleFactor);
			}
			else
			{
				if (faceSize.X == 0)
				{
					tc1 = faceSize.Z / (t.Width * sx * scaleFactor);
					tc2 = faceSize.Y / (t.Height * sy * scaleFactor);
				}
				else if (faceSize.Y == 0)
				{
					//BUG: Not sure why this needs negating at the minute.....
					tc1 = -(faceSize.X / (t.Width * sx * scaleFactor));
					tc2 = faceSize.Z / (t.Height * sy * scaleFactor);
				}
				else if (faceSize.Z == 0)
				{
					tc1 = faceSize.X / (t.Width * sx * scaleFactor);
					tc2 = faceSize.Y / (t.Height * sy * scaleFactor);
				}
			}
			if (t.Texture.ToLowerInvariant() == "pta.bmp")
			{
				int b = 0;
				b++;
			}
			for (int i = 0; i < Points.Count; i++)
			{
				if (firstPoint >= Points.Count)
				{
					firstPoint = 0;
				}
				switch (i)
				{
					case 0:
						s.Add("Coordinates " + firstPoint + ",0,0");
						break;
					case 1:
						s.Add("Coordinates "+ firstPoint + ",0," + tc2);
						break;
					case 2:
						s.Add("Coordinates " + firstPoint + "," + tc1 +"," + tc2);
						break;
					case 3:
						s.Add("Coordinates " + firstPoint + "," + tc1 + ",0");
						break;
				}
				firstPoint++;
			}
			if (transparent)
			{
				s.Add("Transparent 0,0,0");
			}
			o.Object = CsvB3dObjectParser.ReadObject(TdatPath, Encoding.ASCII, ObjectManager.ObjectLoadMode.Normal, false, false, s.ToArray());
			AvailableObjects.Add(o);
			return AvailableObjects.Count - 1;
		}

		private static int CreatePerpendicularPlane(string TdatPath, Vector3 topLeft, double scaleFactor, int textureIndex, bool transparent)
		{
			for (int i = 0; i < AvailableObjects.Count; i++)
			{
				if (AvailableObjects[i].TopLeft == topLeft && AvailableObjects[i].ScaleFactor == scaleFactor && AvailableObjects[i].TextureIndex == textureIndex)
				{
					return i;
				}
			}
			MechanikTexture t = new MechanikTexture();
			for (int i = 0; i < AvailableTextures.Count; i++)
			{
				if (AvailableTextures[i].TextureIndex == textureIndex)
				{
					t = AvailableTextures[i];
					break;
				}
			}
			MechanikObject o = new MechanikObject();
			o.TopLeft = new Vector3(0,0,0);
			o.TextureIndex = textureIndex;	
			//BUG: Not entirely sure why multiplying W & H by 5 makes this work....
			List<string> s = new List<string>();
			s.Add("[MeshBuilder]");
			s.Add("Vertex " + topLeft + ", -1,1,0");
			s.Add("Vertex " + (topLeft.X + (t.Width * 5)) + "," + topLeft.Y + "," + topLeft.Z); //upper right
			s.Add("Vertex " + (topLeft.X + (t.Width * 5)) + "," + (topLeft.Y - (t.Height * 5)) + "," + topLeft.Z); //bottom right
			s.Add("Vertex " + topLeft.X + ","+ (topLeft.Y - (t.Height * 5)) + "," + topLeft.Z); //bottom left
			//Possibly change to Face, check this though (Remember that Mechanik was restricted to the cab, wheras we are not)
			s.Add("Face2 0,1,2,3");
			s.Add("[Texture]");
			s.Add("Load " + t.Texture);
			s.Add("Coordinates 1,1,0");
			s.Add("Coordinates 2,1,1");
			s.Add("Coordinates 3,0,1");
			s.Add("Coordinates 0,0,0");
			s.Add("Transparent 0,0,0");
			o.Object = CsvB3dObjectParser.ReadObject(TdatPath, Encoding.ASCII, ObjectManager.ObjectLoadMode.Normal, false, false, s.ToArray());
			AvailableObjects.Add(o);
			return AvailableObjects.Count - 1;
		}

		private struct MechanikObject
		{
			internal MechnikObjectType Type;
			internal Vector3 TopLeft;
			internal double ScaleFactor;
			internal int TextureIndex;
			internal ObjectManager.StaticObject Object;
		}

		private enum MechnikObjectType
		{
			Perpendicular = 0,
			Horizontal = 1
		}

		private struct MechanikTexture
		{
			internal string Path;
			internal string Texture;
			internal int TextureIndex;
			internal double Width;
			internal double Height;
			internal MechanikTexture(string p, string s, int i)
			{
				Path = p;
				Texture = s;
				TextureIndex = i;
				Bitmap b = new Bitmap(p);
				this.Width = b.Width / 200.0;
				this.Height = b.Height / 200.0;
			}
		}

		

		private static void LoadTextureList(string tDat)
		{
			string[] textureLines = System.IO.File.ReadAllLines(tDat);
			for (int i = 0; i < textureLines.Length; i++)
			{
				int j = textureLines[i].IndexOf(@"//", StringComparison.Ordinal);
				if (j != -1)
				{
					//Split out comments
					textureLines[i] = textureLines[i].Substring(j, textureLines[i].Length - 1);
				}
				if (String.IsNullOrWhiteSpace(textureLines[i]))
				{
					continue;
				}
				textureLines[i] = Regex.Replace(textureLines[i], @"\s+", " ");
				int k = 0;
				string s = null;
				for (int l = 0; l < textureLines[i].Length; l++)
				{
					if (!char.IsDigit(textureLines[i][l]))
					{
						string str = textureLines[i].Substring(0, l);
						k = int.Parse(textureLines[i].Substring(0, l));
						s = textureLines[i].Substring(l, textureLines[i].Length - l).Trim();
						break;
					}
				}
				if (!String.IsNullOrWhiteSpace(s))
				{
					string path = Path.CombineFile(System.IO.Path.GetDirectoryName(tDat), s);
					if (System.IO.File.Exists(path))
					{
						MechanikTexture t = new MechanikTexture(path, s, k);
						AvailableTextures.Add(t);
					}

				}

			}
		}
	}
}
