﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTI_App.Data
{
    public class QuestionTag
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
