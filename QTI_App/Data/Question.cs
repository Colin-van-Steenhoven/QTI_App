﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTI_App.Data
{
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public List<Answer> Answers { get; set; } = new List<Answer>();
        public List<QuestionTag> QuestionTags { get; set; } = new List<QuestionTag>();
    }
}
