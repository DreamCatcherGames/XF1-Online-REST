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
    
    public partial class Verification_Request
    {
        public string Token { get; set; }
        public string Username { get; set; }
    
        public virtual Player Player { get; set; }
    }
}