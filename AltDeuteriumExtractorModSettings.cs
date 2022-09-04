using Verse;

namespace AltDeuteriumExtractor
{
    public class AltDeuteriumExtractorModSettings : ModSettings
    {
        public float efficiency, efficiencyBasic = 0.005F, efficiencyAdvanced = 0.005F, waterPerTick, waterPerTickBasic = 0.083333F, waterPerTickAdvanced = 0.083333F, powerDraw, powerDrawBasic = 1200F, powerDrawAdvanced = 1200F, tolerance = 0.005F;
        public int maxDeuterium, maxDeuteriumBasic = 50, maxDeuteriumAdvanced = 50, maxWater, maxWaterBasic = 500, maxWaterAdvanced = 500, deuteriumPerDay;
        public string bufferEfficiencyAdvanced, bufferWaterPerTickAdvanced, bufferPowerDrawAdvanced, bufferMaxDeuteriumAdvanced, bufferMaxWaterAdvanced, bufferTolerance;
        public bool advancedMode, advancedOverride = false;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref efficiencyBasic, "efficiencyBasic", 0.005F, true);
            Scribe_Values.Look(ref waterPerTickBasic, "waterPerTickBasic", 0.083333F, true);
            Scribe_Values.Look(ref powerDrawBasic, "powerDrawBasic", 1200F, true);
            Scribe_Values.Look(ref maxDeuteriumBasic, "maxDeuteriumBasic", 50, true);
            Scribe_Values.Look(ref maxWaterBasic, "maxWaterBasic", 500, true);
            Scribe_Values.Look(ref efficiencyAdvanced, "efficiencyAdvanced", 0.005F, true);
            Scribe_Values.Look(ref waterPerTickAdvanced, "waterPerTickAdvanced", 0.083333F, true);
            Scribe_Values.Look(ref powerDrawAdvanced, "powerDrawAdvanced", 1200F, true);
            Scribe_Values.Look(ref maxDeuteriumAdvanced, "maxDeuteriumAdvanced", 50, true);
            Scribe_Values.Look(ref maxWaterAdvanced, "maxWaterAdvanced", 500, true);
            Scribe_Values.Look(ref advancedOverride, "advancedOverride", false, true);
            Scribe_Values.Look(ref tolerance, "tolerance", 0.005F, true);
        }

    }
}