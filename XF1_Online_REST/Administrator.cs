//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace XF1_Online_REST
{
    using System;
    using System.Collections.Generic;
    
    public partial class Administrator
    {
        public Administrator() { }

        public Administrator(string Username,string Password,string Token,string Salt) 
        {
            this.Username = Username;
            this.Password = Password;
            this.Token = Token;
            this.Salt = Salt;
        }
        public string Username { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Token { get; set; }
    }
}
