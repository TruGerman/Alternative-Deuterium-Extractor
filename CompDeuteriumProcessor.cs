﻿using System;
using DubsBadHygiene;
using EccentricPower;
using RimWorld;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Verse;
using static AltDeuteriumExtractor.AltDeuteriumExtractorMod;

namespace AltDeuteriumExtractor
{
    class CompDeuteriumProcessor : ThingComp
    {
        public static readonly StringBuilder SB = new();
        //These temporarily store suitable comps during the tick cycle, almost cutting the tick time in half. This is always faster for lists with up to 50 elements
        protected static readonly List<CompWaterStorage> WATER_TANKS = new();
        protected static readonly List<CompFusionStorage> FUSION_TANKS = new();
        //Quick and dirty math hack to save performance, gets set from the main mod class on startup and recalculated whenever the efficiency is changed
        public static float inverseEfficiency;
        public float storedWater, storedDeuterium, output = 1F;
        public CompPipe waterPipe;
        public CompPowerTrader power;
        public CompFusionPipe fusionPipe;
        public CompBreakdownable breakdown;

        public CompProperties_DeuteriumProcessor Props => (CompProperties_DeuteriumProcessor)props;

        public override void CompTick()
        {
            base.CompTick();
            if (!power.PowerOn || breakdown.BrokenDown) return;
            process();
            if (pushDeuterium() + pullWater() < 0.01F && !canProcess())
            {
                power.PowerOutput = 0F;
                return;
            }
            if (power.PowerOutput == 0F)
            {
                power.PowerOutput = settings.powerDraw * output * -1F;
            }
        }

        //Includes a margin to account for rounding errors
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual bool canProcess()
        {
            return storedWater > 0.005F && (settings.maxDeuterium - storedDeuterium) > 0.005F;
        }

        //Pushes Deuterium to the Deuterium tanks and returns the amount that was pushed
        public virtual float pushDeuterium()
        {
            float toPush = pushDeuteriumToNet(fusionPipe.network, min(storedDeuterium, settings.waterPerTick * 1.5F));
            storedDeuterium -= toPush;
            return toPush;
        }

        //Pushes Deuterium to the network and returns the amount that was actually pushed
        public static float pushDeuteriumToNet(FusionNetwork net, float amount)
        {
            if (net == null) return 0F;
            float pushed = 0F;
            for (int i = 0; i < net.storage.Count; ++i)
            {
                var tank = net.storage[i];
                if (tank.deuteriumSpace > 0F && tank.loadingMode == FusionLoadingMode.AllowLoading) FUSION_TANKS.Add(tank);
            }
            if (FUSION_TANKS.NullOrEmpty()) return 0F;
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
        public virtual float pullWater()
        {
            float toExtract = pullWaterFromNet(waterPipe.pipeNet, clamp(settings.maxWater - storedWater, 0F, settings.waterPerTick * 1.5F));
            storedWater += toExtract;
            return toExtract;
        }

        //Runs the conversion
        public virtual void process()
        {
            //Process as much Deuterium as possible, but limit it by how much water is available/allowed to process per tick
            float deuteriumToAdd = clamp(settings.maxDeuterium - storedDeuterium, 0F, min(storedWater, settings.waterPerTick) * settings.efficiency) * output;
            storedWater -= deuteriumToAdd * inverseEfficiency;
            storedDeuterium += deuteriumToAdd;
        }

        //Pulls water from the net and returns the amount that was actually removed
        public static float pullWaterFromNet(PlumbingNet net, float amount)
        {
            if (net == null) return 0F;
            float pushed = 0F;
            for (int i = 0; i < net.WaterTowers.Count; ++i)
            {
                var tank = net.WaterTowers[i];
                if (tank.WaterStorage > 0F) WATER_TANKS.Add(tank);
            }
            if (WATER_TANKS.NullOrEmpty()) return 0F;
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
        public void setPowerLevel()
        {
            //The output is being converted to a float here because it turns out that multiplying/dividing with ints is a lot slower than doing the same with floats
            Find.WindowStack.Add(new Dialog_Slider((x) => "ADE_EfficiencySlider".Translate(x), 1, 100, x =>
            {
                output = (float)x * 0.01F;
                power.PowerOutput = settings.powerDraw * output * -1F;
            }, (int)Math.Round(output * 100F)));
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
            SB.AppendLine("ADE_PowerLevel".Translate(Math.Round(output * 100F)));
            SB.AppendLine("ADE_WaterCounter".Translate(Math.Round(storedWater, 1), settings.maxWater));
            SB.AppendLine("ADE_DeuteriumCounter".Translate(Math.Round(storedDeuterium, 2), settings.maxDeuterium));
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
        public static float clamp(float value, float min, float max)
        {
            return value < min ? min : value > max ? max : value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float min(float value1, float value2)
        {
            return value1 > value2 ? value2 : value1;
        }
    }
}
