using System.Reflection.Emit;
using System;
using Serilog;
using System.Collections.Generic;
using Pawn.Tasks;
using Pawn.Action;
using Pawn.Goal;
using Godot;
using Pawn.Controller;

namespace Pawn
{
	//What exactally do I want to be able to setup?
	//I want to change brain-modules
	//I want to change stats (pawn informaiton class that does not exist yet)

	public class PawnControllerBuilder
	{
		KdTreeController kdTreeController;
		PawnController pawn;
		//NavigationServer navigationServer = null!;

		public PawnControllerBuilder(Node parent, KdTreeController _kdTreeController, Navigation navigation){
			PackedScene pawnScene = GD.Load<PackedScene>("res://scenes/pawn/pawn.tscn");
			pawn = pawnScene.Instance<PawnController>();
			parent.AddChild(pawn);

			kdTreeController = _kdTreeController;
			kdTreeController.AddPawnToAllPawnList(pawn);

			pawn.MovementController.SetNavigation(navigation);
		}
		//TODO: in the future, perhaps the kdTreeController could be an option?
		public static PawnControllerBuilder Start(Node parent, KdTreeController _kdTreeController, Navigation navigation) {
			return new PawnControllerBuilder(parent, _kdTreeController, navigation);
		}

		public PawnControllerBuilder Location(Vector3 spawnLocation) {
			pawn.GlobalTransform = new Transform(pawn.GlobalTransform.basis, spawnLocation);
			return this;
		}

		//Appends to the list of goals, so most important goals should be added first
		public PawnControllerBuilder AddGoal(IPawnGoal goal) {
			pawn.PawnBrain.AddGoal(goal);
			return this;
		}

		public PawnControllerBuilder Weapon(Weapon weapon){
			pawn.SetWeapon(weapon);
			return this;
		}

		public PawnController Finish() {
			pawn.Setup(kdTreeController);
			return pawn;
		}

		public PawnControllerBuilder SetNoCombat(bool noCombat) {
			pawn.PawnBrain.noCombat = noCombat;
			return this;
		}

		public PawnControllerBuilder SetName(string name) {
			pawn.pawnName = name;
			return this;
		}
	}
}