using System.Globalization;
using Godot;
using System;
using Serilog;
using Pawn.Tasks;
using Pawn;
using System.Collections.Generic;

public class ItemContainer : Spatial, IInteractable {

	public ItemContainer(Weapon _containedWeapon, Spatial _mesh) {
		ContainedWeapon = _containedWeapon;
		Mesh = _mesh;
		this.AddChild(Mesh);
	}
	public bool IsInstanceValid() {
		return IsInstanceValid(this);
	}
	public void Delete() {
		ContainedWeapon.Delete();
		Mesh.QueueFree();
		this.QueueFree();
	}
	public Weapon ContainedWeapon;
	public Spatial Mesh;
}