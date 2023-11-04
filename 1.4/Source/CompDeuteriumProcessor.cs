using System;
using DubsBadHygiene;
using EccentricPower;
using RimWorld;
using static UnityEngine.Mathf;
using System.Text;
using Verse;
using static AltDeuteriumExtractor.AltDeuteriumExtractorMod;

namespace AltDeuteriumExtractor
{
    class CompDeuteriumProcessor : ThingComp
    {
        private static readonly StringBuilder Sb = new();
        public static float waterPerTickAmplified, waterPerTick, efficiency, deuteriumPushedPerTick, waterPulledPerTick; //How much caching is too much? Yes
        public float storedWater, storedDeuterium;
        public CompPipe waterPipe;
        public CompPowerTrader power;
        public CompFusionPipe fusionPipe;
        public CompBreakdownable breakdown;

        public CompProperties_DeuteriumProcessor Props => (CompProperties_DeuteriumProcessor)props;

        public override void CompTick()
        {
            if ((power != null && !power.PowerOn) || (breakdown != null && breakdown.BrokenDown)) return;
            Process();
            PullWaterOptimized();
            PushDeuteriumOptimized();
        }


        private void PushDeuteriumOptimized()
        {
            if (fusionPipe?.network is not FusionNetwork net) return;
            float left = Min(storedDeuterium, deuteriumPushedPerTick);
            float ogLeft = left;
            for (int i = 0; i < net.storage.Count; ++i)
            {
                var tank = net.storage[i];
                if (tank.deuteriumSpace <= 0F || tank.loadingMode != FusionLoadingMode.AllowLoading) continue;
                float actuallyAdded = Min(left, tank.deuteriumSpace);
                tank.deuteriumStored += actuallyAdded;
                left -= actuallyAdded;
                if (left <= 0F) break;
            }
            storedDeuterium -= (ogLeft - left);
        }

        private void PullWaterOptimized()
        {
            if (waterPipe?.pipeNet is not PlumbingNet net) return;
            float left = Clamp(settings.maxWater - storedWater, 0, waterPulledPerTick);
            float ogLeft = left;
            for (int i = 0; i < net.WaterTowers.Count; ++i)
            {
                var tank = net.WaterTowers[i];
                if (tank.WaterStorage <= 0F) continue;
                float actuallyAdded = Min(left, tank.WaterStorage);
                tank.WaterStorage -= actuallyAdded;
                left -= actuallyAdded;
                if(left <= 0F) break;
            }
            storedWater += (ogLeft - left);
        }

        private void Process()
        {
            //Process as much Deuterium as possible, but limit it by how much water is available/allowed to process per tick
            float deuteriumToAdd = Clamp(settings.maxDeuterium - storedDeuterium, 0F, Min(storedWater, waterPerTick) * efficiency);
            storedWater -= deuteriumToAdd / efficiency;
            storedDeuterium += deuteriumToAdd;
        }

        public static void RefreshValues()
        {
            efficiency = (float)((double)settings.deuteriumPerDay / (double)settings.waterPerDay);
            waterPerTick = (float)((double)settings.waterPerDay / 60000D); //Yes, this does actually help
            waterPerTickAmplified = (float)((double)waterPerTick * 1.5D);
            waterPulledPerTick = Max(1F, waterPerTickAmplified);
            deuteriumPushedPerTick = Max(1F, waterPerTickAmplified * efficiency);
        } 

        public override string CompInspectStringExtra()
        {
            Sb.AppendLine("ADE_WaterCounter".Translate(Math.Round(storedWater, 2), settings.maxWater));
            Sb.AppendLine("ADE_DeuteriumCounter".Translate(Math.Round(storedDeuterium, 2), settings.maxDeuterium));
            Sb.AppendLine("ADE_WaterPerDay".Translate(Math.Round(settings.waterPerDay, 2)));
            Sb.AppendLine("ADE_DeuteriumPerDay".Translate(Math.Round(settings.deuteriumPerDay, 2)));
            string s = Sb.ToString().Trim();
            Sb.Clear();
            return s;
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            waterPipe = parent.GetComp<CompPipe>();
            fusionPipe = parent.GetComp<CompFusionPipe>();
            power = parent.GetComp<CompPowerTrader>();
            breakdown = parent.GetComp<CompBreakdownable>();
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref storedWater, "storedWater");
            Scribe_Values.Look(ref storedDeuterium, "storedDeuterium");
        }

    }
}