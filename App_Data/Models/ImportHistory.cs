using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Data.Models
{
    public class ImportHistory
    {
        [Key]
        public int Id { get; set; }
        public DateTime ImportDate { get; set; }
        public string FileName { get; set; }
        public int TotalRecords { get; set; }
        public int SuccessfulRecords { get; set; }
        public int FailedRecords { get; set; }
        public string ErrorDetails { get; set; }
    }
}
