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
    using System.ComponentModel.DataAnnotations;

    public partial class room : BaseModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public room()
        {
            this.booking_details = new HashSet<booking_details>();
            this.bookings = new HashSet<booking>();
        }
    
        public int ID { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Room name must not be empty.")]
        [MaxLength(20, ErrorMessage = "Maximum of 20 characters is allowed.")]
        public string RoomName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Status must not be empty.")]
        [Range(0, 3)]
        public int Status { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Price must not be empty.")]
        [Range(0, 999999999)]
        public double Price { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Type must not be empty.")]
        [Range(0, 1)]
        public int Type { get; set; }
        [Required( ErrorMessage = "Max people must not be empty.")]
        [Range(0, 10)]
        [Display(Name = "Max People")]
        public int MaxPeople { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<booking_details> booking_details { get; set; }
        public virtual room_status room_status { get; set; }
        public virtual room_type room_type { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<booking> bookings { get; set; }
    }
}
