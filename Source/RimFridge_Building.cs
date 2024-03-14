using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimFridge
{
	class RimFridge_Building : Building_Storage, IRenameable
	{
		public RimFridge_Building () : base()
		{}

		public string fridgeLabel;

		public string RenamableLabel
		{
			get => this.fridgeLabel ?? this.BaseLabel;
			set => this.fridgeLabel = value;
		}

		public string BaseLabel
		{
			get => this.Label;
		}

		public string InspectLabel
		{
			get => this.RenamableLabel;
		}

		public override IEnumerable<Gizmo> GetGizmos ()
		{
			foreach (var gizmo in base.GetGizmos())
			{
				yield return gizmo;
			}

			yield return new Command_Action
			{
				action = () => Find.WindowStack.Add(new Dialog_RenameFridge(this)),
				defaultLabel = "Rename".Translate(),
				defaultDesc = "RimFridge.RenameTheRefrigerator".Translate(),
				hotKey = KeyBindingDefOf.Misc1,
				icon = ContentFinder<Texture2D>.Get("UI/Buttons/Rename", true)
			};
		}

		public override void ExposeData ()
		{
			base.ExposeData();

			/* Versions older than 1.2.0 of RimFridge handled the fridge label
			   via the CompRefrigerator comp, rather than via the this building.
			   So, if that comp was previously renamed that name will be assigned
			   to its parent RimFridge during the LoadingVars stage. Thus, during the
			   LoadingVars we load the fridgeLabel only if the fridgeLabel is null,
			   so as not clobber the name migrated from the comp. */
			if (Scribe.mode != LoadSaveMode.LoadingVars || this.fridgeLabel == null)
			{
				Scribe_Values.Look(ref this.fridgeLabel, "fridgeLabel");
			}
		}
	}
}

