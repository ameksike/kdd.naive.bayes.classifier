
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Classifier.Models.ORM;

namespace Classifier.Models.DAO
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) { }
        public DbSet<TopicDoc> TopicDoc { get; set; }
        public DbSet<TopicWord> TopicWord { get; set; }
    }
}
