﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasModel
{
    /// <summary>
    /// 收藏
    /// </summary>
  public  class Physics_CollectionModel
    {
        public int Id { get; set; }
        public string ProductGuid { get; set; }
        public string UserId { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
