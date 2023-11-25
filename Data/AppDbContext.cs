using Grpc1.Models;
using Microsoft.EntityFrameworkCore;

namespace Grpc1.Data;

public class AppDbContext : DbContext
{
   public AppDbContext(DbContextOptions<AppDbContext> options) : base (options)
   {
    
   }

   public DbSet<Grpc1Item> Grpc1Items => Set<Grpc1Item>();
}