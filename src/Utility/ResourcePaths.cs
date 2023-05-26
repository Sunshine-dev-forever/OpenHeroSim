
using Godot;
using Serilog;
namespace Util {
	//one day this will all be defined in JSON... one day
	public static class ResourcePaths {
		public static string IRON_SWORD = "res://scenes/weapons/iron_sword.tscn";
		public static string LIGHTSABER = "res://scenes/weapons/light_saber.tscn";
		public static string RUSTED_DAGGER = "res://scenes/weapons/rusted_dagger.tscn";
		public static string HEALTH_POTION = "res://scenes/weapons/health_potion.tscn";
		public static string BOX_HELM = "res://scenes/world_objects/box_helm.tscn";
		public static string TREASURE_CHEST = "res://scenes/world_objects/treasure_chest.tscn";
		//just the pawn model
		public static string PAWN_MODEL = "res://assets/basic_pawn.glb";
		//Pawn scene comes with all the scripts and structure
		public static string PAWN_SCENE = "res://scenes/pawn/pawn.tscn";
		public static string GRAVESTONE = "res://scenes/world_objects/gravestone.tscn";
		//A kind of throwing spear used in the middle east. It is really cool!
		public static string DJERID = "res://scenes/weapons/djerid.tscn";
		public static string DEFAULT_MESH_FILE_PATH = "res://scenes/visual_sphere.tscn";
	}
}