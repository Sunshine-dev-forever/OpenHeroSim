using System.Collections.Generic;
using Item;

namespace Pawn
{
	public interface IPawnInventory
	{
		//Returns all items in the pawn's inventory, including equipment
		List<IItem> GetAllItems();
		List<IItem> GetAllItemsInBag();
		List<IItem> GetAllEquippedItems();
		bool AddItem(IItem item);
		bool RemoveItem(IItem item);
		List<IItem> EmptyAllItems();
		Equipment? GetWornEquipment(EquipmentType type);
		void WearEquipment(Equipment equipment);
		double GetTotalEquiptmentDefense();
		double GetTotalEquiptmentDamage();
		void QueueFree();
	}
}