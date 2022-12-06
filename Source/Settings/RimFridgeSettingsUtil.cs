using RimWorld;
using System.Linq;
using Verse;

namespace RimFridge
{
    internal static class RimFridgeSettingsUtil
    {
        public static void ApplyFactor (float newFactor)
        {
			var powerFactorSetting = DefDatabase<ResearchProjectDef>.GetNamed("RimFridge_PowerFactorSetting");

			foreach (var def in DefDatabase<ThingDef>.AllDefs.Where(d => d.defName.StartsWith("RimFridge")))
			{
				var power = def.GetCompProperties<CompProperties_Power>();

				if (power == null)
				{
					continue;
				}

				power.powerUpgrades ??= new(1);

				var powerUpgrade = power.powerUpgrades.Find(u => u.researchProject == powerFactorSetting);

				if (powerUpgrade == null)
				{
					powerUpgrade = new CompProperties_Power.PowerUpgrade()
					{
						researchProject = powerFactorSetting
					};
					power.powerUpgrades.Insert(0, powerUpgrade);
				}

				powerUpgrade.factor = newFactor;
			}

			Current.Game?.researchManager.FinishProject(
				powerFactorSetting,
				doCompletionDialog: false,
				researcher: null,
				doCompletionLetter: false
			);
        }
    }
}
