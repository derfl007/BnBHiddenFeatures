using BB;
using BB.Base;
using HarmonyLib;
using Rewired;
using UnityEngine;
using InputManager = BB.Controls.InputManager;

namespace BnBHiddenFeatures;

/// <summary>
/// Patches the <see cref="AdventureMode.CreateControlAndUI"/> method in <see cref="AdventureMode"/>.
/// </summary>
[HarmonyPatch(typeof(AdventureMode), "CreateControlAndUI")]
public class AdventureModeCreateControlAndUI {
	public static void Postfix(AdventureMode __instance) {
		var player = ServiceLocator.Resolve<InputManager>().player;

		// Try to figure out what the shortcuts for the devtools are.
		Plugin.PrintActionKeyCodes(player, "DevModeToggleUI"); // Seems to be KeypadPeriod, but only works when DevModeToggleView is active.
		Plugin.PrintActionKeyCodes(player, "DevModeToggleView"); // Seems to be KeypadEnter
		Plugin.PrintActionKeyCodes(player, "DevModePauseTime"); // Can't find this one.
		Plugin.PrintActionKeyCodes(player, "DevModeMaxTime"); // Can't find this one.

		// just making sure we're not adding the delegates twice if isDebugBuild is (somehow) true
		if (Debug.isDebugBuild) return;
		
		// Add delegates for dev actions.
		player.AddInputEventDelegate(data => Plugin.OnToggleUI(data, __instance), UpdateLoopType.Update,
			InputActionEventType.ButtonJustPressed,
			"DevModeToggleUI");
		player.AddInputEventDelegate(data => Plugin.OnToggleDevModeView(data, __instance), UpdateLoopType.Update,
			InputActionEventType.ButtonJustPressed, "DevModeToggleView");
		player.AddInputEventDelegate(data => Plugin.OnPauseTime(data, __instance), UpdateLoopType.Update,
			InputActionEventType.ButtonJustPressed,
			"DevModePauseTime");
		player.AddInputEventDelegate(data => Plugin.OnFFTime(data, __instance), UpdateLoopType.Update,
			InputActionEventType.ButtonJustPressed,
			"DevModeMaxTime");
	}
}

/// <summary>
/// Patches the <see cref="AdventureMode.OnDisable"/> method in <see cref="AdventureMode"/>.
/// </summary>
[HarmonyPatch(typeof(AdventureMode), "OnDisable")]
public class AdventureModeOnDestroy {
	public static void Postfix(AdventureMode __instance) {
		var player = ServiceLocator.Resolve<InputManager>().player;

		// Making sure that the actions are removed in OnDisable if we're not in a debug build.
		if (Debug.isDebugBuild) return;
		player.RemoveInputEventDelegate(data => Plugin.OnToggleUI(data, __instance), UpdateLoopType.Update,
			InputActionEventType.ButtonJustPressed,
			"DevModeToggleUI");
		player.RemoveInputEventDelegate(data => Plugin.OnToggleDevModeView(data, __instance), UpdateLoopType.Update,
			InputActionEventType.ButtonJustPressed, "DevModeToggleView");
		player.RemoveInputEventDelegate(data => Plugin.OnPauseTime(data, __instance), UpdateLoopType.Update,
			InputActionEventType.ButtonJustPressed,
			"DevModePauseTime");
		player.RemoveInputEventDelegate(data => Plugin.OnFFTime(data, __instance), UpdateLoopType.Update,
			InputActionEventType.ButtonJustPressed,
			"DevModeMaxTime");
	}
}