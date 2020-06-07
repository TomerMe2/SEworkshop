using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SEWorkshop.Enums;

namespace SEWorkshop.Models
{
    public class Authority
    {
        public virtual string AuthHandlerUsername { get; set; }
        public virtual string AuthHandlerStoreName { get; set; }
        public virtual AuthorityHandler AuthHandler { get; set; }
        public virtual Authorizations Authorization { get; set; }

        private Authority()
        {
            AuthHandler = null!;
            Authorization = default;
            AuthHandlerUsername = "";
            AuthHandlerStoreName = "";
        }

        public Authority(AuthorityHandler authHandler, Authorizations authorization)
        {
            AuthHandler = authHandler;
            Authorization = authorization;
            AuthHandlerUsername = authHandler.Username;
            AuthHandlerStoreName = authHandler.StoreName;
        }
    }
}
