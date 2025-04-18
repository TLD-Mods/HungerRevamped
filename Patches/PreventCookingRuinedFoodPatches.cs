﻿using HarmonyLib;
using Il2Cpp;
using Il2CppTLD.Cooking;
using MelonLoader;

namespace HungerRevamped {
	internal static class PreventCookingRuinedFoodPatches
	{

		[HarmonyPatch(typeof(CookingPotItem), nameof(CookingPotItem.SetCookedGearProperties), new Type[] { typeof(GearItem), typeof(GearItem) })]
		private static class RuinedFoodRemainsRuinedWhenCooked {
			private static void Postfix(GearItem rawItem, GearItem cookedItem) {
				if (MenuSettings.settings.canCookRuinedFood) return;
				if (!rawItem || !cookedItem) return;

				if (rawItem.IsWornOut()) {
					cookedItem.ForceWornOut();
					cookedItem.UpdateDamageShader();
				}
			}
		}

		[HarmonyPatch(typeof(Cookable), nameof(Cookable.MaybeStartWarmingUpDueToNearbyFire))]
		private static class PreventWarmingUpRuinedFood {
			private static bool Prefix(Cookable __instance) {
				if (MenuSettings.settings.canCookRuinedFood) return true;
				GearItem gearItem = __instance.GetComponent<GearItem>();
				return !gearItem.IsWornOut(); // Do not run original method when item is ruined
			}
		}
	
		[HarmonyPatch(typeof(CookableItem), nameof(CookableItem.GetRecipeCookability))]
		[Harmony]
		private static class GetRecipeCookability {
			private static void Postfix(CookableItem __instance, CookingPotItem cookingPot, Inventory inventory, ref CookableItem.Cookablility __result) {
				if (__result != CookableItem.Cookablility.Cookable) return;
				if (MenuSettings.settings.canCookRuinedFood) return;
			}
		}
	}
}
