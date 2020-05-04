using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.DataModels
{
    public class DataModel<T> where T : class
    {
        protected T InnerModel { get; }

        public DataModel(T innerModel)
        {
            InnerModel = innerModel;
        }

        public bool Represents(T model)
        {
            return model == InnerModel;
        }

        public override bool Equals(object? obj)
        {
            var other = obj as DataModel<T>;
            if (other is null)
            {
                return false;
            }
            return other.Represents(InnerModel);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
