using UnityEngine;
using Verse;
using static AltDeuteriumExtractor.CompDeuteriumProcessor;

namespace AltDeuteriumExtractor
{
    public class AltDeuteriumExtractorMod : Mod
    {
        public static AltDeuteriumExtractorModSettings settings;

        public AltDeuteriumExtractorMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<AltDeuteriumExtractorModSettings>();
            RefreshValues();
        }

        public override string SettingsCategory() => "ADE_SettingsCategoryLabel".Translate();

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            Listing_Standard ls = new()
            {
                ColumnWidth = inRect.width / 2F,
            };
            ls.Begin(inRect);
            ls.TextFieldNumericLabeled("ADE_MaxDeuteriumSetting".Translate(), ref settings.maxDeuterium, ref settings.bufferMaxDeuterium, 1F, 1E+10F);
            ls.TextFieldNumericLabeled("ADE_MaxWaterSetting".Translate(), ref settings.maxWater, ref settings.bufferMaxWater, 1F, 1E+10F);
            ls.TextFieldNumericLabeled("ADE_WaterPerDaySetting".Translate(), ref settings.waterPerDay, ref settings.bufferWaterPerDay, 0.01F, 1E+10F);
            ls.TextFieldNumericLabeled("ADE_DeuteriumPerDaySetting".Translate(), ref settings.deuteriumPerDay, ref settings.bufferDeuteriumPerDay, 0.01F, 1E+10F);
            if (Widgets.ButtonText(new Rect(new Vector2((ls.ColumnWidth - 100F) / 2F, ls.curY), new Vector2(100F, 40F)), "Reset".Translate())) ResetSettings();
            ls.End();
        }

        public override void WriteSettings()
        {
            base.WriteSettings();
            RefreshValues();
        }

        private static void ResetSettings()
        {
            settings.bufferMaxDeuterium = "50";
            settings.bufferMaxWater = "500";
            settings.bufferDeuteriumPerDay = "25";
            settings.bufferWaterPerDay = "5000";
            RefreshValues();
        }
    }
}