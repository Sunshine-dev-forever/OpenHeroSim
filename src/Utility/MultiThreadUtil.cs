
using System.Threading.Tasks;
using System;
using Serilog;

//This class really sucks since It can only handle one task per object
//One day I might make this much more robust
public class MultiThreadUtil {

	
	Task? task;
	public void Run(Action action) {
		task = Task.Run(action);
	}

	public bool IsTaskCompleted() {
		if(task == null) {
			Log.Error("Tried to check if a null task was completed");
			throw new ApplicationException("Task is null");
		}
		return task.IsCompleted;
	}
}