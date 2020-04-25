using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Adapters
{
    public interface ISecurityAdapter
    {
        public byte[] Encrypt(string toEncrypt);
    }
}
