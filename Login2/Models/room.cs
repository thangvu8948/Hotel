//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Login2.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class room
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public room()
        {
            this.booking_details = new HashSet<booking_details>();
        }
    
        public int ID { get; set; }
        public string RoomName { get; set; }
        public int Status { get; set; }
        public double Price { get; set; }
        public int Type { get; set; }
        public int MaxPeople { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<booking_details> booking_details { get; set; }
        public virtual room_status room_status { get; set; }
        public virtual room_type room_type { get; set; }
    }
}
