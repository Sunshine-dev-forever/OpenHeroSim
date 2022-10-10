using Godot;
using System;
using Serilog;

public class GeneralUtil
{
	public void Assert(bool cond, string msg){
		if(cond) {
			//all is well in the world!
		} else {
			//This is an option: GD.PrintErr(message);
			throw new ApplicationException($"Assert Failed: {msg}");
		}
	}
}
