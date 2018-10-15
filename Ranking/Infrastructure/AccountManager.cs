using Ranking.DAL;
using Ranking.Models;
using Ranking.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ranking.Infrastructure
{
    public class AccountManager
    {
        private RankContext db;
        private ISessionManager session;
        private LoginState loginState;
        private UserType userType;

        public AccountManager(RankContext db, ISessionManager session)
        {
            this.session = session;
            this.db = db;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        public void AddUser(dynamic user)
        {
            user.Stat = Status.Registration;
            if (user is Users)
            {
                Users u = user as Users;
                db.Users.Add(u);
            }
            else if (user is Fans)
                db.Fans.Add(user);
            db.SaveChanges();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="MemberName"></param>
        public void AddCaptainToMembers(string UserName, string MemberName)
        {
            var user = db.Users.Where(u => u.Name == UserName).SingleOrDefault();
            db.Member.Add(new Member() { MName = MemberName, IsCaptain = true, UserId = user.UserId });
            db.SaveChanges();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        public void DeleteUser(Users user)
        {
            var rank = db.Rank.Where(r => r.Uname == user.Name).SingleOrDefault();
            db.Users.Remove(user);
            if (rank != null)
                db.Rank.Remove(rank);
            db.Match.RemoveRange(db.Match.Where(m => m.IsFinished == false && (m.Team1 == user.Name || m.Team2 == user.Name)));

            db.SaveChanges();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fan"></param>
        public void DeleteFan(Fans fan)
        {
            db.Fans.Remove(fan);
            db.SaveChanges();
        }
       /// <summary>
       /// 
       /// </summary>
       /// <param name="member"></param>
        public void AddMember(Member member)
        {
            var name = Helpers.UserName();
            var user = db.Users.Where(u => u.Name == name).SingleOrDefault();

            var newMember = new Member()
            {
                MName = member.MName,
                UserId = user.UserId
            };
            db.Member.Add(newMember);
            db.SaveChanges();

            user.IsTwoPlayers = user.Members.Count >= 2;
            db.SaveChanges();

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="member"></param>
        public void DeleteMember(Member member)
        {
            db.Member.Remove(member);
            db.SaveChanges();

            var user = db.Users.Where(u => u.UserId == member.UserId).SingleOrDefault();
            user.IsTwoPlayers = user.Members.Count() >= 2;
            db.SaveChanges();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Member> GetMembers()
        {
            var name = Helpers.UserName();
            var user = db.Users.Where(u => u.Name == name).SingleOrDefault();

            return user.Members.ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool LimitMembers(string name)
        {
            var user = db.Users.Where(u => u.Name == name).SingleOrDefault();
            if (user.Members.Count <= 7)
                return true;
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool isDuplicateMember(string name)
        {
            var members = db.Member.Where(m => m.MName == name);
            if (members.Count() > 0)
                return true;
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="User"></param>
        public void ChangeUserData(UserChangeViewModel User)
        {
            string name = Helpers.UserName();
            var user = db.Users.Where(u => u.Name == name).SingleOrDefault();
            if(user.Name != User.Name || user.Captain != User.Captain)
            {
                user.stat = Status.Modificarion;
                user.IsAccept = false;
                user.TempName = User.Name;
                user.TempCaptain = User.Captain;
            }

            if (User.Password != null)
                user.Password = User.Password;

            db.SaveChanges();

            SetLoginState(false);
            Authentication(null);
            SetLoginState(true);
            Authentication(GetUser(new LoginViewModel() { Name = user.Name, Password = user.Password }));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public void ChangeUserDataFinish(int id)
        {
            var user = db.Users.Where(u => u.UserId == id).SingleOrDefault();
            var rank = db.Rank.Where(r => r.Uname == user.Name).SingleOrDefault();
            var match = db.Match.Where(m => m.Team1 == user.Name || m.Team2 == user.Name).ToList();

            string name = user.Name;

            if (user.TempName != null)
                user.Name = user.TempName;
            if (rank != null)
                rank.Uname = user.TempName;
            if(user.Captain != null && rank != null)
            {
                rank.Captain = user.TempCaptain;
                user.Captain = user.TempCaptain;
                var member = user.Members.Where(u => u.IsCaptain == true).SingleOrDefault();
                if (member != null)
                    member.MName = user.TempCaptain;
            }
            foreach(var mn in match)
            {
                if (mn.Team1 == name)
                    mn.Team1 = user.Name;
                if (mn.Team2 == name)
                    mn.Team2 = user.Name;
                mn.Colour = "czerwony";
            }

            user.stat = Status.Registration;
            user.IsAccept = true;
            user.TempCaptain = null;
            user.TempName = null;

            db.SaveChanges();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public void SetResetPasswordToken(int id)
        {
            var user = db.Users.Find(id);

            Guid guid = Guid.NewGuid();
            ShortGuid sguid1 = guid;

            user.ResetPasswordToken = sguid1;
            db.SaveChanges();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool Login(LoginViewModel user)
        {
            dynamic userT;
            if(userType == UserType.Fan)
            {
                userT = db.Fans.FirstOrDefault(u => u.Name == user.Name && u.Password == user.Password);
                if (userT == null)
                    return false;
            }
            else if(userType == UserType.Team || userType == UserType.Admin)
            {
                userT = db.Users.FirstOrDefault(u => u.Name == user.Name && u.Password == user.Password);
                if (userT == null)
                    return false;
            }
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="isFan"></param>
        /// <returns></returns>
        public bool IsDuplicateEmail(string email, bool isFan = false)
        {
            if(isFan)
            {
                var fanT = db.Fans.Where(u => u.Email == email);
                if (fanT.Count() > 0)
                    return true;
            }
            else
            {
                var userT = db.Users.Where(u => u.Email == email);
                if (userT.Count() > 0)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="change"></param>
        /// <param name="isFan"></param>
        /// <returns></returns>
        public bool IsDuplicateName(string name, bool change = false, bool isFan = false)
        {
            if (change)
                return Helpers.UserName() != name;
            if(isFan)
            {
                var userF = db.Fans.Where(u => u.Name == name);
                if (userF.Count() > 0)
                    return true;
            }
            else
            {
                var userU = db.Users.Where(u => u.Name == name);
                if (userU.Count() > 0)
                    return true;
            }

            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public bool IsDuplicateIp(string ip)
        {
            var user = db.Fans.Where(f => f.IpAddress == ip);
            if (user.Count() > 0)
                return true;
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool AreNotAcceptedMatches(string name)
        {
            var matches = db.Match.Where(m => m.IsFinished == false).ToList();
            if (matches != null)
                foreach (var m in matches)
                    if (m.Team1 == name || m.Team2 == name)
                        return true;
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public dynamic GetUser(LoginViewModel user)
        {
            if (userType == UserType.Team || userType == UserType.Admin)
                return db.Users.Where(u => u.Name == user.Name && u.Password == user.Password).SingleOrDefault();
            else
                return db.Fans.Where(u => u.Name == user.Name && u.Password == user.Password).SingleOrDefault();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        public void Authentication(dynamic user)
        {
            if(loginState == LoginState.Log_on)
            {
                if (user is Fans)
                    session.Set<Fans>(SessionManager.LoginFanSessionKey, user);
                else
                    session.Set<Users>(SessionManager.LoginSessionKey, user);
            }
            else if(loginState == LoginState.Log_off)
            {
                session.Set<Fans>(SessionManager.LoginFanSessionKey, null);
                session.Set<Users>(SessionManager.LoginSessionKey, null);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isLogin"></param>
        public void SetLoginState(bool isLogin)
        {
            loginState = isLogin ? LoginState.Log_on : LoginState.Log_off;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public void SetUserType(UserType type)
        {
            switch(type)
            {
                case UserType.Team:
                    userType = UserType.Team;
                    break;
                case UserType.Fan:
                    userType = UserType.Fan;
                    break;
                case UserType.Admin:
                    userType = UserType.Admin;
                    break;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public UserType GetUserType()
        {
            return userType;
        }
        enum LoginState
        {
            Log_off =0,
            Log_on =1
        }
    }

    /// <summary>
    /// Represents a globally unique identifier (GUID) with a
    /// shorter string value. Sguid
    /// </summary>
    public struct ShortGuid
    {
        #region Static

        /// <summary>
        /// A read-only instance of the ShortGuid class whose value
        /// is guaranteed to be all zeroes.
        /// </summary>
        public static readonly ShortGuid Empty = new ShortGuid(Guid.Empty);

        #endregion

        #region Fields

        Guid _guid;
        string _value;

        #endregion

        #region Contructors

        /// <summary>
        /// Creates a ShortGuid from a base64 encoded string
        /// </summary>
        /// <param name="value">The encoded guid as a
        /// base64 string</param>
        public ShortGuid(string value)
        {
            _value = value;
            _guid = Decode(value);
        }

        /// <summary>
        /// Creates a ShortGuid from a Guid
        /// </summary>
        /// <param name="guid">The Guid to encode</param>
        public ShortGuid(Guid guid)
        {
            _value = Encode(guid);
            _guid = guid;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets/sets the underlying Guid
        /// </summary>
        public Guid Guid
        {
            get { return _guid; }
            set
            {
                if (value != _guid)
                {
                    _guid = value;
                    _value = Encode(value);
                }
            }
        }

        /// <summary>
        /// Gets/sets the underlying base64 encoded string
        /// </summary>
        public string Value
        {
            get { return _value; }
            set
            {
                if (value != _value)
                {
                    _value = value;
                    _guid = Decode(value);
                }
            }
        }

        #endregion

        #region ToString

        /// <summary>
        /// Returns the base64 encoded guid as a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _value;
        }

        #endregion

        #region Equals

        /// <summary>
        /// Returns a value indicating whether this instance and a
        /// specified Object represent the same type and value.
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is ShortGuid)
                return _guid.Equals(((ShortGuid)obj)._guid);
            if (obj is Guid)
                return _guid.Equals((Guid)obj);
            if (obj is string)
                return _guid.Equals(((ShortGuid)obj)._guid);
            return false;
        }

        #endregion

        #region GetHashCode

        /// <summary>
        /// Returns the HashCode for underlying Guid.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return _guid.GetHashCode();
        }

        #endregion

        #region NewGuid

        /// <summary>
        /// Initialises a new instance of the ShortGuid class
        /// </summary>
        /// <returns></returns>
        public static ShortGuid NewGuid()
        {
            return new ShortGuid(Guid.NewGuid());
        }

        #endregion

        #region Encode

        /// <summary>
        /// Creates a new instance of a Guid using the string value,
        /// then returns the base64 encoded version of the Guid.
        /// </summary>
        /// <param name="value">An actual Guid string (i.e. not a ShortGuid)</param>
        /// <returns></returns>
        public static string Encode(string value)
        {
            Guid guid = new Guid(value);
            return Encode(guid);
        }

        /// <summary>
        /// Encodes the given Guid as a base64 string that is 22
        /// characters long.
        /// </summary>
        /// <param name="guid">The Guid to encode</param>
        /// <returns></returns>
        public static string Encode(Guid guid)
        {
            string encoded = Convert.ToBase64String(guid.ToByteArray());
            encoded = encoded
              .Replace("/", "_")
              .Replace("+", "-");
            return encoded.Substring(0, 22);
        }

        #endregion

        #region Decode

        /// <summary>
        /// Decodes the given base64 string
        /// </summary>
        /// <param name="value">The base64 encoded string of a Guid</param>
        /// <returns>A new Guid</returns>
        public static Guid Decode(string value)
        {
            value = value
              .Replace("_", "/")
              .Replace("-", "+");
            byte[] buffer = Convert.FromBase64String(value + "==");
            return new Guid(buffer);
        }

        #endregion

        #region Operators

        /// <summary>
        /// Determines if both ShortGuids have the same underlying
        /// Guid value.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(ShortGuid x, ShortGuid y)
        {
            if ((object)x == null) return (object)y == null;
            return x._guid == y._guid;
        }

        /// <summary>
        /// Determines if both ShortGuids do not have the
        /// same underlying Guid value.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(ShortGuid x, ShortGuid y)
        {
            return !(x == y);
        }

        /// <summary>
        /// Implicitly converts the ShortGuid to it's string equivilent
        /// </summary>
        /// <param name="shortGuid"></param>
        /// <returns></returns>
        public static implicit operator string(ShortGuid shortGuid)
        {
            return shortGuid._value;
        }

        /// <summary>
        /// Implicitly converts the ShortGuid to it's Guid equivilent
        /// </summary>
        /// <param name="shortGuid"></param>
        /// <returns></returns>
        public static implicit operator Guid(ShortGuid shortGuid)
        {
            return shortGuid._guid;
        }

        /// <summary>
        /// Implicitly converts the string to a ShortGuid
        /// </summary>
        /// <param name="shortGuid"></param>
        /// <returns></returns>
        public static implicit operator ShortGuid(string shortGuid)
        {
            return new ShortGuid(shortGuid);
        }

        /// <summary>
        /// Implicitly converts the Guid to a ShortGuid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static implicit operator ShortGuid(Guid guid)
        {
            return new ShortGuid(guid);
        }

        #endregion
    }
}