using System;
using System.Runtime.InteropServices;
using Il2CppInterop.Runtime.Injection;
using Orion;
using OriPlayer;
using OriPlayerAction;
using OriUnit;
using Sonic4;
using static DB_PlayerStats;
// ReSharper disable InconsistentNaming

namespace SuperstarsTestMod;

public class PlActionBounceSonic : PlActionJumpUniqueSonic
{
    private enum EState
    {
        Fall,
        Bounce,
    }

    private EState state;
    private float bounceVelocity;
    private float heightMultiplier = (float)1.0;
    
    public PlActionBounceSonic(IntPtr ptr) : base(ptr)
    {
        state = EState.Fall;
    }

    public PlActionBounceSonic() : base(ClassInjector.DerivedConstructorPointer<PlActionBounceSonic>())
    {
        ClassInjector.DerivedConstructorBody(this);
        state = EState.Fall;
    }

    private void Fall()
    {
        state = EState.Fall;
        var ownerPlyerVelocity = ownerPlyer.velocity;
        ownerPlyerVelocity.y = -6;
        ownerPlyer.velocity = ownerPlyerVelocity;
        var bodyVisible = false;
        ownerPlyer.SetBodyVisible(ref bodyVisible);
        ownerPlyer.ChangeBallForm(true, true, true);
        var vec3One = UnitObjBase.Vec3One;
        ownerPlyer.SetChargeBallLocalScale(ref vec3One);
        ownerPlyer.PlaySe(PlayerBase.ESe.Roll);
    }
    
    public override void Init([In] ref PlayerBase.EActionIndex lastId)
    {
        //this.CallActionBase<PlActionBase2D>(nameof(Init), ref lastId);
        Fall();
    }
    
    public override void End([In] ref PlayerBase.EActionIndex nextId)
    {
        heightMultiplier = (float)1.0;
        ownerPlyer.ResetDrawLayer();
        ownerPlyer.ChangeBallForm(false);
        if (nextId == PlayerBase.EActionIndex.ActFall)
        {
            ownerPlyer.IsRoll = true;
        }
        else
        {
            ownerPlyer.IsRoll = false;   
            bool bodyVisible = true;
            ownerPlyer.SetBodyVisible(ref bodyVisible);
        }

        //this.CallActionBase<PlActionBase2D>(nameof(End), ref nextId);
    }
    
    public override void Update()
    {
        this.CallBase<PlActionBase2D>(nameof(Update));
        if (ownerPlyer.IsOnGround)
        {
            var ownerPlyerVelocity = ownerPlyer.velocity;
            ownerPlyerVelocity.y = bounceVelocity * heightMultiplier;
            ownerPlyer.velocity = ownerPlyerVelocity;
            ownerPlyer.ResetOnGround();
            ownerPlyer.PlaySe(PlayerBase.ESe.SpinRollDash);
            var bit = ETypeBit.Super;
            if (ownerPlyer.IsActDb(ref bit))
            {
                DB_CameraShakeParam.Play(DB_CameraShakeParam.ELabel.Player_SuperSpinDash);
            }
            state = EState.Bounce;
        }
        else if (state == EState.Bounce)
        {
            if (ownerPlyer.velocity.y <= -2)
            {
                var actionId = PlayerBase.EActionIndex.ActJumpStampBound;
                ChangeAction(ref actionId);
                return;
            }
            if (ownerPlyer.velocity.y > bounceVelocity * 0.8 * heightMultiplier) return;
            
            var inpPlyCtrl = ownerPlyer.inputPlyCtlr;
            var nowPad = inpPlyCtrl.playerPad.nowPad;
            if (!SysSaveDataKeyboardConfig.IsTriggerAction(SysSaveDataKeyboardConfig.EAction.Action, nowPad)) return;
            heightMultiplier += (float)0.1;
            if (heightMultiplier >= 1.25)
                heightMultiplier = (float)1.25;
            Fall();
        }
        else
        {
            bounceVelocity = -ownerPlyer.velocity.y * (float)0.75;
            if (bounceVelocity <= 3.9)
                bounceVelocity = (float)3.9;
        }
    }

    public override void FixedUpdate()
    {
        this.CallBase<PlActionJumpUniqueSonic>(nameof(FixedUpdate));
        var ownerPlyerVelocity = ownerPlyer.velocity;
        ownerPlyerVelocity.y -= ownerPlyer.GravityForce;
        ownerPlyer.velocity = ownerPlyerVelocity;
    }

    public override void PostUpdate()
    {
    }
}