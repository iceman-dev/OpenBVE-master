#pragma warning disable 0660 // Defines == or != but does not override Object.Equals
#pragma warning disable 0661 // Defines == or != but does not override Object.GetHashCode

using OpenBveApi.Math;

namespace OpenBveApi.Geometry
{
	/// <summary>Represents a vertex consisting of 3D coordinates and 2D texture coordinates.</summary>
	public struct Vertex
	{
		/// <summary>The spatial coordinates of the vertex</summary>
		public Vector3 Coordinates;
		/// <summary>The texture coordinates of the vertex</summary>
		public Vector2 TextureCoordinates;
		/// <summary>Creates a new vertex from a set of X, Y and Z coordinates</summary>
		public Vertex(double X, double Y, double Z)
		{
			this.Coordinates = new Vector3(X, Y, Z);
			this.TextureCoordinates = new Vector2(0.0f, 0.0f);
		}
		/// <summary>Creates a new vertex with the given coordinates and texture coordinates</summary>
		public Vertex(Vector3 Coordinates, Vector2 TextureCoordinates)
		{
			this.Coordinates = Coordinates;
			this.TextureCoordinates = TextureCoordinates;
		}
		/// <summary>Checks whether two verticies are equal</summary>
		public static bool operator ==(Vertex A, Vertex B)
		{
			if (A.Coordinates.X != B.Coordinates.X | A.Coordinates.Y != B.Coordinates.Y | A.Coordinates.Z != B.Coordinates.Z) return false;
			if (A.TextureCoordinates.X != B.TextureCoordinates.X | A.TextureCoordinates.Y != B.TextureCoordinates.Y) return false;
			return true;
		}
		/// <summary>Checks whether two verticies are NOT equal</summary>
		public static bool operator !=(Vertex A, Vertex B)
		{
			if (A.Coordinates.X != B.Coordinates.X | A.Coordinates.Y != B.Coordinates.Y | A.Coordinates.Z != B.Coordinates.Z) return true;
			if (A.TextureCoordinates.X != B.TextureCoordinates.X | A.TextureCoordinates.Y != B.TextureCoordinates.Y) return true;
			return false;
		}
	}

	/// <summary>Represents a reference to a vertex and the normal to be used for that vertex.</summary>
	public struct MeshFaceVertex
	{
		/// <summary>A reference to an element in the Vertex array of the contained Mesh structure.</summary>
		public ushort Index;
		/// <summary>The normal to be used at the vertex.</summary>
		public Vector3 Normal;
		/// <summary>Creates a new element using the specified index.</summary>
		public MeshFaceVertex(int Index)
		{
			this.Index = (ushort)Index;
			this.Normal = new Vector3(0.0f, 0.0f, 0.0f);
		}
		/// <summary>Creates a new element using the specified index and normal.</summary>
		public MeshFaceVertex(int Index, Vector3 Normal)
		{
			this.Index = (ushort)Index;
			this.Normal = Normal;
		}
		// operators
		/// <summary>Checks whether two MeshFaceVertex elements are equal.</summary>
		public static bool operator ==(MeshFaceVertex A, MeshFaceVertex B)
		{
			if (A.Index != B.Index) return false;
			if (A.Normal.X != B.Normal.X) return false;
			if (A.Normal.Y != B.Normal.Y) return false;
			if (A.Normal.Z != B.Normal.Z) return false;
			return true;
		}
		/// <summary>Checks whether two MeshFaceVertex elements NOT equal.</summary>
		public static bool operator !=(MeshFaceVertex A, MeshFaceVertex B)
		{
			if (A.Index != B.Index) return true;
			if (A.Normal.X != B.Normal.X) return true;
			if (A.Normal.Y != B.Normal.Y) return true;
			if (A.Normal.Z != B.Normal.Z) return true;
			return false;
		}
	}

	/// <summary>Represents a face consisting of vertices and material attributes.</summary>
	public struct MeshFace
	{
		/// <summary>The array of vertices and the corresponding normals</summary>
		public MeshFaceVertex[] Vertices;
		/// <summary>A reference to an element in the Material array of the containing Mesh structure.</summary>
		public ushort Material;
		/// <summary>A bit mask combining constants of the MeshFace structure.</summary>
		public byte Flags;
		/// <summary>Creates a new MeshFace using the specified vertex indicies</summary>
		public MeshFace(int[] Vertices)
		{
			this.Vertices = new MeshFaceVertex[Vertices.Length];
			for (int i = 0; i < Vertices.Length; i++)
			{
				this.Vertices[i] = new MeshFaceVertex(Vertices[i]);
			}
			this.Material = 0;
			this.Flags = 0;
		}
		/// <summary>Flips the MeshFace</summary>
		public void Flip()
		{
			if ((this.Flags & FaceTypeMask) == FaceTypeQuadStrip)
			{
				for (int i = 0; i < this.Vertices.Length; i += 2)
				{
					MeshFaceVertex x = this.Vertices[i];
					this.Vertices[i] = this.Vertices[i + 1];
					this.Vertices[i + 1] = x;
				}
			}
			else
			{
				int n = this.Vertices.Length;
				for (int i = 0; i < (n >> 1); i++)
				{
					MeshFaceVertex x = this.Vertices[i];
					this.Vertices[i] = this.Vertices[n - i - 1];
					this.Vertices[n - i - 1] = x;
				}
			}
		}
		public const int FaceTypeMask = 7;
		public const int FaceTypePolygon = 0;
		public const int FaceTypeTriangles = 1;
		public const int FaceTypeTriangleStrip = 2;
		public const int FaceTypeQuads = 3;
		public const int FaceTypeQuadStrip = 4;
		public const int Face2Mask = 8;
	}
	
}