using System;

namespace Wcs;

public class WcsDomainException : Exception
{
    public WcsDomainException(string msg)
        : base(msg)
    {

    }
}