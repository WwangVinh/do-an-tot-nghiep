using CoreEntities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.DTOs
{
    public class ArticleCar
    {
        public int ArticleId { get; set; }
        public virtual Article Article { get; set; } = null!;

        public int CarId { get; set; }
        public virtual Car Car { get; set; } = null!;
    }
}
