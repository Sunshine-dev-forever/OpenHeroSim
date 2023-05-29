
using Godot;
using Serilog;
namespace Util {
	//one day this will all be defined in JSON... one day
	public static class ResourcePaths {
		public const string IRON_SWORD = "res://scenes/weapons/iron_sword.tscn";
		public const string LIGHTSABER = "res://scenes/weapons/light_saber.tscn";
		public const string RUSTED_DAGGER = "res://scenes/weapons/rusted_dagger.tscn";
		public const string HEALTH_POTION = "res://scenes/weapons/health_potion.tscn";
		public const string BOX_HELM = "res://scenes/world_objects/box_helm.tscn";
		public const string TREASURE_CHEST = "res://scenes/world_objects/treasure_chest.tscn";
		//just the pawn model
		public const string PAWN_MODEL = "res://assets/basic_pawn.glb";
		//Pawn scene comes with all the scripts and structure
		public const string PAWN_SCENE = "res://scenes/pawn/pawn.tscn";
		public const string GRAVESTONE = "res://scenes/world_objects/gravestone.tscn";
		//A kind of throwing spear used in the middle east. It is really cool!
		public const string DJERID = "res://scenes/weapons/djerid.tscn";
		public const string DEFAULT_MESH_FILE_PATH = "res://scenes/visual_sphere.tscn";
	}
}