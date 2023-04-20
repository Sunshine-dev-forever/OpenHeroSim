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
using Pawn.Targeting;

namespace Worlds.MainTest 
{
	public partial class MainTestRunner : Node
	{

		private KdTreeController kdTreeController = null!; 
		public override void _Ready()
		{
			kdTreeController = new KdTreeController();
			this.AddChild(kdTreeController);
		}
		
		public override void _Input(InputEvent input) {
			if(input.IsActionPressed("mouse_left_click")) {
				//CreateThrowableTester();
			} else if(input.IsActionPressed("ui_left")) {
				//CreateTestProjectile();
				NavigationRegion3D navigation = GetNode<NavigationRegion3D>("/root/Node3D/NavigationRegion3D");
			
				Vector3 location = new Vector3(4,0,4);
				PawnControllerBuilder.CreateTrainingDummy(location, this, kdTreeController, navigation);
			} else if (input.IsActionPressed("ui_right")) {
				CreatePawnInCenter();
			}
		}
		private double TimeSinceLastPawnCreation = 4;
		//lazy, bad coding
		private PawnController lastPawnSpawned = null!;
		public override void _Process(double delta)
		{
			TimeSinceLastPawnCreation += delta;
			if(TimeSinceLastPawnCreation > 5) {
				TimeSinceLastPawnCreation = 0;
				//lastPawnSpawned = CreatePawn();
			}
			
		}

		public PawnController CreateThrowableTester() {
			NavigationRegion3D navigation = GetNode<NavigationRegion3D>("/root/Node3D/NavigationRegion3D");
			
			Vector3 location = new Vector3(0,5,0);
			return PawnControllerBuilder.Start(this, kdTreeController, navigation)
								.AddGoal(new HealGoal())
								.AddGoal(new DefendSelfGoal())
								.AddGoal(new LootGoal())
								.AddGoal(new WanderGoal())
								.AddAbility(new ThrowAbility())
								.AddAbility(new StabAbility())
								.WearEquipment(GetRandomWeapon())
								.WearEquipment(GetHelmet())
								.AddItem(CreateThrowable())
								.Location(location)
								.Finish();		
		}

		public Throwable CreateThrowable() {
			Node3D spear = CustomResourceLoader.LoadMesh(ResourcePaths.DJERID);
			return new Throwable(spear, 60);
		}

		private void CreateTestProjectile() {
			Node3D spear = CustomResourceLoader.LoadMesh(ResourcePaths.DJERID);
			//I add an offset so the spear target's the pawns chest and not the pawn's feet
			Vector3 offset = new Vector3(0,1,0);
			ITargeting target = new InteractableTargeting(lastPawnSpawned, offset);
			Projectile projectile = new Projectile(spear, target, 50 );
			this.AddChild(projectile);
			projectile.GlobalTransform = new Transform3D(projectile.GlobalTransform.Basis, new Vector3(0,5,0));
		}

		private PawnController CreatePawn(){
			NavigationRegion3D navigation = GetNode<NavigationRegion3D>("/root/Node3D/NavigationRegion3D");
			
			Vector3 location = GenerateRandomVector();
			
			return PawnControllerBuilder.Start(this, kdTreeController, navigation)
								.AddGoal(new HealGoal())
								.AddGoal(new DefendSelfGoal())
								.AddGoal(new LootGoal())
								.AddGoal(new WanderGoal())
								.AddAbility(new StabAbility())
								.WearEquipment(GetRandomWeapon())
								.WearEquipment(GetHelmet())
								.Location(location)
								.Finish();		
		}

		private PawnController CreatePawnInCenter(){
			NavigationRegion3D navigation = GetNode<NavigationRegion3D>("/root/Node3D/NavigationRegion3D");
			
			Vector3 location = new Vector3(0,5,0);
			
			return PawnControllerBuilder.Start(this, kdTreeController, navigation)
								.AddGoal(new HealGoal())
								.AddGoal(new DefendSelfGoal())
								.AddGoal(new LootGoal())
								.AddGoal(new WanderGoal())
								.AddAbility(new StabAbility())
								.WearEquipment(GetRandomWeapon())
								.WearEquipment(GetHelmet())
								.Location(location)
								.Finish();		
		}

