using HarmonyLib;
using OriBattleRoyal;
using Orion;
using OriPlayer;
using OriPlayerAction;
using OriUnit;
using UnityEngine;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local

namespace Sonic4;

public class Patcher
{
    public static void DoPatching()
    {
        var harmony = new Harmony("com.orion.sonic4");
        harmony.PatchAll();
    }
}

[HarmonyPatch(typeof(PlayerSonic2D), nameof(PlayerSonic2D.InitAction))]
class SonicInitActionPatch
{
    static void Postfix(PlayerSonic2D __instance)
    {
        var bounceAction = new PlActionBounceSonic();
        var actionId = (PlayerBase.EActionIndex)300;
        __instance.SetUpAction(ref actionId, bounceAction);
    }
}

[HarmonyPatch(typeof(PlActionJumpUniqueTails), nameof(PlActionJumpUniqueTails.Update))]
class FlyUpdate
{
    static bool Prefix(PlActionJumpUniqueTails __instance)
    {
        bool isJumpTriggered = __instance.ownerPlyer.IsJumpTriggered();
        var inpPlyCtrl = __instance.ownerPlyer.inputPlyCtlr;
        var nowPad = inpPlyCtrl.playerPad.nowPad;
        if (SysSaveDataKeyboardConfig.IsOnAction(SysSaveDataKeyboardConfig.EAction.MoveDown, nowPad) && isJumpTriggered)
        {
            var actionId = PlayerBase.EActionIndex.ActJumpStampBound;
            __instance.ChangeAction(ref actionId);
            return false;
        }

        return true;
    }
}

[HarmonyPatch(typeof(PlActionKnucklesGlideLanding), nameof(PlActionKnucklesGlideLanding.Update))]
class GlideLandUpdate
{
    static bool Prefix(PlActionKnucklesGlideLanding __instance)
    {
        bool isJumpTriggered = __instance.ownerPlyer.IsJumpTriggered();
        var inpPlyCtrl = __instance.ownerPlyer.inputPlyCtlr;
        var nowPad = inpPlyCtrl.playerPad.nowPad;
        if (SysSaveDataKeyboardConfig.IsOnAction(SysSaveDataKeyboardConfig.EAction.MoveDown, nowPad) && isJumpTriggered)
        {
            var actionId = PlayerBase.EActionIndex.ActSpinDashCharge;
            __instance.ChangeAction(ref actionId);
            return false;
        }

        return true;
    }
}

[HarmonyPatch(typeof(PlActionKnucklesGlideFallLanding), nameof(PlActionKnucklesGlideFallLanding.Update))]
class GlideFallLandUpdate
{
    static bool Prefix(PlActionKnucklesGlideFallLanding __instance)
    {
        bool isJumpTriggered = __instance.ownerPlyer.IsJumpTriggered();
        var inpPlyCtrl = __instance.ownerPlyer.inputPlyCtlr;
        var nowPad = inpPlyCtrl.playerPad.nowPad;
        if (SysSaveDataKeyboardConfig.IsOnAction(SysSaveDataKeyboardConfig.EAction.MoveDown, nowPad) && isJumpTriggered)
        {
            var actionId = PlayerBase.EActionIndex.ActSpinDashCharge;
            __instance.ChangeAction(ref actionId);
            return false;
        }

        return true;
    }
}

[HarmonyPatch(typeof(PlActionBase2D), nameof(PlActionBase2D.UpdateUniqueSonic))]
class UpdateUniqueSonic
{
    static bool Prefix(PlActionBase2D __instance)
    {
        if (!__instance.firstUpdateUniqActCheck)
        {
            __instance.firstUpdateUniqActCheck = __instance.ownerPlyer.IsJumpTriggered();
        }
        if (__instance.IsUniqueActBase && __instance.ownerPlyer.IsRoll && __instance.firstUpdateUniqActCheck)
        {
            bool isJumpPress = __instance.ownerPlyer.IsJumpPress();
            if (isJumpPress)
            {
                if (__instance.animIputRate > 0)
                {
                    __instance.animIputRate += __instance.ownerPlyer.GetDeltaTime;
                    if (__instance.animIputRate >= 0.3666)
                    {
                        var actionId = PlayerBase.EActionIndex.ActJumpUnique;
                        __instance.ChangeAction(ref actionId);
                    }
                }
                else
                {
                    __instance.animIputRate = (float)(__instance.ownerPlyer.GetDeltaTime + 0.016667);
                }
            }
            else if (__instance.animIputRate > 0)
            {
                var actionId = (PlayerBase.EActionIndex)300;
                __instance.ChangeAction(ref actionId);
            }
        }
        return false;
    }
}

[HarmonyPatch(typeof(PlActionBase2D), nameof(PlActionBase2D.UpdateJumpLanding))]
class AmyJumpPatch
{
    static bool Prefix(PlActionBase2D __instance)
    {
        if (!__instance.ownerPlyer.IsJumpPress() || __instance.ownerPlyer.IsJumpTriggered()) return true;
        if (__instance.ownerPlyer.playerType != PlayerBase.EPlayerType.Amy) return true;
        var actionId = PlayerBase.EActionIndex.ActAmyHammer;
        __instance.ChangeAction(ref actionId);
        return false;
    }
}

