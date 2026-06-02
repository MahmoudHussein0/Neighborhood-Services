using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Staffs.DTOs
{
    public class StaffPermissionDto
    {
        public int Id { get; set; }
        public int StaffId { get; set; }
        public string Permission { get; set; }
    }
}
