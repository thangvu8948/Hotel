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

    public partial class staff:BaseModel
    {
        public int ID { get; set; }
        public int Account_id { get; set; }
        [Required(ErrorMessage = "Room name must not be empty.")]
        [MaxLength(20, ErrorMessage = "Maximum of 20 characters is allowed.")]
        public string Name { get; set; }
        [DataType(DataType.DateTime, ErrorMessage = "Invalid Datetime")]
        [Display(Name = "Date Of Birth")]
        public Nullable<System.DateTime> DOB { get; set; }
        [Required(ErrorMessage = "IdentityCard must not be empty.")]
        [MaxLength(10, ErrorMessage = "Maximum of 10 characters is allowed.")]
        [RegularExpression("^[0-9]{15}$",
        ErrorMessage = "Invalid IdentityCard")]
        public string IdentityCard { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Phone must not be empty.")]
        [MaxLength(10, ErrorMessage = "Maximum of 10 characters is allowed.")]
        [RegularExpression("0([0-9]{9})",
 ErrorMessage = "Invalid Phone")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Required")]
        [StringLength(50, ErrorMessage = "Max 50 characters")]
        [RegularExpression(@"^[a-zA-Z]+(([\'\,\.\- ][a-zA-Z ])?[a-zA-Z]*)*\s+<(\w[-._\w]*\w@\w[-._\w]*\w\.\w{2,3})>$|^(\w[-._\w]*\w@\w[-._\w]*\w\.\w{2,3})$", ErrorMessage = "Invalid EmailAddress")]
        public string Email { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "IdentityCard must not be empty.")]
        [MaxLength(50, ErrorMessage = "Maximum of 50 characters is allowed.")]
        public string Address { get; set; }
    
        public virtual account account { get; set; }
    }
}
