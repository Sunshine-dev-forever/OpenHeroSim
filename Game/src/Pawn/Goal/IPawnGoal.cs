using Pawn.Components;
using Pawn.Tasks;
namespace Pawn.Goal;
public interface IPawnGoal
{
    ITask GetTask(IPawnController pawnController, SensesStruct sensesStruct);
}
