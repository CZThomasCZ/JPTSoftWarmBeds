﻿using HarmonyLib;
using RimWorld;
using Verse;

namespace SoftWarmBeds;

//Makes the bed stats cover for a possible lack of apparel when calculating comfortable temperature range for pawns in bed
[HarmonyPatch(typeof(GenTemperature), "ComfortableTemperatureRange", typeof(Pawn))]
public class ComfortableTemperatureRange_Patch
{
    public static void Postfix(Pawn p, ref FloatRange __result)
    {
        if (!p.InBed())
        {
            return;
        }

        var bed = p.CurrentBed();
        var InsulationCold = bed.GetStatValue(BedStatDefOf.Bed_Insulation_Cold);
        var InsulationHeat = bed.GetStatValue(BedStatDefOf.Bed_Insulation_Heat);
        if (InsulationCold == 0 && InsulationHeat == 0)
        {
            return;
        }

        var raceDef = p.kindDef.race;
        var altResult = new FloatRange(raceDef.GetStatValueAbstract(StatDefOf.ComfyTemperatureMin),
            raceDef.GetStatValueAbstract(StatDefOf.ComfyTemperatureMax));
        altResult.min -= InsulationCold;
        altResult.max += InsulationHeat;
        if (__result.min > altResult.min)
        {
            __result.min = altResult.min;
        }

        if (__result.max < altResult.max)
        {
            __result.max = altResult.max;
        }
        //Log.Message(bed+" insulation cold is "+bed.GetStatValue(BedInsulationCold.Bed_Insulation_Cold, true));
        //Log.Message("comfortable range modified for " + p + " by bed" + bed + ": " + __result.ToString());
    }
}