using NLog;
using SEWorkshop.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEWorkshop.Models
{
    public class Manages
    {
        public LoggedInUser LoggedInUser { get; set; }
        public Store Store { get; set; }
        public  ICollection<Authorizations> AuthoriztionsOfUser { get; private set; }
        private static readonly Logger log = LogManager.GetCurrentClassLogger();


        public Manages(LoggedInUser loggedInUser, Store store)
        {
            LoggedInUser = loggedInUser;
            Store = store;
            AuthoriztionsOfUser = new List<Authorizations>();
        }

     
    }
    public enum Authorizations
    {
        Products,
        Owner,
        Manager,
        Authorizing,
        Replying,
        Watching
    }

}
