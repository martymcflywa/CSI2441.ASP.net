﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using A2.University.Web.Models.Entities;
using FluentValidation.Attributes;

namespace A2.University.Web.Models
{
    [Validator(typeof(StudentBaseViewModelValidator))]
    public class StudentBaseViewModel
    {
        [Key]
        [Display(Name = "Student ID")]
        public long StudentId { get; set; }

        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Display(Name = "Surname")]
        public string LastName { get; set; }

        [Display(Name = "DOB")]
        [DisplayFormat(DataFormatString = "{0:DD/MM/YYYY}", ApplyFormatInEditMode = true)]
        public DateTime Dob { get; set; }

        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Landline")]
        public string LandLine { get; set; }

        [Display(Name = "Mobile")]
        public string Mobile { get; set; }

        [Display(Name = "Address")]
        public string Adrs { get; set; }

        [Display(Name = "City")]
        public string AdrsCity { get; set; }

        [Display(Name = "State")]
        public string AdrsState { get; set; }

        // made string instead of int, else displays 0 as prefilled default for some reason
        // cast to int when updating db
        [Display(Name = "Postcode")]
        public string AdrsPostcode { get; set; }
    }

    public class StudentIndexViewModel : StudentBaseViewModel
    {
        public List<StudentIndexViewModel> Students = new List<StudentIndexViewModel>();
    }

    public class StudentDropDownListViewModel : StudentBaseViewModel
    {
        // Gender dropdownlist
        // view sets selected value to model, see base class
        public IEnumerable<SelectListItem> GenderDropDownList = new List<SelectListItem>
        {
            new SelectListItem { Value = "M", Text = "Male" },
            new SelectListItem { Value = "F", Text = "Female" }
        };

        // state dropdownlist
        // view sets selected value to model, see base class
        public IEnumerable<SelectListItem> StateDropDownList = new List<SelectListItem>
        {
            new SelectListItem {Text = "ACT", Value = "ACT"},
            new SelectListItem {Text = "NSW", Value = "NSW"},
            new SelectListItem {Text = "NT", Value = "NT"},
            new SelectListItem {Text = "QLD", Value = "QLD"},
            new SelectListItem {Text = "SA", Value = "SA"},
            new SelectListItem {Text = "TAS", Value = "TAS"},
            new SelectListItem {Text = "VIC", Value = "VIC"},
            new SelectListItem {Text = "WA", Value = "WA"}
        };
    }

    public class StudentDetailsViewModel : StudentBaseViewModel
    {
        // No custom fields required
    }

    public class StudentCreateViewModel : StudentDropDownListViewModel
    {
        // default date shown at create view, today - 18 years
        [DisplayFormat(DataFormatString = "{0:DD/MM/YYYY}", ApplyFormatInEditMode = true)]
        public DateTime DefaultDob = DateTime.Today.AddYears(-18);
    }

    public class StudentEditViewModel : StudentDropDownListViewModel
    {
        // Inherits StudentDropDownListViewModel, no custom fields required
    }

    public class StudentDeleteViewModel : StudentBaseViewModel
    {
        // No custom fields required
    }
}