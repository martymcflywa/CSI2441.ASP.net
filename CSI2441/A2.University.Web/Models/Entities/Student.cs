//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using GridMvc.DataAnnotations;

namespace A2.University.Web.Models.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class Student
    {
        [Display(Name = "Student ID")]
        [GridColumn(Title = "Student ID", SortEnabled = true, FilterEnabled = true)]
        public long student_id { get; set; }

        [Display(Name = "First name")]
        [Required(ErrorMessage = "The First name field is required.")]
        [RegularExpression("(^[a-zA-Z]+$)", ErrorMessage = "Must be a name.")]
        [GridColumn(Title = "First name", SortEnabled = true, FilterEnabled = true)]
        public string firstname { get; set; }

        [Display(Name = "Surname")]
        [Required(ErrorMessage = "The Surname field is required.")]
        [RegularExpression("(^[a-zA-Z]+$)", ErrorMessage = "Must be a name.")]
        [GridColumn(Title = "Surname", SortEnabled = true, FilterEnabled = true)]
        public string lastname { get; set; }

        [Display(Name = "DOB")]
        [DataType(DataType.Date, ErrorMessage = "Must be a date.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [GridColumn(Title = "DOB", SortEnabled = true, FilterEnabled = true, Format = "{0:dd/MM/yyyy}")]
        public System.DateTime dob { get; set; }

        [Display(Name = "Email")]
        [GridColumn(Title = "Email", SortEnabled = true, FilterEnabled = true)]
        public string email { get; set; }

        [Display(Name = "Landline")]
        [Required(ErrorMessage = "The Landline field is required.")]
        [RegularExpression("(\\+?\\(?[0-9 ]{2}?\\)?[0-9 ]{6,})", ErrorMessage = "Must be a phone number.")]
        [NotMappedColumn]
        public string ph_landline { get; set; }

        [Display(Name = "Mobile")]
        [Required(ErrorMessage = "The Mobile field is required.")]
        [RegularExpression("(\\+?\\(?[0-9 ]{2}?\\)?[0-9 ]{8,})", ErrorMessage = "Must be a phone number.")]
        [NotMappedColumn]
        public string ph_mobile { get; set; }

        [Display(Name = "Address")]
        [Required(ErrorMessage = "The Address field is required.")]
        [NotMappedColumn]
        public string adrs { get; set; }

        [Display(Name = "City")]
        [Required(ErrorMessage = "The City field is required.")]
        [RegularExpression("^([a-zA-Z]+\\s)*[a-zA-Z]+$", ErrorMessage = "Must be a name.")]
        [NotMappedColumn]
        public string adrs_city { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "The State field is required.")]
        [NotMappedColumn]
        public string adrs_state { get; set; }

        [Display(Name = "Postcode")]
        [Required(ErrorMessage = "The Postcode field is required.")]
        [RegularExpression("(^[0-9]{4}$)", ErrorMessage = "Must be a valid postcode.")]
        [NotMappedColumn]
        public int adrs_postcode { get; set; }
    }
}
