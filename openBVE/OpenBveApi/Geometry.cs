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

}