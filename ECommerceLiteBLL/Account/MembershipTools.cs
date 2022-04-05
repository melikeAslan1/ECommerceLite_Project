using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceLiteDAL;
using ECommerceLiteEntity.IdentityModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web;


namespace ECommerceLiteBLL.Account
{
    public static class MembershipTools
    {
        public static UserStore<ApplicationUser> NewUserStore()
        {
            return new UserStore<ApplicationUser>(new MyContext());

        }

        public static UserManager<ApplicationUser> NewUserManager()
        {
            return new UserManager<ApplicationUser>(NewUserStore());
        }

        public static RoleStore<ApplicationRole> NewRoleStore()
        {
            return new RoleStore<ApplicationRole>(new MyContext());
        }

        public static RoleManager<ApplicationRole> NewRoleManager()
        {
            return new RoleManager<ApplicationRole>(NewRoleStore());
        }

        public static string GetUserName(string id)
        {
            var usermanager = NewUserManager();
            var user = usermanager.FindById(id);
            if(user!=null)
            {
                return user.UserName;
            }
            return null;
        }

        public static string GetNameSurname()
        {
            var id = HttpContext.Current.User.Identity.GetUserId();
            if(string.IsNullOrEmpty(id))
            {
                return "Ziyaretçi";
            }

            var usermanager = NewUserManager();
            var user = usermanager.FindById(id);
            // string usernamesurname="";
            // string usernamesurname=null;

            string usernamesurname = string.Empty;
            usernamesurname = user != null ?
                //string.Format("{0} {1}", user.Name, user.Surname)
                //user.Name + " " + user.Surname
                $"{user.Name} {user.Surname}"
                :
                "Ziyaretçi";

            return usernamesurname;
        }

        public static ApplicationUser GetUser()
        {
            var id = HttpContext.Current.User.Identity.GetUserId();

            if(string.IsNullOrEmpty(id))
            {
                return null;
            }

            var myusermanager = NewUserManager();
            var user = myusermanager.FindById(id);

            return user;
        }


    }
}
