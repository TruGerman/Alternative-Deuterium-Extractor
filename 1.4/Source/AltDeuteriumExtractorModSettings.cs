using Verse;

namespace AltDeuteriumExtractor
{
    public class AltDeuteriumExtractorModSettings : ModSettings
    {
        public float waterPerDay = 5000F, deuteriumPerDay = 25F;
        public int maxDeuterium = 50, maxWater = 500;
        public string bufferMaxDeuterium, bufferMaxWater, bufferDeuteriumPerDay, bufferWaterPerDay;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref deuteriumPerDay, "deuteriumPerDay", 25F, true);
            Scribe_Values.Look(ref waterPerDay, "waterPerDay", 5000F, true);
            Scribe_Values.Look(ref maxDeuterium, "maxDeuterium", 50, true);
            Scribe_Values.Look(ref maxWater, "maxWater", 500, true);
        }
    }
}