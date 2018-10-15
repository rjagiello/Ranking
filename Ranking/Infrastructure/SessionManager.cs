using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace Ranking.Infrastructure
{
    public class SessionManager : ISessionManager
    {
        public const string LoginSessionKey = "LoginSessionKey";
        public const string LoginFanSessionKey = "LoginFanSessionKey";
        private HttpSessionState session;

        public SessionManager()
        {
            session = HttpContext.Current.Session;
            session.Timeout = 60;
        }
        public void Abandon()
        {
            session.Abandon();
        }

        public T Get<T>(string key)
        {
            return (T)session[key];
        }

        public void Set<T>(string name, T value)
        {
            session[name] = value;
        }

        public T TryGet<T>(string key)
        {
            try
            {
                return (T)session[key];
            }
            catch(NullReferenceException)
            {
                return default(T);
            }
        }
    }
}