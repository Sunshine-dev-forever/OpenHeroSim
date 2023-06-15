extends Panel

@export var main_scene : PackedScene
@export var battle_royale : PackedScene
@export var tower_defense : PackedScene


func _on_main_test_button_down():
	get_tree().change_scene_to_packed(main_scene)

func _on_back_button_down():
	self.queue_free()

func _on_battle_royale_button_down():
	get_tree().change_scene_to_packed(battle_royale)


func _on_tower_defense_button_down():
	get_tree().change_scene_to_packed(tower_defense)
