
using System;
using System.Linq;
 
using CAF.Infrastructure.Core.Utilities.Randomizer;
using CAF.WebSite.Application.Services.Authentication.External;
using System.Collections.Generic;
using CAF.Infrastructure.Core;
namespace CAF.ConsoleApplication
{
   
    public class AuthorizerTest  
    {
        public   IExternalAuthorizer _authorizer;


        public void GetRandowString()
        {
            _authorizer = EngineContext.Current.Resolve<IExternalAuthorizer>();



            // Console.Write(negativeInteger);
            OAuthUser oAuthUser = new OAuthUser() { Nickname = "疯狂蚂蚁" };

            //将返回结果序列化为对象
            var parameters = new OAuthAuthenticationParameters("CAF.WebSite.QQAuth")
            {
                ExternalIdentifier = "003A6D4B6480874BE8B9379C559B850A",
                OAuthToken = "7754A05BF35E68DA7DD2F223C8A7E0AF",
                OAuthAccessToken = "003A6D4B6480874BE8B9379C559B850A",
            };

            ParseClaims(oAuthUser, parameters);
         
            var result = _authorizer.Authorize(parameters);
        }
        private void ParseClaims(OAuthUser user, OAuthAuthenticationParameters parameters)
        {
            var claims = new UserClaims();
            claims.Contact = new ContactClaims();

            string name = user.Nickname;

            claims.Name = new NameClaims();

            if (!name.IsEmpty())
            {
                var nameSplit = name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (nameSplit.Length >= 2)
                {
                    claims.Name.First = nameSplit[0];
                    claims.Name.Last = nameSplit[1];
                }
                else
                {
                    claims.Name.Last = nameSplit[0];
                }

            }
            parameters.AddClaim(claims);
        }

    }
    public class OAuthUser
    {
        private int _ret = -1;//状态码
        private string _msg = "";//错误信息
        private string _nickname = "";//用户昵称

        /// <summary>
        /// 状态码
        /// </summary>
        public int Ret
        {
            get { return _ret; }
            set { _ret = value; }
        }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Msg
        {
            get { return _msg; }
            set { _msg = value; }
        }
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string Nickname
        {
            get { return _nickname; }
            set { _nickname = value; }
        }
    }
    [Serializable]
    public class OAuthAuthenticationParameters : OpenAuthenticationParameters
    {
        private readonly string _providerSystemName;
        private IList<UserClaims> _claims;

        public OAuthAuthenticationParameters(string providerSystemName)
        {
            this._providerSystemName = providerSystemName;
        }

        public override IList<UserClaims> UserClaims
        {
            get
            {
                return _claims;
            }
        }

        public void AddClaim(UserClaims claim)
        {
            if (_claims == null)
                _claims = new List<UserClaims>();

            _claims.Add(claim);
        }

        public override string ProviderSystemName
        {
            get { return _providerSystemName; }
        }
    }
}