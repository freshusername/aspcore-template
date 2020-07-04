using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace api.models
{
    public class Role : IdentityRole<string>, IEntityTypeConfiguration<Role>
    {
        //added to properly call constructor of base class
        public Role() : base() { }

        public Role(string roleName) : base(roleName)
        {
            base.Name = roleName;
        }
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");
        }
    }
}