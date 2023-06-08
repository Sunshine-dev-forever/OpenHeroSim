using Godot;
using System;
using System.Collections.Generic;
using Serilog;
using Pawn.Action;
using System.Threading.Tasks;
using Pawn;
using Pawn.Goal;
using Item;
using Pawn.Action.Ability;
using Util;
using System.Collections;
using System.Linq;

namespace Worlds.TowerDefense {
	public partial class TowerDefenseRunner : Node
	{
		private List<IPawnController> pawns = new List<IPawnController>();
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

		public override void _Process(double delta){

		}

		private IPawnController CreatePawn(){
			NavigationRegion3D navigation = GetNode<NavigationRegion3D>("/root/Node3D/NavigationRegion3D");
			
			Vector3 location = this.GetNode<Node3D>("Spawn").GlobalTransform.Origin;
			List<Node3D> waypoints = new List<Node3D>();
			//I need to create a library for selecting and filtering children of a node
			waypoints.Add(this.GetNode<Node3D>("WayPoint1"));
			waypoints.Add(this.GetNode<Node3D>("WayPoint2"));
			waypoints.Add(this.GetNode<Node3D>("WayPoint3"));
			
			return PawnControllerBuilder.Start(this, kdTreeController, navigation)
								.AddGoal(new WaypointGoal(waypoints))
								.Location(location)
								.Finish();		
		}

		private void CreateTowers(){
			NavigationRegion3D navigation = GetNode<NavigationRegion3D>("/root/Node3D/NavigationRegion3D");
			
			List<Node3D> TowerSpawns = new List<Node3D>();
			//I need to create a library for selecting and filtering children of a node
			TowerSpawns.Add(this.GetNode<Node3D>("SpawnTower1"));
			TowerSpawns.Add(this.GetNode<Node3D>("SpawnTower2"));
			TowerSpawns.Add(this.GetNode<Node3D>("SpawnTower3"));
			
			foreach (Node3D location in TowerSpawns) {
				PawnControllerBuilder.Start(this, kdTreeController, navigation)
								.AddGoal(new DefendSelfGoal())
								.AddAbility(AbilityDefinitions.THROW_ABILITY)
								.Faction("Towers")
								.AddItem(CreateThrowable())
								.Location(location.GlobalTransform.Origin)
								.Finish();	
			}
		}

		public Throwable CreateThrowable() {
			Node3D spear = CustomResourceLoader.LoadMesh(ResourcePaths.DJERID);
			return new Throwable(spear, 60);
		}

	}

}
