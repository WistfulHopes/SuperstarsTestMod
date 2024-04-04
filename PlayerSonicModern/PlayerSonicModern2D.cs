using System;
using System.Runtime.InteropServices;
using Il2CppInterop.Runtime.Injection;
using Il2CppSystem;
using OriPlayer;

namespace Sonic4.PlayerSonicModern;

public class PlayerSonicModern2D : PlayerMain2D
{
    protected bool Allocated;

    public override void Awake()
    {
        this.CallBase<PlayerMain2D>(nameof(Awake));
        Allocate();
    }

    public virtual bool Allocate()
    {
        if (Allocated) return false;

        
        
        Allocated = true;
        return true;
    }

    public override void InitAction()
    {
        InitBaseAction();
        CheckStartEventActionState();
        EActionIndex action = EActionIndex.ActSpawn;
        SetCurrentAction(ref action);
    }

    public override bool IsTranslucentBallState()
    {
        return crntAction != EActionIndex.ActSpinDashCharge 
               && crntAction != EActionIndex.ActGmkAttachBall
               && crntAction != EActionIndex.ActJumpUnique;
    }

    public override bool IgnoreBallRotY()
    {
        if (IsOnGround) return false;
        if (dataCmp == null) return false;
        
        var ballChargeObj = dataCmp.crntBallChargeMeshObj;
        if (ballChargeObj == null || !ballChargeObj.activeInHierarchy) return false;

        var animName = dataCmp.crntBallAnimName;
        if (animName == AnimBallIdleMeshKey)
            return true;
        if (animName.Length == AnimBallIdleMeshKey.Length)
        {
            var animChar = animName[(System.Index)0];
            var keyChar = AnimBallIdleMeshKey[(System.Index)0];
            return SpanHelpers.SequenceEqual(ref animChar, ref keyChar, 2 * animName.Length);
        }

        return false;
    }
}