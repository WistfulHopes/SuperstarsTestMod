using System;
using Il2CppInterop.Runtime.Injection;
using OriColli;
using OriPlayer;
using OriPlayerAction;
using OriUnit;
using UnityEngine;

namespace SuperstarsTestMod;

public class PlActionWSpinAttackSonic : PlActionBase2D
{
    private const float HitRadius = 0.25f;
    private const float OriginalHitRadius = 0.175f;

    public PlActionWSpinAttackSonic(IntPtr ptr) : base(ptr) { }
    
    public PlActionWSpinAttackSonic() : base(ClassInjector.DerivedConstructorPointer<PlActionWSpinAttackSonic>())
    {
        ClassInjector.DerivedConstructorBody(this);
    }

    private float _duration;

    public override void Init(ref PlayerBase.EActionIndex lastId)
    {
        ownerPlyer.gameObject.GetComponent<PlayerDataComponent>().localMultiBallColliAtk.GetComponent<SphereCollider>()
            .radius = HitRadius;
        ownerPlyer.bodyDamageObj.GetComponent<ColliEventReciever>().receptAttr = 0;
        
        ownerPlyer.IsRoll = true;
        
        var bodyVisible = false;
        ownerPlyer.SetBodyVisible(ref bodyVisible);
        ownerPlyer.ChangeBallForm(true, true, true);
        var vec3One = UnitObjBase.Vec3One;
        ownerPlyer.SetChargeBallLocalScale(ref vec3One);

        var anim = "Roll";
        ownerPlyer.PlayAnim(ref anim);

        ownerPlyer.PlaySe(PlayerBase.ESe.SpinCharge);
    }

    public override void Update()
    {
        this.CallBase<PlActionBase2D>(nameof(Update));

        if (!ownerPlyer.IsOnGround) return;
        
        var action = PlayerBase.EActionIndex.ActStand;
        ChangeAction(ref action);
    }
    
    public override void FixedUpdate()
    {
        this.CallBase<PlActionBase2D>(nameof(FixedUpdate));

        _duration += Time.fixedDeltaTime;
        
        if (ownerPlyer.IsOnGround) return;
        var delta = Time.fixedDeltaTime;
        ownerPlyer.ApplyAir(ref delta, false, false, false, 
            false, 0, 0, true);

        if (_duration < 0.15f) return;
        var action = PlayerBase.EActionIndex.ActJumpStampBound;
        ChangeAction(ref action);
    }
    
    public override void End(ref PlayerBase.EActionIndex nextId)
    {
        ownerPlyer.gameObject.GetComponent<PlayerDataComponent>().localMultiBallColliAtk.GetComponent<SphereCollider>()
            .radius = OriginalHitRadius;
        ownerPlyer.bodyDamageObj.GetComponent<ColliEventReciever>().receptAttr = (EAtkAttribute)(-1);

        _duration = 0;
        ownerPlyer.ResetDrawLayer();
        ownerPlyer.ChangeBallForm(false);
        if (nextId == PlayerBase.EActionIndex.ActJumpStampBound)
        {
            ownerPlyer.IsRoll = true;
        }
        else
        {
            ownerPlyer.IsRoll = false;   
            var bodyVisible = true;
            ownerPlyer.SetBodyVisible(ref bodyVisible);
        }
    }
}