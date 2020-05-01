using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;
using static SEWorkshop.Models.Policy;
using System.Linq;

namespace SEWorkshop.DataModels
{
    public class DataPolicy
    {
        public IReadOnlyCollection<DataRule> Rules => InnerPolicy.Rules.Select(rule => new DataRule(rule)).ToList().AsReadOnly();
        public IReadOnlyCollection<DataProduct> Products => InnerPolicy.Products.Select(prod => new DataProduct(prod)).ToList().AsReadOnly();
        private Policy InnerPolicy { get; }

        public DataPolicy(Policy policy)
        {
            InnerPolicy = policy;
        }

        public class DataRule
        {
            public int Number => InnerRule.Number;
            public string Description => InnerRule.Description;
            public IReadOnlyCollection<DataRule> References => InnerRule.References.Select(rule => new DataRule(rule)).ToList().AsReadOnly();
            private Rule InnerRule { get; }

            public DataRule(Rule rule)
            {
                InnerRule = rule;
            }
        }
    }
}
