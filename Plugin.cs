using System.Reflection;
using BB;
using BB.Base;
using BB.UI;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Rewired;
using UnityEngine;

namespace BnBHiddenFeatures;

public static class Reference {
	public const string GUID = "at.derfl007.bnb.hiddenfeatures";
	public const string PLUGIN_NAME = "BnBHiddenFeatures";
	public const string PLUGIN_VERSION = "1.0.1";
}

[BepInPlugin(Reference.GUID, Reference.PLUGIN_NAME, Reference.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin {
	public static readonly ManualLogSource LogSource = new("BnBHiddenFeatures");

	private void Awake() {
		// Plugin startup logic
		BepInEx.Logging.Logger.Sources.Add(LogSource);
		LogSource.LogInfo($"Plugin {Reference.PLUGIN_NAME} is loaded!");
		var harmony = new Harmony(Reference.GUID);
		harmony.PatchAll();
	}

	private void Update() {
		// Called every game loop. Displays the cheat panel when F3 is pressed
		if (!Input.GetKeyDown(KeyCode.F3)) return;
        UIRoot.GetFactory().SpawnPanel<CheatsPanel>();
    }

	/// <summary>
	/// Helper method to reflect the static method OnToggleUI from the BB.AdventureMode class. 
	/// </summary>
	/// <param name="obj">The parameter for the method</param>
	/// <param name="instance">The instance of BB.AdventureMode</param>
	public static void OnToggleUI(InputActionEventData obj, AdventureMode instance) {
		var onToggleUI =
			typeof(AdventureMode).GetMethod("OnToggleUI", BindingFlags.NonPublic | BindingFlags.Instance);

		var parameters = new object[] { obj };

		onToggleUI?.Invoke(instance, parameters);
	}

	/// <summary>
	/// Helper method to reflect the static method OnToggleDevMode from the BB.AdventureMode class.
	/// </summary>
	/// <param name="obj">The parameter for the method</param>
	/// <param name="instance">The instance of BB.AdventureMode</param>
	public static void OnToggleDevModeView(InputActionEventData obj, AdventureMode instance) {
		var onToggleDevModeView =
			typeof(AdventureMode).GetMethod("OnToggleDevModeView", BindingFlags.NonPublic | BindingFlags.Instance);

		var parameters = new object[] { obj };

		onToggleDevModeView?.Invoke(instance, parameters);
	}

	/// <summary>
	/// Helper method to reflect the static method OnPauseTime from the BB.AdventureMode class.
	/// </summary>
	/// <param name="obj">The parameter for the method</param>
	/// <param name="instance">The instance of BB.AdventureMode</param>
	public static void OnPauseTime(InputActionEventData obj, AdventureMode instance) {
		var onPauseTime =
			typeof(AdventureMode).GetMethod("OnPauseTime", BindingFlags.NonPublic | BindingFlags.Instance);

		var parameters = new object[] { obj };

		onPauseTime?.Invoke(instance, parameters);
	}
	
	/// <summary>
	/// Helper method to reflect the static method OnFFTime from the BB.AdventureMode class.
	/// </summary>
	/// <param name="obj">The parameter for the method</param>
	/// <param name="instance">The instance of BB.AdventureMode</param>
	public static void OnFFTime(InputActionEventData obj, AdventureMode instance) {
		var onFFTime =
			typeof(AdventureMode).GetMethod("OnFFTime", BindingFlags.NonPublic | BindingFlags.Instance);

		var parameters = new object[] { obj };

		onFFTime?.Invoke(instance, parameters);
	}

	/// <summary>
	/// Prints the keycode associated with the given action.
	/// </summary>
	/// <param name="player">The Player object. Needed to get the control map as it is tied to the player</param>
	/// <param name="actionName">The action that we want to find the keycode(s) for</param>
	public static void PrintActionKeyCodes(Player player, string actionName) {
		var aems = player.controllers.maps.ButtonMapsWithAction(ControllerType.Keyboard, actionName, false);
		foreach (var aem in aems) {
			var action = ReInput.mapping.GetAction(aem.actionId);
			if (action == null) continue; // invalid Action
			if (aem.keyCode == KeyCode.None) continue; // there is no key assigned

			var descriptiveName = action.descriptiveName; // get the descriptive name of the Action

			// Create a string name that contains the primary key and any modifier keys
			var key = aem.keyCode.ToString(); // get the primary key code as a string
			if (aem.modifierKey1 != ModifierKey.None) key += " + " + aem.modifierKey1;
			if (aem.modifierKey2 != ModifierKey.None) key += " + " + aem.modifierKey2;
			if (aem.modifierKey3 != ModifierKey.None) key += " + " + aem.modifierKey3;

			if (action.type == InputActionType.Axis) {
				// this is an axis-type Action

				// Determine if it contributes to the positive or negative value of the Action
				if (aem.axisContribution == Pole.Positive) {
					// positive
					descriptiveName = !string.IsNullOrEmpty(action.positiveDescriptiveName)
						? action.positiveDescriptiveName
						: // use the positive name if one exists
						action.descriptiveName + " +"; // use the descriptive name with sign appended if not
				}
				else {
					// negative
					descriptiveName = !string.IsNullOrEmpty(action.negativeDescriptiveName)
						? action.negativeDescriptiveName
						: // use the negative name if one exists
						action.descriptiveName + " -"; // use the descriptive name with sign appended if not
				}
			}

			LogSource.LogInfo(descriptiveName + " is assigned to " + key);
		}
	}
}