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
    
    public partial class Championship
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Championship()
        {
            this.Races = new HashSet<Race>();
        }
    
        public string Unique_Key { get; set; }
        public string Name { get; set; }
        public bool CurrentChamp { get; set; }
        public string Rules_Description { get; set; }
        public System.DateTime Beginning_Date { get; set; }
        public System.TimeSpan Beginning_Time { get; set; }
        public System.DateTime Ending_Date { get; set; }
        public System.TimeSpan Ending_Time { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Race> Races { get; set; }
    }
}
