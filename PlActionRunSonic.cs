using System;
using Il2CppInterop.Runtime.Injection;
using Orion;
using OriPlayerAction;
using OriUnit;

namespace Sonic4;

public class PlActionRunSonic : PlActionRun
{
    public PlActionRunSonic(IntPtr ptr) : base(ptr) { }

    public PlActionRunSonic() : base(ClassInjector.DerivedConstructorPointer<PlActionRunSonic>())
    {
        ClassInjector.DerivedConstructorBody(this);
    }

    public override void Update()
    {
        this.CallBase<PlActionRun>(nameof(Update));
        var inpPlyCtrl = ownerPlyer.inputPlyCtlr;
        var nowPad = inpPlyCtrl.playerPad.nowPad;
        if (!SysSaveDataKeyboardConfig.IsOnAction(SysSaveDataKeyboardConfig.EAction.SubButton2, nowPad)) return;
        if (ownerPlyer.direType == UnitObjBase.EDirectionType.Right)
        {
            if (ownerPlyer.velocity.x <= 2.8 && SysSaveDataKeyboardConfig.IsOnAction(SysSaveDataKeyboardConfig.EAction.MoveRight, nowPad))
            {
                ownerPlyer.SetGroundVelocity((float)2.8);
            }
            else if (ownerPlyer.velocity.x >= -2.8 && SysSaveDataKeyboardConfig.IsOnAction(SysSaveDataKeyboardConfig.EAction.MoveLeft, nowPad))
            {
                ownerPlyer.SetGroundVelocity((float)-2.8);
            }
        }
        else
        {
            if (ownerPlyer.velocity.x >= -2.8 && SysSaveDataKeyboardConfig.IsOnAction(SysSaveDataKeyboardConfig.EAction.MoveRight, nowPad))
            {
                ownerPlyer.SetGroundVelocity((float)-2.8);
            }
            else if (ownerPlyer.velocity.x <= 2.8 && SysSaveDataKeyboardConfig.IsOnAction(SysSaveDataKeyboardConfig.EAction.MoveLeft, nowPad))
            {
                ownerPlyer.SetGroundVelocity((float)2.8);
            }
        }
    }
}