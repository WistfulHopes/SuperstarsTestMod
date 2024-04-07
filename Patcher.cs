using HarmonyLib;
using Il2CppSystem;
using OriBattleRoyal;
using Orion;
using OriPlayer;
using OriPlayerAction;
using OriUnit;
using UnityEngine;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local

namespace SuperstarsTestMod;

public class Patcher
{
    public static void DoPatching()
    {
        var harmony = new Harmony("com.orion.testmod");
        harmony.PatchAll();
    }
}

[HarmonyPatch(typeof(PlayerSonic2D), nameof(PlayerSonic2D.InitAction))]
class SonicInitActionPatch
{
    static void Postfix(PlayerSonic2D __instance)
    {
        var bounceAction = new PlActionBounceSonic();
        var bounceActionId = (PlayerBase.EActionIndex)300;
        __instance.SetUpAction(ref bounceActionId, bounceAction);
        var wSpinAction = new PlActionWSpinAttackSonic();
        var wSpinActionId = (PlayerBase.EActionIndex)301;
        __instance.SetUpAction(ref wSpinActionId, wSpinAction);
    }
}

[HarmonyPatch(typeof(PlayerRabbit2D), nameof(PlayerRabbit2D.InitAction))]
class RabbitInitActionPatch
{
    static void Postfix(PlayerRabbit2D __instance)
    {
        var bounceAction = new PlActionBounceSonic();
        var bounceActionId = (PlayerBase.EActionIndex)300;
        __instance.SetUpAction(ref bounceActionId, bounceAction);
        var wSpinAction = new PlActionWSpinAttackSonic();
        var wSpinActionId = (PlayerBase.EActionIndex)301;
        __instance.SetUpAction(ref wSpinActionId, wSpinAction);
    }
}

[HarmonyPatch(typeof(PlActionJumpUniqueTails), nameof(PlActionJumpUniqueTails.Update))]
class FlyUpdate
{
    static bool Prefix(PlActionJumpUniqueTails __instance)
    {
        var isJumpTriggered = __instance.ownerPlyer.IsJumpTriggered();
        var inpPlyCtrl = __instance.ownerPlyer.inputPlyCtlr;
        var nowPad = inpPlyCtrl.playerPad.nowPad;
        if (!SysSaveDataKeyboardConfig.IsOnAction(SysSaveDataKeyboardConfig.EAction.MoveDown, nowPad) ||
            !isJumpTriggered) return true;
        var actionId = PlayerBase.EActionIndex.ActJumpStampBound;
        __instance.ChangeAction(ref actionId);
        return false;
    }
}

[HarmonyPatch(typeof(PlActionKnucklesGlideLanding), nameof(PlActionKnucklesGlideLanding.Update))]
class GlideLandUpdate
{
    static bool Prefix(PlActionKnucklesGlideLanding __instance)
    {
        var isJumpTriggered = __instance.ownerPlyer.IsJumpTriggered();
        var inpPlyCtrl = __instance.ownerPlyer.inputPlyCtlr;
        var nowPad = inpPlyCtrl.playerPad.nowPad;
        if (!SysSaveDataKeyboardConfig.IsOnAction(SysSaveDataKeyboardConfig.EAction.MoveDown, nowPad) ||
            !isJumpTriggered) return true;
        var actionId = PlayerBase.EActionIndex.ActSpinDashCharge;
        __instance.ChangeAction(ref actionId);
        return false;
    }
}

[HarmonyPatch(typeof(PlActionKnucklesGlideFallLanding), nameof(PlActionKnucklesGlideFallLanding.Update))]
class GlideFallLandUpdate
{
    static bool Prefix(PlActionKnucklesGlideFallLanding __instance)
    {
        var isJumpTriggered = __instance.ownerPlyer.IsJumpTriggered();
        var inpPlyCtrl = __instance.ownerPlyer.inputPlyCtlr;
        var nowPad = inpPlyCtrl.playerPad.nowPad;
        if (!SysSaveDataKeyboardConfig.IsOnAction(SysSaveDataKeyboardConfig.EAction.MoveDown, nowPad) ||
            !isJumpTriggered) return true;
        var actionId = PlayerBase.EActionIndex.ActSpinDashCharge;
        __instance.ChangeAction(ref actionId);
        return false;
    }
}

