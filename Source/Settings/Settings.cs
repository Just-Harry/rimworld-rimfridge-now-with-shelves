using RimWorld;
﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimFridge
{
    public class SettingsController : Mod
    {
        public SettingsController(ModContentPack content) : base(content)
		{
			base.GetSettings<Settings>();

			var harmony = new Harmony("com.rimfridge.rimworld.mod");

			/* |TODO| Use Harmony patch-categories for this, when the next version of Harmony is released. */

			harmony.CreateClassProcessor(typeof(Patch_ReachabilityUtility_CanReach)).Patch();
			harmony.CreateClassProcessor(typeof(Patch_Thing_AmbientTemperature)).Patch();
			harmony.CreateClassProcessor(typeof(Patch_PassingShip_TryOpenComms)).Patch();
			harmony.CreateClassProcessor(typeof(Patch_FoodUtility_TryFindBestFoodSourceFor)).Patch();
			harmony.CreateClassProcessor(typeof(DisplayStackedItemsNicelyInFridges.MungeTrueCenterOfItemsInFridges)).Patch();
			harmony.CreateClassProcessor(typeof(DisplayStackedItemsNicelyInFridges.MakeTheStackCountLabelsReadable)).Patch();
			harmony.CreateClassProcessor(typeof(HacksForCompatibility.ForceTheApplicationOfSomePatches)).Patch();

			if (VersionControl.CurrentVersion < new Version(1, 4, 3580))
			{
				harmony.CreateClassProcessor(typeof(Patch_Building_NutrientPasteDispenser_FindFeedInAnyHopper)).Patch();
				harmony.CreateClassProcessor(typeof(Patch_Building_NutrientPasteDispenser_HasEnoughFeedstockInHoppers)).Patch();
				harmony.CreateClassProcessor(typeof(Patch_Alert_PasteDispenserNeedsHopper_BadDispensers_Getter)).Patch();

				Logger.Message("The RimWorld version is older than 1.4.3580, so we're patching the hopper-finding behaviour of Nutrient Paste Dispensers.");
			}
		}

        public override string SettingsCategory()
        {
            return "RimFridge";
        }

		internal class GUIState
		{
			internal Vector2 forcedApplicationOfPatchesScrollPosition;
			internal List<bool> initialStateOfShouldForceApplicationOfPatches;
		}

		internal GUIState guiState = null;

        public override void DoSettingsWindowContents(Rect rect)
        {
			if (guiState == null)
			{
				guiState = new();
				guiState.initialStateOfShouldForceApplicationOfPatches = (
					Settings.forcedApplicationOfPatches.Select(a => a.shouldForceApplication).ToList()
				);
			}

            GUI.BeginGroup(new Rect(0, 60, 800, 600));
            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(0, 40, 300, 20), "Modify Base Power Requirement" + ":");
            Settings.PowerFactor.AsString = Widgets.TextField(new Rect(320, 40, 100, 20), Settings.PowerFactor.AsString);
            if (Widgets.ButtonText(new Rect(320, 65, 100, 20), "Apply"))
            {
                if (Settings.PowerFactor.ValidateInput())
                {
                    GetSettings<Settings>().Write();
                    Messages.Message("New Power Factor Applied", MessageTypeDefOf.PositiveEvent);

                    if (Current.Game != null)
                    {
                        RimFridgeSettingsUtil.ApplyFactor(Settings.PowerFactor.AsFloat);
                    }
                }
            }
            Widgets.Label(new Rect(20, 100, 400, 30), "<new power usage> = <input value> * <original power usage>");
            Widgets.CheckboxLabeled(new Rect(0, 140, 200, 30), "Act as Trade Beacon:", ref Settings.ActAsBeacon);

            if (ShouldShowCompatibilitySettings)
            {
				Widgets.DrawMenuSection(new Rect(0, 180, 800, 370));

				Widgets.Label(new Rect(10, 180, 790, 30), "RimFridge.Compatibility".Translate());

				Widgets.Label(new Rect(10, 210, 790, 30), "RimFridge.ForceApplicationOfThesePatches".Translate());
				Widgets.BeginScrollView(new Rect(0, 250, 800, 300), ref guiState.forcedApplicationOfPatchesScrollPosition, new Rect(0, 250, 800 - GenUI.ScrollBarWidth, 30 * Settings.forcedApplicationOfPatches.Count));

				for (int index = 0; index < Settings.forcedApplicationOfPatches.Count; ++index)
				{
					var patch = Settings.forcedApplicationOfPatches[index];

					Widgets.CheckboxLabeled(
						new Rect(20, 250 + index * 30, 780 - GenUI.ScrollBarWidth, 28),
						patch.patch,
						ref patch.shouldForceApplication,
						placeCheckboxNearText: true
					);
				}

				Widgets.EndScrollView();
			}

            GUI.EndGroup();
        }

		public override void WriteSettings ()
		{
			base.WriteSettings();

			if (ShouldShowCompatibilitySettings)
			{
				if (
					!guiState.initialStateOfShouldForceApplicationOfPatches.SequenceEqual(
						Settings.forcedApplicationOfPatches.Select(a => a.shouldForceApplication)
					)
				)
				{
					ModsConfig.RestartFromChangedMods();
				}
			}

			guiState = null;
		}

		protected static bool ShouldShowCompatibilitySettings
		{
			get => Current.Game == null;
		}
    }

    internal class Settings : ModSettings
    {
        public static readonly FloatInput PowerFactor = new FloatInput("Base Power Factor");
        public static bool ActAsBeacon = false;
		/* Making this a List causes access to be O(n), but we want to maintain
			the order the patches were loaded in. */
		public static List<ApplicationOfPatch> forcedApplicationOfPatches = new();

		internal class ApplicationOfPatch : IExposable
		{
			public string patch;
			public bool shouldForceApplication;

			public void ExposeData ()
			{
				Scribe_Values.Look(ref this.patch, "patch");
				Scribe_Values.Look(ref this.shouldForceApplication, "shouldForceApplication", true);
			}
		}

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref (PowerFactor.AsString), "RimFridge.PowerFactor", "1.00", false);
            Scribe_Values.Look(ref ActAsBeacon, "RimFridge.ActAsBeacon", false, false);
            Scribe_Collections.Look(ref forcedApplicationOfPatches, "RimFridge.ForcedApplicationOfPatches", LookMode.Deep);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
            	forcedApplicationOfPatches ??= new();
            }
        }
    }
}