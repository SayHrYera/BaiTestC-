using System;
using System.Collections.Generic;

namespace App_Data.Models
{
    public partial class StaffMajorFacility
    {
        public byte? Status { get; set; }
        public long? CreatedDate { get; set; }
        public long? LastModifiedDate { get; set; }
        public Guid Id { get; set; }
        public Guid? IdMajorFacility { get; set; }
        public Guid? IdStaff { get; set; }

        public virtual MajorFacility? IdMajorFacilityNavigation { get; set; }
        public virtual staff? IdStaffNavigation { get; set; }
    }
}
