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
            CompDeuteriumProcessor.inverseEfficiency = 1 / settings.efficiency;
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
            ls.TextFieldNumeric( ref settings.waterPerTick, ref settings.bufferWaterPerTick, 0.001F, float.MaxValue);
            ls.Label("ADE_PowerDrawSetting".Translate(), -1f, "ADE_PowerDrawSettingTooltip".Translate());
            ls.TextFieldNumeric(ref settings.powerDraw, ref settings.bufferPowerDraw, float.MinValue, float.MaxValue);
            ls.Label("ADE_MaxDeuteriumSetting".Translate(), -1F, "ADE_MaxDeuteriumSettingTooltip".Translate());
            ls.TextFieldNumeric(ref settings.maxDeuterium, ref settings.bufferMaxDeuterium, 1, int.MaxValue);
            ls.Label("ADE_MaxWaterSetting".Translate(), -1F, "ADE_MaxWaterSettingTooltip".Translate());
            ls.TextFieldNumeric( ref settings.maxWater, ref settings.bufferMaxWater, 1, int.MaxValue);
            ls.End();
        }

        public override void WriteSettings()
        {
            base.WriteSettings();
            CompDeuteriumProcessor.inverseEfficiency = 1/settings.efficiency;
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

    [StaticConstructorOnStartup]
    public class Textures
    {
        public static readonly Texture2D GIZMO_POWER_LEVEL = ContentFinder<Texture2D>.Get("UI/Commands/ADE_GizmoSetPowerLevel");
    }
}
