using System;
using Il2CppInterop.Runtime.Injection;
using OriPlayer;

namespace Sonic4.PlayerSonicModern;

public class PlayerUniqueComponentSonicModern : PlayerUniqueComponentBase
{
    public PlayerUniqueComponentSonicModern(IntPtr ptr) : base(ptr) { }

    public PlayerUniqueComponentSonicModern() : base(ClassInjector.DerivedConstructorPointer<PlayerUniqueComponentSonicModern>())
    {
        ClassInjector.DerivedConstructorBody(this);
    }
}