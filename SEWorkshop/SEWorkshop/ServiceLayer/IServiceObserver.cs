using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.ServiceLayer
{
    public interface IServiceObserver<T>
    {
        public void Notify(T arg);
    }
}
