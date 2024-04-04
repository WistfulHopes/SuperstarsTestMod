using System;
using Il2CppInterop.Runtime.Injection;
using Orion;
using OriPlayer;
using OriPlayerAction;
using OriUnit;
using UnityEngine;

namespace Sonic4;

public class PlActionDrillKnuckles : PlActionBase2D
{
    public PlActionDrillKnuckles(IntPtr ptr) : base(ptr)
    {
    }

    public PlActionDrillKnuckles() : base(ClassInjector.DerivedConstructorPointer<PlActionDrillKnuckles>())
    {
        ClassInjector.DerivedConstructorBody(this);
    }

    public override void Init(ref PlayerBase.EActionIndex lastId)
    {
        ownerPlyer.velocity = new Vector3(0, -8, 0);
        ownerPlyer.PlaySe(PlayerBase.ESe.SpinRollDash);
        ownerPlyer.PlayAnim((PlayerBase.EAnimIndex)301);
    }

    public override void Update()
    {
        this.CallBase<PlActionBase2D>(nameof(Update));
        if (ownerPlyer.IsOnGround)
        {
            ownerPlyer.PlaySe(PlayerBase.ESe.StgBomb);
            var eff = PlayerBase.EEffectType.EmeBulletShoot;
            ownerPlyer.PlayEff(ref eff);
            var bit = DB_PlayerStats.ETypeBit.Super;
            if (ownerPlyer.IsActDb(ref bit))
            {
                DB_CameraShakeParam.Play(DB_CameraShakeParam.ELabel.Player_SuperSpinDash);
            }
            var action = PlayerBase.EActionIndex.ActKnuGlideFallLanding;
            ChangeAction(ref action);
        }
    }

    public override void End(ref PlayerBase.EActionIndex nextId)
    {
        ownerPlyer.ResetDrawLayer();
    }
}