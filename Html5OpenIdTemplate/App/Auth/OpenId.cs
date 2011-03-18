﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.RelyingParty;

namespace AppBase.App.Auth
{
    public class OpenId
    {
        private OpenIdRelyingParty _openIdRelyingParty { get; set; }
        private IAuthenticationResponse _response { get; set; }

        public OpenId()
            : this(null)
        {

        }

        public OpenId(OpenIdRelyingParty relyingParty)
        {
            _openIdRelyingParty = relyingParty ?? new OpenIdRelyingParty();
        }

        public ActionResult ProcessOpenId(string openId)
        {
            _response = _openIdRelyingParty.GetResponse();

            if (_response == null)
            {
                Authenticate(openId);
            }
            else
            {
                Verify();
            }

            return new EmptyResult();
        }

        public void Authenticate(string openId)
        {
            Identifier id;
            if (Identifier.TryParse(openId, out id))
            {
                _openIdRelyingParty.CreateRequest(id).RedirectingResponse.Send();
            }
            else
            {
                throw new ApplicationException("Invalid Identifier");
            }
        }

        public string Verify()
        {
            string retVal = null;

            switch (_response.Status)
            {
                case AuthenticationStatus.Authenticated:
                    retVal = _response.ClaimedIdentifier;
                    break;
                case AuthenticationStatus.Canceled:
                    throw new ApplicationException("Canceled at Provider");
                case AuthenticationStatus.Failed:
                    throw new ApplicationException(_response.Exception.Message);
            }

            return retVal;
        }
    }
}