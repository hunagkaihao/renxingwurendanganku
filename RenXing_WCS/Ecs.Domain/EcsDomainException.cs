using System;

namespace Ecs;

public class EcsDomainException : Exception
{
    public EcsDomainException(string msg)
        : base(msg)
    {   
        
    }
}