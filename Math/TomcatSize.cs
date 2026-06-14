using System;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Globalization;

namespace TomcatMono.Math {
	/// <summary>
	/// Represents a size defined by a width and height.
	/// </summary>
	public readonly struct TomcatSize : IEquatable<TomcatSize> {
		public int Width { get; }
		public int Height { get; }

		public TomcatSize(int width, int height) {
			Width = width;
			Height = height;
		}

		[Pure] public bool IsEmpty => Width == 0 && Height == 0;

		// Factory methods
		public static TomcatSize Empty => new(0, 0);

		// Equality members
		public bool Equals(TomcatSize other) => Width == other.Width && Height == other.Height;

		public override int GetHashCode() => HashCode.Combine(Width, Height);
		public override bool Equals(object? obj) => obj is TomcatSize other && Equals(other);

		public static bool operator ==(TomcatSize left, TomcatSize right) => left.Equals(right);
		public static bool operator !=(TomcatSize left, TomcatSize right) => !left.Equals(right);

		// Basic arithmetic operators (Optional but highly recommended)
		public static TomcatSize operator +(TomcatSize left, TomcatSize right) => new(left.Width + right.Width, left.Height + right.Height);
		public static TomcatSize operator -(TomcatSize left, TomcatSize right) => new(left.Width - right.Width, left.Height - right.Height);

		// Formatting
		public override string ToString() => $"{{Width={Width}, Height={Height}}}";
	}
}