using Verse;

namespace AltDeuteriumExtractor
{
    public class AltDeuteriumExtractorModSettings : ModSettings
    {
        public float efficiency = 0.005F, waterPerTick = 0.08333F, powerDraw = 1200F;
        public int maxDeuterium = 50, maxWater = 500;
        public string bufferEfficiency, bufferWaterPerTick, bufferPowerDraw, bufferMaxDeuterium, bufferMaxWater;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref efficiency, "efficiency", 0.005F, true);
            Scribe_Values.Look(ref waterPerTick, "waterPerTick", 0.08333F, true);
            Scribe_Values.Look(ref powerDraw, "powerDraw", 1200F, true);
            Scribe_Values.Look(ref maxDeuterium, "maxDeuterium", 50, true);
            Scribe_Values.Look(ref maxWater, "maxWater", 500, true);
        }
    }
}