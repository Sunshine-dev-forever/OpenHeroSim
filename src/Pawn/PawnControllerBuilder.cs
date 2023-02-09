using System.Reflection.Emit;
using System;
using Serilog;
using System.Collections.Generic;
using Pawn.Tasks;
using Pawn.Action;
using Pawn.Goal;
using Godot;
using Pawn.Controller;
using Pawn.Item;
using Pawn.Action.Ability;
using Util;
namespace Pawn
{
	//What exactally do I want to be able to setup?
	//I want to change brain-modules
	//I want to change stats (pawn informaiton class that does not exist yet)

	public class PawnControllerBuilder
	{
		KdTreeController kdTreeController;
		PawnController pawn;

		private static string PAWN_RIG_RESOURCE_FILE_DEFAULT = ResourcePaths.PAWN_MODEL;
		private string pawnRigResourceFile = PAWN_RIG_RESOURCE_FILE_DEFAULT;
		public static PawnController CreateTrainingDummy(Vector3 location, 
														Node parent, 
														KdTreeController _kdTreeController, 
														Navigation navigation) {
			return PawnControllerBuilder.Start(parent, _kdTreeController, navigation)
										.Location(location)
										.AddGoal(new DebugGoal())
										.SetName("Training Dummy")
										.Finish();
		}

		public PawnControllerBuilder(Node parent, KdTreeController _kdTreeController, Navigation navigation){
			//TODO: my custom resource loader should support return types other than spatial...
			//But then how do I create defaults?
			pawn = ResourceLoader.Load<PackedScene>(ResourcePaths.PAWN_SCENE).Instance<PawnController>();
			parent.AddChild(pawn);

			kdTreeController = _kdTreeController;
			kdTreeController.AddInteractable(pawn);

			pawn.MovementController.SetNavigation(navigation);
		}
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

		public PawnControllerBuilder WearEquipment(Equipment equipment){
			pawn.PawnInventory.WearEquipment(equipment);
			return this;
		}

		public PawnController Finish() {
			pawn.VisualController.SetPawnRig(pawnRigResourceFile);
			pawn.Setup(kdTreeController);
			return pawn;
		}

		public PawnControllerBuilder SetName(string name) {
			pawn.PawnInformation.Name = name;
			return this;
		}

		public PawnControllerBuilder AddConsumable(Consumable item) {
			pawn.PawnInventory.inventory.Add(item);
			return this;
		}

		public PawnControllerBuilder DealDamage(double damage) {
			pawn.TakeDamage(damage);
			return this;
		}

		//sets the resource file for the pawnMesh
		public PawnControllerBuilder SetPawnRig(string filename) {
			pawnRigResourceFile = filename;
			return this;
		}

		public PawnControllerBuilder AddAbility(IAbility ability) {
			pawn.PawnInformation.AddAbility(ability);
			return this;
		}
	}
}