extends ColorRect

func _process(_delta):
	$FPSCounter.text = str(Engine.get_frames_per_second())
