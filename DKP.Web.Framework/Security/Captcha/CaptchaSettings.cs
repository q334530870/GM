using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DKP.Core.Configuration;

namespace DKP.Web.Framework.Security.Captcha
{
    public class CaptchaSettings : ISettings
    {
        public bool Enabled { get; set; }
        public bool ShowOnLoginPage { get; set; }
        public bool ShowOnRegistrationPage { get; set; }
        public bool ShowOnProductCommentPage { get; set; }
        public bool ShowOnBorrowingPage { get; set; }
    }
}
