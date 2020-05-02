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
    }
}