[HarmonyPatch(typeof(PlActionBase2D), nameof(PlActionBase2D.UpdateUniqueTails))]
class UpdateUniqueTails
{
    static bool Prefix(PlActionBase2D __instance)
    {
        return __instance.ownerPlyer.lastAction != PlayerBase.EActionIndex.ActJumpUnique;
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

        if (!__instance.IsUniqueActBase || !__instance.ownerPlyer.IsRoll ||
            !__instance.firstUpdateUniqActCheck &&
            __instance.ownerPlyer.lastAction != (PlayerBase.EActionIndex)301) return false;
        
        var isJumpPress = __instance.ownerPlyer.IsJumpPress();

        var enableDropDash = __instance.ownerPlyer.TryCast<PlayerRabbit2D>() ? Plugin.RabbitEnableDropDash : Plugin.SonicEnableDropDash;
        var extraActionType = __instance.ownerPlyer.TryCast<PlayerRabbit2D>() ? Plugin.RabbitExtraActionType : Plugin.SonicExtraActionType;
        
        if (isJumpPress && enableDropDash)
        {
            if (__instance.animIputRate > 0)
            {
                __instance.animIputRate += __instance.ownerPlyer.GetDeltaTime;
                
                if
                    (__instance.animIputRate < 0.3666 &&
                     __instance.ownerPlyer.lastAction != (PlayerBase.EActionIndex)301 ||
                     __instance.animIputRate < 0.1666) return false;
                
                var actionId = PlayerBase.EActionIndex.ActJumpUnique;
                __instance.ChangeAction(ref actionId);
            }
            else
            {
                __instance.animIputRate = (float)(__instance.ownerPlyer.GetDeltaTime + 0.016667);
                switch (extraActionType)
                {
                    case Plugin.ESonicExtraActionType.WSpinAttack:
                    {
                        var bit = DB_PlayerStats.ETypeBit.Super;
                        if (!__instance.ownerPlyer.IsActDb(ref bit) &&
                            __instance.ownerPlyer.lastAction != (PlayerBase.EActionIndex)301
                            && __instance.ownerPlyer.lastAction != PlayerBase.EActionIndex.ActJumpUnique)
                        {
                            var actionId = (PlayerBase.EActionIndex)301;
                            __instance.ChangeAction(ref actionId);
                        }
                        break;
                    }
                    case Plugin.ESonicExtraActionType.BounceAttack:
                    case Plugin.ESonicExtraActionType.None:
                    default:
                        break;
                }
            }
        }
        else if (__instance.animIputRate > 0)
        {
            switch (extraActionType)
            {
                case Plugin.ESonicExtraActionType.BounceAttack:
                {
                    var actionId = (PlayerBase.EActionIndex)300;
                    __instance.ChangeAction(ref actionId);
                    break;
                }
                case Plugin.ESonicExtraActionType.WSpinAttack:
                {
                    var bit = DB_PlayerStats.ETypeBit.Super;
                    if (!__instance.ownerPlyer.IsActDb(ref bit) &&
                        __instance.ownerPlyer.lastAction != (PlayerBase.EActionIndex)301
                        && __instance.ownerPlyer.lastAction != PlayerBase.EActionIndex.ActJumpUnique)
                    {
                        var actionId = (PlayerBase.EActionIndex)301;
                        __instance.ChangeAction(ref actionId);
                    }
                    break;
                }
                case Plugin.ESonicExtraActionType.None:
                default:
                    __instance.animIputRate = 0;
                    return false;
            }
        }
        else
        {
            __instance.animIputRate = 0;
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

        if (!__instance.ownerPlyer.IsOnGround) return false;
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

        if (__instance.ownerPlyer.nextAction != PlayerBase.EActionIndex.ActNone
            || __instance.ownerPlyer.IsOnGround) return false;
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

                var speed = __instance.dirRate + turnSpeed;
                speed = speed switch
                {
                    < -1 => -1,
                    > 1 => 1,
                    _ => speed
                };

                __instance.dirRate = speed;
            }
        }

        var glideSpeed = 0.6f;
            
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

        return false;
    }
}