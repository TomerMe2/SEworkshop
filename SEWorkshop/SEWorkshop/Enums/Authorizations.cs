using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Enums
{
    public enum Authorizations : byte
    {
        Products,
        Owner,
        Manager,
        Authorizing,
        Replying,
        Watching
    }
}