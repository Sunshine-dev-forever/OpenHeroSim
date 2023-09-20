# Asset contributions
## blender
for blender these are the things to note:\
1 blender unit = 1 godot unit = 1 meter

A normal human is 1.7 meters tall on average, model with that in mind

All used animations by this game are as follows:
    Die, (ex: pawn death)\
    Consume, (ex: using a health potion)\
    Idle, (ex: waiting around)\
    Interact, (ex: looking a grave, crafting an item, buying something from a shop, harvesting wheat)\
    Walk, (ex: ...its walking...)\
    MeleeAttack, (ex: a stab, strike, swing, etc)\
    RangedAttack (ex: throwing a javelin, shooting a bow)

For the game to function, only the Interact animation needs to be implemented, but that would look awful.\
Any contribution should implement all the above animations, and at least one attack. (both attacks are still preferred)

Exporting assets from blender:\
	make sure that the animations you are exporting are stored in the NLA stack\
	export as a glb file. \
	You want to look up a video that explains this in more detail, because to be honest I dont understand it much myself
