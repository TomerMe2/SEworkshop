using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Enums
{
    public enum KindOfUser : byte
    {
        Guest,
        LoggedInNotOwnNotManage,
        LoggedInNoOwnYesManage,
        LoggedInYesOwn,
        Admin
    }
}
