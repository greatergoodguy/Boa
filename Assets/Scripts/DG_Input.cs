using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DG_Input {
	public static bool GoUp() => Input.GetKeyDown(KeyCode.W);
	public static bool GoRight() => Input.GetKeyDown(KeyCode.D);
	public static bool GoDown() => Input.GetKeyDown(KeyCode.S);
	public static bool GoLeft() => Input.GetKeyDown(KeyCode.A);
	
	public static bool ToggleManualTick() => Input.GetKeyDown(KeyCode.P);
	public static bool NextTick() => Input.GetKeyDown(KeyCode.N);
	public static bool BackTick() => Input.GetKeyDown(KeyCode.B);
	public static bool NextTickBig() => NextTick() && Shift();
	public static bool BackTickBig() => BackTick() && Shift();

	static bool Shift() => Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.RightShift);
}
