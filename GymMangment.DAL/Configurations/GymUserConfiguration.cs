using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymMangment.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymMangment.DAL.Configurations
{
    public class GymUserConfiguration<T> : IEntityTypeConfiguration<T> where T : GymUsey
    {
        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property(x => x.Name)
                .HasColumnType("nvarchar")
                .HasMaxLength(50);

            builder.Property(x => x.Email)
                .HasColumnType("nvarchar")
                .HasMaxLength(100);

            builder.HasIndex(x => x.Email).IsUnique();
            builder.HasIndex(x => x.Phone).IsUnique();

            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("EmailCheck", "Email LIKE '%@gmail.com' or Email LIKE '%@yahoo.com' or Email LIKE '%@outlook.com' or Email LIKE '%@hotmail.com'");
                tb.HasCheckConstraint("PhoneCheck", "Phone LIKE '010%' or Phone LIKE '011%' or Phone LIKE '012%' or Phone LIKE '015%' ");
            });


            builder.OwnsOne(x => x.Address, address =>
            {
                address.Property(a => a.Street)
                .HasColumnName("Street")
                    .HasColumnType("nvarchar")
                    .HasMaxLength(30);
                address.Property(a => a.City)
                    .HasColumnName("City")
                    .HasColumnType("nvarchar")
                    .HasMaxLength(30);
            });
        }

    }
}
