using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Models
{
    public class Policy
    {
        public ICollection<Rule> Rules { get; private set; }
        public ICollection<Product> Products { get; private set; }
        
        public Policy()
        {
            Products = new List<Product>();
            Rules = new List<Rule>();
        }

        public override string ToString()
        {
            string output = "Rules:\n";
            foreach (var Rule in Rules)
            {
                output += Rule.ToString() + "\n";
            }
            return output;
        }

        public class Rule
        {
            public int Number { get; private set; }
            public string Description { get; private set;}
            public ICollection<Rule> References { get; private set;}

            public Rule(int number, string description)
            {
                Number = number;
                Description = description;
                References = new List<Rule>();
            }

            public override string ToString()
            {
                string referencesDescription = "";
                foreach (var Rule in References)
                {
                    referencesDescription += Rule.Number + "\n";
                }
                return "Number: " + Number + "\nDescription: " + Description + "\nReferences: " + referencesDescription;
            }
        }
    }
}
