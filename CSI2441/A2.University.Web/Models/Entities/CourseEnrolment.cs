//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace A2.University.Web.Models.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class CourseEnrolment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CourseEnrolment()
        {
            this.UnitEnrolments = new HashSet<UnitEnrolment>();
        }
    
        public long course_enrolment_id { get; set; }
        public long student_id { get; set; }
        public string course_id { get; set; }
        public string course_status { get; set; }
    
        public virtual Course Course { get; set; }
        public virtual Student Student { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UnitEnrolment> UnitEnrolments { get; set; }
    }
}
