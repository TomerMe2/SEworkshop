using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;
using static SEWorkshop.Models.Policy;
using System.Linq;

namespace SEWorkshop.DataModels
{
    public class DataPolicy : DataModel<Policy>
    {
        public IReadOnlyCollection<DataRule> Rules => InnerModel.Rules.Select(rule => new DataRule(rule)).ToList().AsReadOnly();
        public IReadOnlyCollection<DataProduct> Products => InnerModel.Products.Select(prod => new DataProduct(prod)).ToList().AsReadOnly();

        public DataPolicy(Policy policy) : base(policy) { }

        public class DataRule : DataModel<Rule>
        {
            public int Number => InnerModel.Number;
            public string Description => InnerModel.Description;
            public IReadOnlyCollection<DataRule> References => InnerModel.References.Select(rule => new DataRule(rule)).ToList().AsReadOnly();

            public DataRule(Rule rule) : base(rule) { }
        }
    }
}
