using Verse;

namespace AltDeuteriumExtractor
{
    public class AltDeuteriumExtractorModSettings : ModSettings
    {
        public float efficiency, efficiencyBasic, efficiencyAdvanced, waterPerTick, waterPerTickBasic, waterPerTickAdvanced, powerDraw, powerDrawBasic, powerDrawAdvanced, tolerance;
        public int maxDeuterium, maxDeuteriumBasic, maxDeuteriumAdvanced, maxWater, maxWaterBasic, maxWaterAdvanced, deuteriumPerDay;
        public string bufferEfficiencyAdvanced, bufferWaterPerTickAdvanced, bufferPowerDrawAdvanced, bufferMaxDeuteriumAdvanced, bufferMaxWaterAdvanced, bufferTolerance;
        public bool advancedMode, advancedOverride;

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