namespace TomcatMono.UI.Controls {
	public abstract partial class Control {

		private void SetParent(Control? value) {
			if (_parent == value) return;
			if (value == this) throw new ControlHierarchyException("Control cannot be its own parent.");

			Control? p = value;
			while (p != null) {
				if (p == this)
					throw new ControlHierarchyException("Recursive parent chain detected.");
				p = p.Parent;
			}

			if (_parent != null)
				_parent.RemoveChild(this);

			_parent = value;

			if (_parent != null)
				_parent.AddChild(this);
		}

		private void AddChild(Control? candidate) {
			if (candidate == null) return;
			if (_children.Contains(candidate)) return;

			if (candidate._parent != null && candidate._parent != this)
				candidate._parent.RemoveChild(candidate);

			_children.Add(candidate);
		}

		private void RemoveChild(Control? child) {
			if (child == null) return;
			_children.Remove(child);
		}
	}
}
