using Bulky.Models.ViewModels.Admin;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bulky.DataAccess.Data.Configurations
{
    internal class UserListViewModelConfiguration : IEntityTypeConfiguration<UserListViewModel>
    {
        public void Configure(EntityTypeBuilder<UserListViewModel> builder)
        {
            builder.HasNoKey()
                .ToView("UserList_View");
        }
    }
}