		private Equipment GetHelmet() {
			//"res://scenes/world_objects/box_helm.tscn"
			Node3D boxHelm = CustomResourceLoader.LoadMesh(ResourcePaths.BOX_HELM);
			Equipment equipment = new Equipment(boxHelm, EquipmentType.HEAD);
			equipment.Defense = 5;
			return equipment;
		}

		private Vector3 GenerateRandomVector() {
			Random rand = new Random();
			int rng = rand.Next(0,3);
			if(rng == 0) {
				return new Vector3(23,5,23);
			} else if (rng == 1) {
				return new Vector3(-23,5,23);
			} else if (rng == 2) {
				return new Vector3(-23, 5, -23);
			} else if (rng == 3) {
				return new Vector3(23, 5, -23);
			}
			return new Vector3();
		}

		private Equipment GetRandomWeapon() {
			Random rand = new Random();
			int rng = rand.Next(0,3);
			switch (rng) {
				case 0: return CreateSpearMelee();
				case 1: return CreateRustedDagger();
				case 2: return CreateIronSword();
				default:
				return CreateRustedDagger();
			}
		}
		private void Adhoc2(){

		}

		private PawnController CreateHealingPotionTester() {
			NavigationRegion3D navigation = GetNode<NavigationRegion3D>("/root/Node3D/NavigationRegion3D");
			return PawnControllerBuilder.Start(this, kdTreeController, navigation)
								.AddGoal(new HealGoal())
								.AddGoal(new WanderGoal())
								.Location(new Vector3(0,5,0))
								.WearEquipment(CreateLightSaber())
								.AddItem(CreateHealingPotion()).AddItem(CreateHealingPotion())
								.DealDamage(50)
								.Finish();
		}

		private void CreateItemContainer() {
			Node3D TreasureChestMesh = CustomResourceLoader.LoadMesh(ResourcePaths.TREASURE_CHEST);
			//The iron sword gets leaked when created like this
			List<IItem> items = new List<IItem>();
			items.Add(CreateHealingPotion());
			items.Add(CreateIronSword());
			ItemContainer itemContainer = new ItemContainer(items, TreasureChestMesh);
			this.AddChild(itemContainer);
			itemContainer.GlobalTransform = new Transform3D(itemContainer.GlobalTransform.Basis, new Vector3(0,1,0));
			kdTreeController.AddInteractable(itemContainer);
		}

		private Equipment CreateIronSword() {
			Node3D ironSword = CustomResourceLoader.LoadMesh(ResourcePaths.IRON_SWORD);
			Equipment equipment = new Equipment(ironSword, EquipmentType.HELD);
			equipment.Damage = 5;
			return equipment;
		}

		private Equipment CreateRustedDagger() {
			Node3D rustedDagger = CustomResourceLoader.LoadMesh(ResourcePaths.RUSTED_DAGGER);;
			Equipment equipment = new Equipment(rustedDagger, EquipmentType.HELD);
			equipment.Damage = 3;
			return equipment;
		}

		private Equipment CreateLightSaber() {
			Node3D lightSaber = CustomResourceLoader.LoadMesh(ResourcePaths.LIGHTSABER);
			Equipment equipment = new Equipment(lightSaber, EquipmentType.HELD);
			equipment.Damage = 10;
			return equipment;
		}
		private Equipment CreateSpearMelee() {
			Node3D spear = CustomResourceLoader.LoadMesh(ResourcePaths.DJERID);
			Equipment equipment = new Equipment(spear, EquipmentType.HELD);
			equipment.Damage = 40;
			return equipment;
		}

		private Consumable CreateHealingPotion() {
			Node3D healthPotion = CustomResourceLoader.LoadMesh(ResourcePaths.HEALTH_POTION);
			return new Consumable(40, healthPotion);
		}
	}
}
