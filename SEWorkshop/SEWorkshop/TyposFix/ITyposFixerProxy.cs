using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.TyposFix
{
    interface ITyposFixerProxy
    {
        public void AddToDictionary(string productName);
        public void RemoveFromDictionary(string productName);
        public string Correct(string input);
    }
}
