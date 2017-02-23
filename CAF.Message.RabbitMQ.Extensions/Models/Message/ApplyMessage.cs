﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Message.Distributed.Extensions.Models
{
   public class ApplyMessage
    {
        public int applyid { get; set; }
        public int userid { get; set; }
        public string applyname { get; set; }
        public string applyavatar { get; set; }
        public int applyim { get; set; }
        public string msg { get; set; }
        public int targetid { get; set; }
        public string addtime { get; set; }
        public int result { get; set; }
        public int applytype { get; set; }
        public bool isself { get; set; }

    }
}
