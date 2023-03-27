using Godot;
using System;
using System.Collections.Generic;
using Serilog;
using Pawn.Action;
using System.Threading.Tasks;
using Pawn;
using Pawn.Controller;
using Pawn.Goal;
using Pawn.Item;
using Pawn.Action.Ability;
using Util;
using System.Collections;
using System.Linq;

namespace Worlds.TowerDefense {
	public class TowerDefenseRunner : Node
	{
		private List<PawnController> pawns = new List<PawnController>();
		private KdTreeController kdTreeController = null!; 
		public override void _Ready()
		{
			kdTreeController = new KdTreeController();
			this.AddChild(kdTreeController);
		}
		
		public override void _Input(InputEvent input) {
			if(input.IsActionPressed("mouse_left_click")) {
				//Do nothing... for now
			} else if(input.IsActionPressed("ui_left")) {
				CreatePawn();
			} else if (input.IsActionPressed("ui_right")) {
				CreateTowers();
			} else if(input.IsActionPressed("ui_up")) {

			} else if(input.IsActionPressed("ui_down")){

			}
		}

		public override void _Process(float  delta){

		}

		private PawnController CreatePawn(){
			Navigation navigation = GetNode<Navigation>("/root/Spatial/Navigation");
			
			Vector3 location = this.GetNode<Spatial>("Spawn").GlobalTransform.origin;
			List<Spatial> waypoints = new List<Spatial>();
			//I need to create a library for selecting and filtering children of a node
			waypoints.Add(this.GetNode<Spatial>("WayPoint1"));
			waypoints.Add(this.GetNode<Spatial>("WayPoint2"));
			waypoints.Add(this.GetNode<Spatial>("WayPoint3"));
			
			return PawnControllerBuilder.Start(this, kdTreeController, navigation)
								.AddGoal(new WaypointGoal(waypoints))
								.AddAbility(new StabAbility())
								.Location(location)
								.Finish();		
		}

		private void CreateTowers(){
			Navigation navigation = GetNode<Navigation>("/root/Spatial/Navigation");
			
			List<Spatial> TowerSpawns = new List<Spatial>();
			//I need to create a library for selecting and filtering children of a node
			TowerSpawns.Add(this.GetNode<Spatial>("SpawnTower1"));
			TowerSpawns.Add(this.GetNode<Spatial>("SpawnTower2"));
			TowerSpawns.Add(this.GetNode<Spatial>("SpawnTower3"));
			
			foreach (Spatial location in TowerSpawns) {
				PawnControllerBuilder.Start(this, kdTreeController, navigation)
								.AddGoal(new DefendSelfGoal())
								.AddAbility(new ThrowAbility())
								.Faction("Towers")
								.AddItem(CreateThrowable())
								.Location(location.GlobalTransform.origin)
								.Finish();	
			}
		}

		public Throwable CreateThrowable() {
			Spatial spear = CustomResourceLoader.LoadMesh(ResourcePaths.DJERID);
			return new Throwable(spear, 60);
		}

	}

}
