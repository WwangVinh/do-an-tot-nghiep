using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEntities.Models
{
    public enum CarCondition
    {
        New = 0,        // Mới 100%
        Used = 1,       // Cũ / Đã qua sử dụng
        LikeNew = 2    // Xe lướt / 99%
    }
}
