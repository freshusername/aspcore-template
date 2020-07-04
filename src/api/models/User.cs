using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace api.models
{
    public class User : IdentityUser<string>, IEntityTypeConfiguration<User>
    {
        public string Name { get; set; }

        public ICollection<Record> Records { get; set; }

        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
        }
    }
}