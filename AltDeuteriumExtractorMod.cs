using UnityEngine;
using Verse;

namespace AltDeuteriumExtractor
{
    public class AltDeuteriumExtractorMod : Mod
    {
        public static AltDeuteriumExtractorModSettings settings;

        public AltDeuteriumExtractorMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<AltDeuteriumExtractorModSettings>();
        }

        public override string SettingsCategory()
        {
            return "ADE_settingsCategoryLabel".Translate();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            Listing_Standard ls = new();
            ls.Begin(inRect);
            ls.TextFieldNumericLabeled("ADE_EfficiencySetting".Translate(), ref settings.efficiency, ref settings.bufferEfficiency, 0.0001F, float.MaxValue);
            ls.TextFieldNumericLabeled("ADE_WaterPerTickSetting".Translate(), ref settings.waterPerTick, ref settings.bufferWaterPerTick, 0.001F, float.MaxValue);
            ls.TextFieldNumericLabeled("ADE_PowerDrawSetting".Translate(), ref settings.powerDraw, ref settings.bufferPowerDraw, float.MinValue, float.MaxValue);
            ls.TextFieldNumericLabeled("ADE_MaxDeuteriumSetting".Translate(), ref settings.maxDeuterium, ref settings.bufferMaxDeuterium, 1, int.MaxValue);
            ls.TextFieldNumericLabeled("ADE_MaxWaterSetting".Translate(), ref settings.maxWater, ref settings.bufferMaxWater, 1, int.MaxValue);
            ls.End();
        }
    }

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
