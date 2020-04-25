using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.TyposFix
{
    interface ITyposFixerProxy
    {
        public void AddToDictionary(string input);
        public void RemoveFromDictionary(string input);
        public string Correct(string input);
    }
}
