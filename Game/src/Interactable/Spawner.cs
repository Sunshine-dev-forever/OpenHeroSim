using Godot;
using GUI.DebugInspector.Display;
using Item;
using NSubstitute.Routing.Handlers;
using Pawn.Goal;
using System;
using System.Collections.Generic;
using Worlds;

namespace Interactable;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

public partial class Spawner : Node3D, IInteractable {

    private PawnGenerator pawnGenerator;
    private List<IPawnGoal> pawnGoals;
    private bool wasSetup = false;

    [Export]
    //How many seconds to wait inbetween spawning
    private double spawnRate = 10;

    private double timeSinceLastSpawn = 0;

    public override void _Process(double delta) {
        if (!wasSetup) return;

        timeSinceLastSpawn += delta;
        if (timeSinceLastSpawn > spawnRate) {
            timeSinceLastSpawn = 0;
            pawnGenerator.RandomPawn(pawnGoals, GetSpawnLocation());
        }
    }

    public void Setup(PawnGenerator _pawnGenerator, List<IPawnGoal> _pawnGoals) {
        wasSetup = true;
        pawnGenerator = _pawnGenerator;
        pawnGoals = _pawnGoals;
    }

    public IDisplay Display {
        get {
            Display root = new("Spawner");
            if (!wasSetup) {
                root.AddDetail("was not setup yet");
                return root;
            }

            Display pawnGoalsDisplay = new("Pawn Goals");
            foreach (IPawnGoal goal in pawnGoals) {
                pawnGoalsDisplay.AddDetail(goal.GetType().Name);
            }

            root.AddChildDisplay(pawnGoalsDisplay);
            return root;
        }
    }

    public bool IsInstanceValid() => IsInstanceValid(this);

    private Vector3 GetSpawnLocation() {
        Vector3 origin = GlobalTransform.Origin;
        Vector3 offset = new(0, 1, -5);
        return origin + offset;
    }
}
