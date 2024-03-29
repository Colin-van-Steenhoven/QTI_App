﻿using QTI_App.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace QTI_App.Data.Seeders
{
    public class AnswerConfiguration : IEntityTypeConfiguration<Answer>
    {
        public void Configure(EntityTypeBuilder<Answer> builder)
        {
            builder.HasData(
                new Answer { Id = 1, Text = "Answer 1", QuestionId = 1, Option = 'a', IsCorrect = true },
                new Answer { Id = 2, Text = "Answer 2", QuestionId = 2, Option = 'a', IsCorrect = false }
            );

            // Configure the one-to-many relationship
            builder
                .HasOne(a => a.Question)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.QuestionId);
        }
    }
}
