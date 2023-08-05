extends Panel

@export var optionsMenu : PackedScene
@export var worldSelector : PackedScene

func _on_options_button_down():
	get_tree().current_scene.add_child(optionsMenu.instantiate())

func _on_exit_button_down():
	get_tree().quit() # Replace with function body.

func _on_start_button_down():
	get_tree().current_scene.add_child(worldSelector.instantiate())
