﻿using HarmonyLib;
using System;
using ValheimPlus.Configurations;
using ValheimPlus.RPC;

namespace ValheimPlus.GameClasses
{
    /// <summary>
    /// Sync server config to clients
    /// </summary>
    [HarmonyPatch(typeof(Game), nameof(Game.Start))]
    public static class Game_Start_Patch
    {
        private static void Prefix()
        {
            ZRoutedRpc.instance.Register("VPlusConfigSync", new Action<long, ZPackage>(VPlusConfigSync.RPC_VPlusConfigSync)); //Config Sync
            ZRoutedRpc.instance.Register("VPlusMapSync", new Action<long, ZPackage>(VPlusMapSync.RPC_VPlusMapSync)); //Map Sync
            ZRoutedRpc.instance.Register("VPlusAck", new Action<long>(VPlusAck.RPC_VPlusAck)); //Ack
        }
    }


    /// <summary>
    /// Alter game difficulty damage scale
    /// </summary>
    [HarmonyPatch(typeof(Game), nameof(Game.GetDifficultyDamageScale))]
    public static class Game_GetDifficultyDamageScale_Patch
    {
        private static float baseDifficultyDamageScale = 0.04f;

        private static void Postfix(ref float __result)
        {
            if (Configuration.Current.Game.IsEnabled)
                __result = ((__result - 1f) / baseDifficultyDamageScale * Configuration.Current.Game.gameDifficultyDamageScale) + 1f;
        }
    }


    /// <summary>
    /// Disable the "i have arrived" message on spawn.
    /// </summary>
    [HarmonyPatch(typeof(Game), nameof(Game.UpdateRespawn))]
    public static class Game_UpdateRespawn_Patch
    {
        private static void Prefix(ref Game __instance, float dt)
        {
            if (Configuration.Current.Player.IsEnabled && !Configuration.Current.Player.iHaveArrivedOnSpawn)
                __instance.m_firstSpawn = false;
        }
    }

    /// <summary>
    /// Alter game difficulty health scale
    /// </summary>
    [HarmonyPatch(typeof(Game), nameof(Game.GetDifficultyHealthScale))]
    public static class Game_GetDifficultyHealthScale_Patch
    {
        private static float baseDifficultyHealthScale = 0.4f;

        private static void Postfix(ref float __result)
        {
            if (Configuration.Current.Game.IsEnabled)
                __result = ((__result - 1f) / baseDifficultyHealthScale * Configuration.Current.Game.gameDifficultyHealthScale) + 1f;
        }
    }

    /// <summary>
    /// Alter player difficulty scale
    /// </summary>
    [HarmonyPatch(typeof(Game), nameof(Game.GetPlayerDifficulty))]
    public static class Game_GetPlayerDifficulty_Patch
    {
        private static void Postfix(ref int __result)
        {
            if (Configuration.Current.Game.IsEnabled)
            {
                if (Configuration.Current.Game.setFixedPlayerCountTo > 0)
                {
                    __result = Configuration.Current.Game.setFixedPlayerCountTo + Configuration.Current.Game.extraPlayerCountNearby;
                    return;
                }
                __result += Configuration.Current.Game.extraPlayerCountNearby;
            }
        }
    }
}