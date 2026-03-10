using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace CasualTweaks;

[BepInPlugin("com.laymxd.casualtweaks", "CasualTweaks", "1.3.0")]
public class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource Log;

    // Config entries
    ConfigEntry<float> fuelCapacityMultiplier;
    ConfigEntry<float> energyCapacityMultiplier;
    ConfigEntry<float> maxSpeedScaleMultiplier;
    ConfigEntry<float> jobPriceMultiplier;

    ConfigEntry<bool> doFuelAndEnergyChanges;
    ConfigEntry<bool> doSpeedChanges;
    ConfigEntry<bool> doPriceChanges;

    void Awake()
    {
        Log = Logger;
        Log.LogInfo("CasualTweaks loaded");

        // Create config entries
        fuelCapacityMultiplier = Config.Bind(
            "Multipliers",
            "FuelCapacityMultiplier",
            2f,
            "Multiplier applied to vehicle fuel capacity"
        );

        energyCapacityMultiplier = Config.Bind(
            "Multipliers",
            "EnergyCapacityMultiplier",
            2f,
            "Multiplier applied to vehicle energy capacity"
        );

        maxSpeedScaleMultiplier = Config.Bind(
            "Multipliers",
            "MaxSpeedScaleMultiplier",
            4f,
            "Multiplier applied to vehicle max speed"
        );

        jobPriceMultiplier = Config.Bind(
            "Multipliers",
            "JobPriceMultiplier",
            6f,
            "Multiplier applied to job payouts"
        );

        doFuelAndEnergyChanges = Config.Bind(
            "Toggles",
            "EnableFuelAndEnergyChanges",
            true,
            "Enable fuel and energy capacity changes"
        );

        doSpeedChanges = Config.Bind(
            "Toggles",
            "EnableSpeedChanges",
            false,
            "Enable car speed multiplier"
        );

        doPriceChanges = Config.Bind(
            "Toggles",
            "EnablePriceChanges",
            true,
            "Enable job price multiplier"
        );

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Log.LogInfo($"Scene loaded: {scene.name}");

        string[] menuScenes =
        {
            "TitleScreen",
            "Credits",
            "Ending",
            "TrainIntro"
        };

        if (System.Array.IndexOf(menuScenes, scene.name) != -1)
            return;

        var runnerObj = new GameObject("CasualTweaksRunner");
        var runner = runnerObj.AddComponent<CoroutineRunner>();

        runner.StartCoroutine(DelayedSetup());
    }

    IEnumerator DelayedSetup()
    {
        yield return new WaitForSeconds(2f);
        ApplyTweaks();
    }

    void ApplyTweaks()
    {
        Log.LogInfo("Applying gameplay tweaks");

        var hud = FindObjectOfType<sHUD>();
        var car = FindObjectOfType<sCarController>();
        var jobBoard = FindObjectOfType<jobBoard>();

        if (hud != null && doFuelAndEnergyChanges.Value)
        {
            hud.fuelCapacity *= fuelCapacityMultiplier.Value;
            hud.energyCapacity *= energyCapacityMultiplier.Value;
            Log.LogInfo("Fuel/Energy tweaked");
        }
        else Log.LogWarning("HUD not found or fuel/energy disabled");

        if (car != null && doSpeedChanges.Value)
        {
            car.maxSpeedScale *= maxSpeedScaleMultiplier.Value;
            Log.LogInfo("Car speed tweaked");
        }
        else Log.LogWarning("Car controller not found or speed disabled");

        if (jobBoard != null && doPriceChanges.Value)
        {
            jobBoard.priceBase *= jobPriceMultiplier.Value;
            jobBoard.pricePerKM *= jobPriceMultiplier.Value;
            Log.LogInfo("Job prices tweaked");
        }
        else Log.LogWarning("JobBoard not found or price changes disabled");
    }
}

public class CoroutineRunner : MonoBehaviour { }