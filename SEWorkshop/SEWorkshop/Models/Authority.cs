using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SEWorkshop.Enums;

namespace SEWorkshop.Models
{
    [Table("Authorities")]
    public class Authority
    {
        [ForeignKey("AuthorityHandlers"), Key, Column(Order = 0)]
        public AuthorityHandler AuthHandler { get; private set; }
        public Authorizations Authorization { get; private set; }

        public Authority(AuthorityHandler authHandler, Authorizations authorization)
        {
            AuthHandler = authHandler;
            Authorization = authorization;
        }
    }
}
