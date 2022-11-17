using Bvs.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bvs_API.Data
{
    public class Seed
    {
        public static async Task SeedStudents(DataContext context)
        {
            if (await context.Student.AnyAsync()) return;

            var studentData = await System.IO.File.ReadAllTextAsync("Data/StudentSeedData.json");
            var students = JsonSerializer.Deserialize<List<Student>>(studentData);

            foreach (var student in students)
            {
                context.Student.Add(student);
            }

            await context.SaveChangesAsync();
        }
    }
}
