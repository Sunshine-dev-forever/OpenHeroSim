using System;
using System.Collections.Generic;
using Godot;
using Pawn.Action;

namespace Pawn.Targeting {
	public class StaticPointTargeting : ITargeting {
		private Vector3 Point {get;}
		private Predicate<Vector3> PointTester{get;}
		public StaticPointTargeting(Vector3 _point) {
			Point = _point;
			//If no predicate is specified then the point is assumed to always be valid
			PointTester = (Vector3 point) => {return true;};
		}

		public StaticPointTargeting(Vector3 _point, Predicate<Vector3> predicate) {
			Point = _point;
			PointTester = predicate;
		}
		public Vector3 GetTargetLocation() {
			return Point;
		}
		public bool IsValid { get {
			return PointTester(Point);
		} }

	}
}