[HarmonyPatch(typeof(PlActionSpinDashCharge), nameof(PlActionSpinDashCharge.Update))]
class SpinDashChargeUpdatePatch
{
    static bool Prefix(PlActionSpinDashCharge __instance)
    {
        __instance.CallBase<PlActionBase2D>(nameof(PlActionBase2D.Update));

        if (__instance.ownerPlyer.IsOnGround)
        {
            if (__instance.ownerPlyer.IsJumpTriggered())
            {
                __instance.addCharge = true;
            }

            if (__instance.ownerPlyer.IsDirDown()) return false;
            var action = PlayerBase.EActionIndex.ActGmkRailSlide;

            if (!__instance.ownerPlyer.IsTransitActRideSlide())
            {
                action = __instance.GetType() == typeof(PlActionBRSpinDashCharge) ? PlayerBase.EActionIndex.ActRun : PlayerBase.EActionIndex.ActSpinDash;
            }
                
            __instance.ChangeAction(ref action);
        }

        return false;
    }
}

[HarmonyPatch(typeof(PlActionSpinDashCharge), nameof(PlActionSpinDashCharge.FixedUpdate))]
class SpinDashChargeFixedUpdatePatch
{
    static bool Prefix(PlActionSpinDashCharge __instance)
    {
        __instance.CallBase<PlActionBase2D>(nameof(PlActionBase2D.FixedUpdate));
        
        var chargePowerAdd = __instance.ownerPlyer.dataCmp.commonDBStats.ChargePowerAdd;
        var chargePowerMax = __instance.ownerPlyer.dataCmp.commonDBStats.ChargePowerMax;
        var chargePowerBrake = __instance.ownerPlyer.dataCmp.commonDBStats.ChargePowerBrakeRate;

        var typeBit = DB_PlayerStats.ETypeBit.Super;
        if (__instance.ownerPlyer.IsActDb(ref typeBit))
        {
            chargePowerAdd = __instance.ownerPlyer.dataCmp.commonDBStats.SuperChargePowerAdd;
            chargePowerMax = __instance.ownerPlyer.dataCmp.commonDBStats.SuperChargePowerMax;
            chargePowerBrake = __instance.ownerPlyer.dataCmp.commonDBStats.SuperChargePowerBrakeRate;
        }

        if (__instance.addCharge)
        {
            __instance.postDecSec = PlActionSpinDashCharge.PostDecSecParam;
            __instance.chargePower += chargePowerAdd * Time.fixedDeltaTime;
            if (__instance.chargePower > chargePowerMax) __instance.chargePower = chargePowerMax;

            __instance.ownerPlyer.PlaySe(PlayerBase.ESe.SpinCharge);
            var eff = PlayerBase.EEffectType.SpineDashTrigger;
            __instance.ownerPlyer.PlayEff(ref eff);
            if (__instance.ownerPlyer.GetType() == typeof(PlayerMain2D))
                ((PlayerMain2D)__instance.ownerPlyer).DashChargeBallAnim();
        }
        else
        {
            if (__instance.postDecSec <= 0.0)
            {
                __instance.chargePower -= __instance.chargePower * chargePowerBrake * Time.fixedDeltaTime;
                __instance.chargePower = Mathf.Max(__instance.chargePower, 0);
            }

            __instance.postDecSec -= Time.fixedDeltaTime;
            __instance.postDecSec = Mathf.Max(__instance.postDecSec, 0);
        }

        __instance.addCharge = false;
        
        return false;
    }
}

[HarmonyPatch(typeof(PlActionJumpUniqueKnuckles), nameof(PlActionJumpUniqueKnuckles.FixedUpdate))]
class GlideUpdatePatch
{
    static bool Prefix(PlActionJumpUniqueKnuckles __instance)
    {
        __instance.CallBase<PlActionBase2D>(nameof(PlActionBase2D.FixedUpdate));
        
        if (__instance.ownerPlyer.nextAction == PlayerBase.EActionIndex.ActNone
            && !__instance.ownerPlyer.IsOnGround)
        {
            var knuckles = __instance.ownerPlyer.TryCast<PlayerKnuckles2D>();
            if (knuckles != null)
            {
                if (!knuckles.lastHitWallAir)
                {
                    float turnSpeed = 2.35f;
                    if (__instance.targetDir != UnitObjBase.EDirectionType.Right)
                    {
                        turnSpeed *= -1;
                    }

                    if (GameScene_Helper.IsPlayerReverseAct)
                        turnSpeed *= -1;
                    
                    turnSpeed *= Time.fixedDeltaTime;

                    float speed = __instance.dirRate + turnSpeed;
                    if (speed < -1)
                        speed = -1;
                    else if (speed > 1)
                        speed = 1;

                    __instance.dirRate = speed;
                }
            }

            float glideSpeed = 0.6f;
            
            if (__instance.targetVelX < 0)
            {
                glideSpeed *= -1;
            }
                    
            glideSpeed *= Time.fixedDeltaTime;

            glideSpeed += __instance.targetVelX;
            if (glideSpeed < -15.2)
                glideSpeed = -15.2f;
            else if (glideSpeed > 15.2)
                glideSpeed = 15.2f;

            __instance.targetVelX = glideSpeed;
        }
        
        return false;
    }
}

[HarmonyPatch(typeof(PlActionKnucklesGlideFall), nameof(PlActionKnucklesGlideFall.Update))]
class GlideFallUpdatePatch
{
    static void Postfix(PlActionKnucklesGlideFall __instance)
    {
        if (__instance.ownerPlyer.IsJumpTriggered())
        {
            var actionId = (PlayerBase.EActionIndex)301;
            __instance.ChangeAction(ref actionId);
        }
    }
}