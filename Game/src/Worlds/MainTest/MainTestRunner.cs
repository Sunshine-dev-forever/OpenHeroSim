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
using Pawn.Targeting;
using Interactable;

namespace Worlds.MainTest 
{
	public class MainTestRunner : IRunner
	{

		private KdTreeController kdTreeController;
		//MainTestRunner will make children out of nodeStorage
		private Node nodeStorage;
		public MainTestRunner(KdTreeController _kdTreeController, Node _nodeStorage)
		{
			kdTreeController = _kdTreeController;
			nodeStorage = _nodeStorage;
		}
		
		public void Input(InputEvent input) {
			if(input.IsActionPressed("ui_left")) {
				//CreateTestProjectile();
				NavigationRegion3D navigation = nodeStorage.GetNode<NavigationRegion3D>("/root/Node3D/NavigationRegion3D");
			
				Vector3 location = new Vector3(4,0,4);
				PawnControllerBuilder.CreateTrainingDummy(location, nodeStorage, kdTreeController, navigation);
			} else if (input.IsActionPressed("ui_right")) {
				CreatePawnInCenter();
			}
		}
		private double TimeSinceLastPawnCreation = 4;
		//lazy, bad coding
		private IPawnController lastPawnSpawned = null!;

		public void Process(double delta)
		{
			TimeSinceLastPawnCreation += delta;
			if(TimeSinceLastPawnCreation > 5) {
				TimeSinceLastPawnCreation = 0;
				lastPawnSpawned = CreatePawn();
			}
		}

		public IPawnController CreateThrowableTester() {
			NavigationRegion3D navigation = nodeStorage.GetNode<NavigationRegion3D>("/root/Node3D/NavigationRegion3D");
			
			Vector3 location = new Vector3(0,5,0);
			return PawnControllerBuilder.Start(nodeStorage, kdTreeController, navigation)
								.AddGoal(new HealGoal())
								.AddGoal(new DefendSelfGoal())
								.AddGoal(new LootGoal())
								.AddGoal(new WanderGoal())
								.AddAbility(AbilityDefinitions.THROW_ABILITY)
								.AddAbility(AbilityDefinitions.STAB_ABILITY)
								.WearEquipment(GetRandomWeapon())
								.WearEquipment(GetHelmet())
								.AddItem(CreateThrowable())
								.Location(location)
								.Finish();		
		}

		public Throwable CreateThrowable() {
			Node3D spear = CustomResourceLoader.LoadMesh(ResourcePaths.DJERID);
			return new Throwable(spear, 60, "throwing djerd");
		}

		private void CreateTestProjectile() {
			Node3D spear = CustomResourceLoader.LoadMesh(ResourcePaths.DJERID);
			//I add an offset so the spear target's the pawns chest and not the pawn's feet
			Vector3 offset = new Vector3(0,1,0);
			ITargeting target = new InteractableTargeting(lastPawnSpawned, offset);
			Projectile projectile = new Projectile(spear, target, 50, true );
			nodeStorage.AddChild(projectile);
			projectile.GlobalTransform = new Transform3D(projectile.GlobalTransform.Basis, new Vector3(0,5,0));
		}

		private IPawnController CreatePawn(){
			NavigationRegion3D navigation = nodeStorage.GetNode<NavigationRegion3D>("/root/Node3D/NavigationRegion3D");
			
			Vector3 location = GenerateRandomVector();
			
			return PawnControllerBuilder.Start(nodeStorage, kdTreeController, navigation)
								.AddGoal(new HealGoal())
								.AddGoal(new DefendSelfGoal())
								.AddGoal(new LootGoal())
								.AddGoal(new WanderGoal())
								.WearEquipment(GetRandomWeapon())
								.AddAbility(AbilityDefinitions.STAB_ABILITY)
								.Location(location)
								.Finish();
		}

		private IPawnController CreatePawnInCenter(){
			NavigationRegion3D navigation = nodeStorage.GetNode<NavigationRegion3D>("/root/Node3D/NavigationRegion3D");
			
			Vector3 location = new Vector3(0,5,0);
			
			return PawnControllerBuilder.Start(nodeStorage, kdTreeController, navigation)
								.AddGoal(new HealGoal())
								.AddGoal(new DefendSelfGoal())
								.AddAbility(AbilityDefinitions.STAB_ABILITY)
								.AddGoal(new LootGoal())
								.AddGoal(new WanderGoal())
								.WearEquipment(GetRandomWeapon())
								.WearEquipment(GetHelmet())
								.Location(location)
								.Finish();
		}

		private Equipment GetHelmet() {
			Equipment equipment = new Equipment(EquipmentType.HEAD, "box helm");
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

		private IPawnController CreateHealingPotionTester() {
			NavigationRegion3D navigation = nodeStorage.GetNode<NavigationRegion3D>("/root/Node3D/NavigationRegion3D");
			return PawnControllerBuilder.Start(nodeStorage, kdTreeController, navigation)
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
			nodeStorage.AddChild(itemContainer);
			itemContainer.GlobalTransform = new Transform3D(itemContainer.GlobalTransform.Basis, new Vector3(0,1,0));
			kdTreeController.AddInteractable(itemContainer);
		}

		private Equipment CreateIronSword() {
			Equipment equipment = new Equipment(EquipmentType.HELD, "iron sword");
			equipment.Damage = 5;
			return equipment;
		}

		private Equipment CreateRustedDagger() {
			Equipment equipment = new Equipment(EquipmentType.HELD, "rusted dagger");
			equipment.Damage = 3;
			return equipment;
		}

		private Equipment CreateLightSaber() {
			Equipment equipment = new Equipment(EquipmentType.HELD, "light saber");
			equipment.Damage = 10;
			return equipment;
		}
		private Equipment CreateSpearMelee() {
			Equipment equipment = new Equipment(EquipmentType.HELD, "melee djerd");
			equipment.Damage = 40;
			return equipment;
		}

		private Consumable CreateHealingPotion() {
			return new Consumable(40, "Health Potion");
		}
	}
}
