using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTI_App.Data
{
    public class Answer
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public char Option { get; set; }
        public bool IsCorrect { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
    }
}
