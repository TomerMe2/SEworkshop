using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SEWorkshop.Adapters
{
    public class SecurityAdapter : ISecurityAdapter
    {
        /// <summary>
        /// Encrypts using SHA256
        /// </summary>
        public byte[] Encrypt(string toEncrypt)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(toEncrypt);
            using var shaMan = new SHA256Managed();
            return shaMan.ComputeHash(bytes);
        }
    }
}
