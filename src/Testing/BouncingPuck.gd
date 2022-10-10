extends Spatial


# Declare member variables here. Examples:
# var a = 2
# var b = "text"

onready var body = $"KinematicBody"

export var negative_x_limit = -20
export var positive_x_limit = 20
export var moving_toward_pos = true
export var speed = 8
var gravity = -5
var velocity = Vector3.ZERO

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.

func _physics_process(delta):
	
	#if we exited our bounds
	if(body.global_transform.origin.x > positive_x_limit or
		body.global_transform.origin.x < negative_x_limit):
		#then switch direction
		moving_toward_pos = not moving_toward_pos
	
	velocity.y += gravity * delta
	if(moving_toward_pos):
		velocity.x = speed
		velocity = body.move_and_slide(velocity, Vector3.UP)
	else:
		velocity.x = -speed
		velocity = body.move_and_slide(velocity, Vector3.UP)

# a very usefull testing function
func _input(event):
	if event.is_action_pressed("mouse_left_click"):
		pass
# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass
