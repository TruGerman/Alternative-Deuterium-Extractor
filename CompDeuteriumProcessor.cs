using DubsBadHygiene;
using EccentricPower;
using RimWorld;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using Verse;
using static AltDeuteriumExtractor.AltDeuteriumExtractorMod;

namespace AltDeuteriumExtractor
{
    class CompDeuteriumProcessor : ThingComp
    {
        private static readonly StringBuilder SB = new();
        //These temporarily store suitable comps during the tick cycle, almost cutting the tick time in half. This is always faster for lists with up to 50 elements
        private static readonly List<CompWaterStorage> WATER_TANKS = new();
        private static readonly List<CompFusionStorage> FUSION_TANKS = new();
        //Quick and dirty math hack to save performance, gets set from the main mod class on startup and recalculated whenever the efficiency is changed
        public static float inverseEfficiency, waterPerDay, deuteriumPerDay;
        private float storedWater, storedDeuterium, output = 1F;
        CompPipe waterPipe;
        CompPowerTrader power;
        CompFusionPipe fusionPipe;
        CompBreakdownable breakdown;

        public CompProperties_DeuteriumProcessor Props => (CompProperties_DeuteriumProcessor)props;

        public override void CompTick()
        {
            base.CompTick();
            power.PowerOutput = settings.powerDraw * output * -1F;
            if (!power.PowerOn || breakdown.BrokenDown) return;
            process();
            if (pushDeuterium() + pullWater() > settings.tolerance || canProcess()) return;
            power.PowerOutput = 0F;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool canProcess()
        {
            return storedWater > settings.tolerance && (settings.maxDeuterium - storedDeuterium) > settings.tolerance;
        }

        //Pushes Deuterium to the Deuterium tanks and returns the amount that was pushed
        private float pushDeuterium()
        {
            float toPush = pushDeuteriumToNet(fusionPipe.network, min(storedDeuterium, settings.waterPerTick * 1.5F));
            storedDeuterium -= toPush;
            return toPush;
        }

        //Pushes Deuterium to the network and returns the amount that was actually pushed
        private static float pushDeuteriumToNet(FusionNetwork net, float amount)
        {
            if (net == null) return 0F;
            float pushed = 0F;
            for (int i = 0; i < net.storage.Count; ++i)
            {
                var tank = net.storage[i];
                if (tank.deuteriumSpace > 0F && tank.loadingMode == FusionLoadingMode.AllowLoading) FUSION_TANKS.Add(tank);
            }

            if (FUSION_TANKS.Count == 0) return 0F;
            float toAdd = amount / (float)FUSION_TANKS.Count;
            for (int i = 0; i < FUSION_TANKS.Count; ++i)
            {
                var tank = FUSION_TANKS[i];
                float actuallyAdded = min(toAdd, tank.deuteriumSpace);
                tank.deuteriumStored += actuallyAdded;
                pushed += actuallyAdded;
            }

            FUSION_TANKS.Clear();
            return pushed;
        }

        //Pulls water from the net and adds it to the internal tank, returns the amount that was added
        private float pullWater()
        {
            float toExtract = pullWaterFromNet(waterPipe.pipeNet, clamp(settings.maxWater - storedWater, 0F, settings.waterPerTick * 1.5F));
            storedWater += toExtract;
            return toExtract;
        }

        private void process()
        {
            //Process as much Deuterium as possible, but limit it by how much water is available/allowed to process per tick
            float deuteriumToAdd = clamp(settings.maxDeuterium - storedDeuterium, 0F, min(storedWater, settings.waterPerTick) * settings.efficiency) * output;
            storedWater -= deuteriumToAdd * inverseEfficiency;
            storedDeuterium += deuteriumToAdd;
        }

        //Pulls water from the net and returns the amount that was actually removed
        private static float pullWaterFromNet(PlumbingNet net, float amount)
        {
            if (net == null) return 0F;
            float pushed = 0F;
            for (int i = 0; i < net.WaterTowers.Count; ++i)
            {
                var tank = net.WaterTowers[i];
                if (tank.WaterStorage > 0F) WATER_TANKS.Add(tank);
            }

            if (WATER_TANKS.Count == 0) return 0F;
            float toAdd = amount / (float)WATER_TANKS.Count;
            for (int i = 0; i < WATER_TANKS.Count; ++i)
            {
                var tank = WATER_TANKS[i];
                float actuallyAdded = min(toAdd, tank.WaterStorage);
                tank.WaterStorage -= actuallyAdded;
                pushed += actuallyAdded;
            }

            WATER_TANKS.Clear();
            return pushed;
        }

        //Sets the operating level via a crude method I bootlegged from vanilla
        private void setPowerLevel()
        {
            //It turns out that multiplying/dividing with ints is a lot slower than doing the same with floats
            Find.WindowStack.Add(new Dialog_Slider_Based(x => "ADE_EfficiencySlider".Translate(x, Mathf.RoundToInt(waterPerDay * (float)x * 0.01F), Mathf.Round(deuteriumPerDay * (float)x * 0.1F) * 0.1D), 1, 100, x =>
            {
                output = (float)x * 0.01F;
                power.PowerOutput = settings.powerDraw * output * -1F;
                recalculateDailyProcessingRates();
            }, Mathf.RoundToInt(output * 100F)));
        }

        public static void recalculateDailyProcessingRates()
        {
            waterPerDay = settings.waterPerTick * 60000F;
            deuteriumPerDay = waterPerDay * settings.efficiency;
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (var gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }

            Command_Action setPowerLevel = new()
            {
                action = this.setPowerLevel,
                defaultLabel = "ADE_PowerLevelGizmoLabel".Translate(),
                defaultDesc = "ADE_PowerLevelGizmoDescription".Translate(),
                icon = Textures.GIZMO_POWER_LEVEL
            };
            yield return setPowerLevel;
        }

        public override string CompInspectStringExtra()
        {
            SB.AppendLine(base.CompInspectStringExtra());
            SB.AppendLine("ADE_PowerLevel".Translate(Mathf.RoundToInt(output * 100F)));
            SB.AppendLine("ADE_WaterCounter".Translate(Mathf.Round(storedWater * 10F) * 0.1F, settings.maxWater));
            SB.AppendLine("ADE_DeuteriumCounter".Translate(Mathf.Round(storedDeuterium * 100F) * 0.01F, settings.maxDeuterium));
            SB.AppendLine("ADE_WaterPerDay".Translate(Mathf.RoundToInt(waterPerDay * output)));
            SB.AppendLine("ADE_DeuteriumPerDay".Translate(Mathf.Round(deuteriumPerDay * output * 100F) * 0.01F));
            string s = SB.ToString().Trim();
            SB.Clear();
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
            Scribe_Values.Look(ref output, "output");
        }

        //Custom math methods that hopefully get inlined to provide a tiny performance boost. Functionally identical to their Mathf counterparts
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float clamp(float value, float min, float max)
        {
            return value < min ? min : value > max ? max : value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float min(float value1, float value2)
        {
            return value1 > value2 ? value2 : value1;
        }
    }
}