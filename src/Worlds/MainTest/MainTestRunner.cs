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

namespace Worlds.MainTest 
{
	public class MainTestRunner : Node
	{

		private KdTreeController kdTreeController = null!; 
		public override void _Ready()
		{
			kdTreeController = new KdTreeController();
			this.AddChild(kdTreeController);
		}
		
		public override void _Input(InputEvent input) {
			if(input.IsActionPressed("mouse_left_click")) {
				
			} else if(input.IsActionPressed("ui_left")) {
				CreateItemContainer();
			} else if (input.IsActionPressed("ui_right")) {
				CreateHealingPotionTester();
			}
		}
		private float TimeSinceLastPawnCreation = 4;
		public override void _Process(float delta)
		{
			TimeSinceLastPawnCreation += delta;
			if(TimeSinceLastPawnCreation > 5) {
				TimeSinceLastPawnCreation = 0;
				CreatePawn();
			}
			
		}

		private PawnController CreatePawn(){
			Navigation navigation = GetNode<Navigation>("/root/Spatial/Navigation");
			
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

		private Equipment GetHelmet() {
			//"res://scenes/world_objects/box_helm.tscn"
			Spatial boxHelm = CustomResourceLoader.LoadMesh(ResourcePaths.BOX_HELM);
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
				case 0: return CreateLightSaber();
				case 1: return CreateRustedDagger();
				case 2: return CreateIronSword();
				default:
				return CreateRustedDagger();
			}
		}
		private void Adhoc2(){

		}

		private PawnController CreateHealingPotionTester() {
			Navigation navigation = GetNode<Navigation>("/root/Spatial/Navigation");
			return PawnControllerBuilder.Start(this, kdTreeController, navigation)
								.AddGoal(new HealGoal())
								.AddGoal(new WanderGoal())
								.Location(new Vector3(0,5,0))
								.WearEquipment(CreateLightSaber())
								.AddConsumable(CreateHealingPotion()).AddConsumable(CreateHealingPotion())
								.DealDamage(50)
								.Finish();
		}

		private void CreateItemContainer() {
			Spatial TreasureChestMesh = CustomResourceLoader.LoadMesh(ResourcePaths.TREASURE_CHEST);
			//The iron sword gets leaked when created like this
			List<IItem> items = new List<IItem>();
			items.Add(CreateHealingPotion());
			items.Add(CreateIronSword());
			ItemContainer itemContainer = new ItemContainer(items, TreasureChestMesh);
			this.AddChild(itemContainer);
			itemContainer.GlobalTransform = new Transform(itemContainer.GlobalTransform.basis, new Vector3(0,1,0));
			kdTreeController.AddInteractable(itemContainer);
		}

		private Equipment CreateIronSword() {
			Spatial ironSword = CustomResourceLoader.LoadMesh(ResourcePaths.IRON_SWORD);
			Equipment equipment = new Equipment(ironSword, EquipmentType.HELD);
			equipment.Damage = 5;
			return equipment;
		}

		private Equipment CreateRustedDagger() {
			Spatial rustedDagger = CustomResourceLoader.LoadMesh(ResourcePaths.RUSTED_DAGGER);;
			Equipment equipment = new Equipment(rustedDagger, EquipmentType.HELD);
			equipment.Damage = 3;
			return equipment;
		}

		private Equipment CreateLightSaber() {
			Spatial lightSaber = CustomResourceLoader.LoadMesh(ResourcePaths.LIGHTSABER);
			Equipment equipment = new Equipment(lightSaber, EquipmentType.HELD);
			equipment.Damage = 10;
			return equipment;
		}

		private Consumable CreateHealingPotion() {
			Spatial healthPotion = CustomResourceLoader.LoadMesh(ResourcePaths.HEALTH_POTION);
			return new Consumable(40, healthPotion);
		}
	}
}
