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
            CompDeuteriumProcessor.inverseEfficiency = 1F / settings.efficiency;
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
            ls.Label("ADE_EfficiencySetting".Translate(), -1F, "ADE_EfficiencySettingTooltip".Translate());
            ls.TextFieldNumeric(ref settings.efficiency, ref settings.bufferEfficiency, 0.0001F, float.MaxValue);
            ls.Label("ADE_WaterPerTickSetting".Translate(), -1F, "ADE_WaterPerTickSettingTooltip".Translate());
            ls.TextFieldNumeric(ref settings.waterPerTick, ref settings.bufferWaterPerTick, 0.001F, float.MaxValue);
            ls.Label("ADE_PowerDrawSetting".Translate(), -1f, "ADE_PowerDrawSettingTooltip".Translate());
            ls.TextFieldNumeric(ref settings.powerDraw, ref settings.bufferPowerDraw, float.MinValue, float.MaxValue);
            ls.Label("ADE_MaxDeuteriumSetting".Translate(), -1F, "ADE_MaxDeuteriumSettingTooltip".Translate());
            ls.TextFieldNumeric(ref settings.maxDeuterium, ref settings.bufferMaxDeuterium, 1, int.MaxValue);
            ls.Label("ADE_MaxWaterSetting".Translate(), -1F, "ADE_MaxWaterSettingTooltip".Translate());
            ls.TextFieldNumeric(ref settings.maxWater, ref settings.bufferMaxWater, 1, int.MaxValue);
            ls.End();
        }

        public override void WriteSettings()
        {
            base.WriteSettings();
            CompDeuteriumProcessor.inverseEfficiency = 1F / settings.efficiency;
        }
    }
}
