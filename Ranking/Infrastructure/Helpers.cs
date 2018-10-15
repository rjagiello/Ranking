using Ranking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ranking.Infrastructure
{
    public static class Helpers
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string UserName()
        {
            ISessionManager session = new SessionManager();
            dynamic user = new Users();
            if(session.Get<Users>(SessionManager.LoginFanSessionKey) != null)
            {
                user = session.Get<Users>(SessionManager.LoginFanSessionKey) as Users;
                return user.Name;
            }
            else if(session.Get<Fans>(SessionManager.LoginFanSessionKey) != null)
            {
                user = session.Get<Fans>(SessionManager.LoginFanSessionKey) as Fans;
                return user.Name;
            }
            return " ";
        }
    }
}