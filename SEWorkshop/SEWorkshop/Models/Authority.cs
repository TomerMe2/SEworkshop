using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SEWorkshop.Enums;

namespace SEWorkshop.Models
{
    public class Authority
    {
        public virtual int AuthHandlerId { get; set; }
        public virtual AuthorityHandler AuthHandler { get; set; }
        public virtual Authorizations Authorization { get; set; }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public Authority()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {

        }

        public Authority(AuthorityHandler authHandler, Authorizations authorization)
        {
            AuthHandler = authHandler;
            Authorization = authorization;
            AuthHandlerId = authHandler.Id;
        }
    }
}
