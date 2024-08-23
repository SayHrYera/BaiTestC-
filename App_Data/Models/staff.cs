using System;
using System.Collections.Generic;

namespace App_Data.Models
{
    public partial class staff
    {
        public staff()
        {
            DepartmentFacilities = new HashSet<DepartmentFacility>();
            StaffMajorFacilities = new HashSet<StaffMajorFacility>();
        }

        public byte? Status { get; set; }
        public long? CreatedDate { get; set; }
        public long? LastModifiedDate { get; set; }
        public Guid Id { get; set; }
        public string? AccountFe { get; set; }
        public string? AccountFpt { get; set; }
        public string? Name { get; set; }
        public string? StaffCode { get; set; }

        public virtual ICollection<DepartmentFacility> DepartmentFacilities { get; set; }
        public virtual ICollection<StaffMajorFacility> StaffMajorFacilities { get; set; }
    }
}
