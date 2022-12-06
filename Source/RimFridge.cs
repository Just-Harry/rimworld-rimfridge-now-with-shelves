using RimWorld;
using Verse;

namespace RimFridge
{
	public class RimFridgeComponent : GameComponent
	{
		internal SettingsController mod;

		public RimFridgeComponent (Game game) : this()
		{}

		public RimFridgeComponent ()
		{
			this.mod = LoadedModManager.GetMod<SettingsController>();
		}

		internal static RimFridgeComponent get;

		public override void LoadedGame ()
		{
			this.StartedOrLoadedGame();
		}

		public override void StartedNewGame ()
		{
			this.StartedOrLoadedGame();
		}

		public void StartedOrLoadedGame ()
		{
			RimFridgeComponent.get = Current.Game.GetComponent<RimFridgeComponent>();

			DefDatabase<ResearchProjectDef>.GetNamed("RimFridge_PowerFactorSetting").tab = null;

			RimFridgeSettingsUtil.ApplyFactor(Settings.PowerFactor.AsFloat);
		}
	}
}

