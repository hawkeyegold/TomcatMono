namespace TomcatMono {
	public sealed class ControlHierarchyException : Exception {
		public ControlHierarchyException(string message) : base(message) { }
	}

	public sealed class TomcatUIVirtualBoundsException : Exception {
		public TomcatUIVirtualBoundsException(string message) : base(message) { }
	}
}
