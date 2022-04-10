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
            applySettings();
            recalculateReciprocal();
            refreshDeuteriumPerDay();
            recalculateDailyProcessingRates();
        }

        public override string SettingsCategory()
        {
            return "ADE_settingsCategoryLabel".Translate();
        }

        //This is scuffed
        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            Listing_Standard ls = new();
            ls.Begin(inRect);
            bool advancedOverrideCache = settings.advancedOverride;
            ls.CheckboxLabeled("ADE_AdvancedSetting".Translate(), ref settings.advancedMode, "ADE_AdvancedSettingTooltip".Translate());
            ls.CheckboxLabeled("ADE_AdvancedOverrideSetting".Translate(), ref settings.advancedOverride, "ADE_AdvancedOverrideSettingTooltip".Translate());
            ls.Gap(15F);
            ls.ColumnWidth = 600F;
            if (settings.advancedMode) renderAdvancedSettings(ls);
            else renderBasicSettings(ls);
            ls.Gap(80F);
            ls.ColumnWidth = 100F;
            if (ls.ButtonText("ADE_ResetButtonLabel".Translate())) resetSettings();
            if(settings.advancedOverride != advancedOverrideCache) applySettings();
        }

        private static void renderBasicSettings(Listing_Standard ls)
        {
            var oldSettings = getSettings();
            ls.Label("ADE_PowerDrawSetting".Translate(Mathf.RoundToInt(settings.powerDrawBasic)), -1F, "ADE_PowerDrawSettingTooltip".Translate());
            settings.powerDrawBasic = ls.Slider(Mathf.RoundToInt(settings.powerDrawBasic * 0.02F) * 50F, 0F, 5000F);
            ls.Label("ADE_MaxDeuteriumSetting".Translate(settings.maxDeuteriumBasic), -1F, "ADE_MaxDeuteriumSettingTooltip".Translate());
            settings.maxDeuteriumBasic = Mathf.RoundToInt(ls.Slider(settings.maxDeuteriumBasic, 1F, 250F));
            ls.Label("ADE_MaxWaterSetting".Translate(settings.maxWaterBasic), -1F, "ADE_MaxWaterSettingTooltip".Translate());
            settings.maxWaterBasic = Mathf.RoundToInt(ls.Slider(settings.maxWaterBasic * 0.1F, 1F, 100F)) * 10;
            ls.Label("ADE_WaterPerDaySetting".Translate(Mathf.RoundToInt(settings.waterPerTickBasic * 60000F)), -1F, "ADE_WaterPerDaySettingTooltip".Translate());
            settings.waterPerTickBasic = ls.Slider(Mathf.RoundToInt(settings.waterPerTickBasic * 600F) * 100F, 100F, 15000F) / 60000F;
            ls.Label("ADE_DeuteriumPerDaySetting".Translate(Mathf.RoundToInt(settings.efficiencyBasic * 60000F * settings.waterPerTickBasic)), -1F, "ADE_DeuteriumPerDaySettingTooltip".Translate());
            settings.deuteriumPerDay = Mathf.RoundToInt(ls.Slider(settings.deuteriumPerDay, 1F, 100F));
            var newSettings = getSettings();
            if (settingsChanged(oldSettings, newSettings))
            {
                if (oldSettings[3] == newSettings[3]) refreshEfficiency();
                if (oldSettings[5] == newSettings[5]) refreshDeuteriumPerDay();
            }
            ls.End();
        }

        private static void renderAdvancedSettings(Listing_Standard ls)
        {
            var oldSettings = getSettings();
            ls.Label("ADE_PowerDrawAdvancedSetting".Translate(settings.powerDrawAdvanced), -1f, "ADE_PowerDrawSettingTooltip".Translate());
            ls.TextFieldNumeric(ref settings.powerDrawAdvanced, ref settings.bufferPowerDrawAdvanced, 0F, float.MaxValue);
            ls.Label("ADE_MaxDeuteriumAdvancedSetting".Translate(settings.maxDeuteriumAdvanced), -1F, "ADE_MaxDeuteriumSettingTooltip".Translate());
            ls.TextFieldNumeric(ref settings.maxDeuteriumAdvanced, ref settings.bufferMaxDeuteriumAdvanced, 1F, float.MaxValue);
            ls.Label("ADE_MaxWaterAdvancedSetting".Translate(settings.maxWaterAdvanced), -1F, "ADE_MaxWaterSettingTooltip".Translate());
            ls.TextFieldNumeric(ref settings.maxWaterAdvanced, ref settings.bufferMaxWaterAdvanced, 1F, float.MaxValue);
            ls.Label("ADE_EfficiencySetting".Translate(), -1F, "ADE_EfficiencySettingTooltip".Translate());
            ls.TextFieldNumeric(ref settings.efficiencyAdvanced, ref settings.bufferEfficiencyAdvanced, 0, float.MaxValue);
            ls.Label("ADE_WaterPerTickSetting".Translate(), -1F, "ADE_WaterPerTickSettingTooltip".Translate());
            ls.TextFieldNumeric(ref settings.waterPerTickAdvanced, ref settings.bufferWaterPerTickAdvanced, 0, float.MaxValue);
            ls.Label("ADE_DeuteriumPerDayAdvancedSetting".Translate(), -1F, "ADE_DeuteriumPerDayAdvancedSettingTooltip".Translate());
            ls.TextFieldNumeric(ref settings.deuteriumPerDayAdvanced, ref settings.bufferDeuteriumPerDayAdvanced, 0, float.MaxValue);
            var newSettings = getSettings();
            if (settingsChanged(oldSettings, newSettings))
            {
                if(oldSettings[3] == newSettings[3]) refreshEfficiency();
                if(oldSettings[5] == newSettings[5]) refreshDeuteriumPerDay();
            }

            if (settings.efficiencyAdvanced == 0 && settings.advancedOverride)
            {
                Color cachedColor = GUI.color;
                ls.Gap();
                GUI.color = Color.red;
                ls.Label("ADE_DivideByZeroError".Translate());
                GUI.color = cachedColor;
            }

            ls.End();
        }

        public override void WriteSettings()
        {
            base.WriteSettings();
            applySettings();
            recalculateDailyProcessingRates();
            recalculateReciprocal();
        }

        private static void refreshEfficiency()
        {
            if (settings.advancedMode)
            {
                if (settings.waterPerTickAdvanced == 0) return;
                settings.efficiencyAdvanced = (float)settings.deuteriumPerDayAdvanced / (settings.waterPerTickAdvanced * 60000F);
                settings.bufferEfficiencyAdvanced = settings.efficiencyAdvanced.ToString();
                return;
            }
            settings.efficiencyBasic = (float)settings.deuteriumPerDay / (settings.waterPerTickBasic * 60000F);
        } 

        private static void refreshDeuteriumPerDay()
        {
            settings.deuteriumPerDay = Mathf.RoundToInt(settings.waterPerTickBasic * 60000F * settings.efficiencyBasic);
            settings.deuteriumPerDayAdvanced = Mathf.Round(settings.waterPerTickAdvanced * 60000F * settings.efficiencyAdvanced * 100000) * 0.00001F;
            settings.bufferDeuteriumPerDayAdvanced = settings.deuteriumPerDayAdvanced.ToString();
        }

        private static void recalculateReciprocal() => inverseEfficiency = settings.efficiency == 0 ? 10000000000F : 1F / settings.efficiency;


        private static void resetSettings()
        {
            if (settings.advancedMode)
            {
                settings.powerDrawAdvanced = 1200F;
                settings.bufferPowerDrawAdvanced = "1200";
                settings.maxDeuteriumAdvanced = 50;
                settings.bufferMaxDeuteriumAdvanced = "50";
                settings.maxWaterAdvanced = 500;
                settings.bufferMaxWaterAdvanced = "500";
                settings.waterPerTickAdvanced = 0.083333F;
                settings.bufferWaterPerTickAdvanced = "0.083333";
                settings.efficiencyAdvanced = 0.005F;
                settings.bufferEfficiencyAdvanced = "0.005";
                settings.deuteriumPerDayAdvanced = settings.waterPerTickAdvanced * 60000F * settings.efficiencyAdvanced;
                settings.bufferDeuteriumPerDayAdvanced = settings.deuteriumPerDayAdvanced.ToString();
                return;
            }
            settings.deuteriumPerDay = 25;
            settings.powerDrawBasic = 1200F;
            settings.maxDeuteriumBasic = 50;
            settings.maxWaterBasic = 500;
            settings.waterPerTickBasic = 0.083333F;
            settings.efficiencyBasic = 0.005F;
        }

        private static void applySettings()
        {
            if (settings.advancedOverride)
            {
                settings.powerDraw = settings.powerDrawAdvanced;
                settings.maxDeuterium = settings.maxDeuteriumAdvanced;
                settings.maxWater = settings.maxWaterAdvanced;
                settings.waterPerTick = settings.waterPerTickAdvanced;
                settings.efficiency = settings.efficiencyAdvanced;
                return;
            }

            settings.powerDraw = settings.powerDrawBasic;
            settings.maxDeuterium = settings.maxDeuteriumBasic;
            settings.maxWater = settings.maxWaterBasic;
            settings.waterPerTick = settings.waterPerTickBasic;
            settings.efficiency = settings.efficiencyBasic;
        }

        private static float[] getSettings()
        {
            if (settings.advancedMode) return new []
            {
                settings.powerDrawAdvanced, settings.maxDeuteriumAdvanced, settings.maxWaterAdvanced,
                settings.efficiencyAdvanced, settings.waterPerTickAdvanced, settings.deuteriumPerDayAdvanced
            };
            return new []
            {
                settings.powerDrawBasic, settings.maxDeuteriumBasic, settings.maxWaterBasic,
                settings.efficiencyBasic, settings.waterPerTickBasic, settings.deuteriumPerDay
            };
        }

        private static bool settingsChanged(float[] oldSettings, float[] newSettings)
        {
            for (int i = 0; i < oldSettings.Length; i++)
            {
                if (oldSettings[i] == newSettings[i]) continue;
                return true;
            }

            return false;
        }

    }
}