
using System;

namespace Pawn.Item {
	public class InventoryFullException : Exception {
		public InventoryFullException(string message): base(message) {}
	}
}