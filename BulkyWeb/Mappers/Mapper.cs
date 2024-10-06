using Bulky.Models;
using Bulky.Models.Common;
using Bulky.Models.Identity;
using Bulky.Models.Orders;
using Bulky.Models.ViewModels;
using Bulky.Models.ViewModels.Admin;
using Bulky.Models.ViewModels.Admin.Books;
using Bulky.Models.ViewModels.Customer;

namespace BulkyWeb.Mappers
{
    public static class Mapper
	{
		public static void Map(TbCategory model, CategoryViewModel viewModel)
		{
			viewModel.Id = model.Id;
			viewModel.Name = model.Name;
			viewModel.DisplayOrder = model.DisplayOrder;
		}

		public static void Map(TbBook bookModel, AddEditBookViewModel bookViewModel)
		{
			bookViewModel.Id = bookModel.Id;
			bookViewModel.Title = bookModel.Title;
			bookViewModel.Description = bookModel.Description;
			bookViewModel.ISBN = bookModel.ISBN;
			bookViewModel.Author = bookModel.Author;
			bookViewModel.ListPrice = bookModel.ListPrice;
			bookViewModel.Price = bookModel.Price;
			bookViewModel.Price50 = bookModel.Price50;
			bookViewModel.Price100 = bookModel.Price100;
			bookViewModel.ImageUrl = bookModel.ImageUrl;
			bookViewModel.CategoryId = bookModel.CategoryId;
		}

		public static void Map(AddEditBookViewModel bookViewModel, TbBook bookModel)
		{
			bookModel.Title = bookViewModel.Title;
			bookModel.Description = bookViewModel.Description;
			bookModel.ISBN = bookViewModel.ISBN;
			bookModel.Author = bookViewModel.Author;
			bookModel.ListPrice = bookViewModel.ListPrice;
			bookModel.Price = bookViewModel.Price;
			bookModel.Price50 = bookViewModel.Price50;
			bookModel.Price100 = bookViewModel.Price100;
			bookModel.CategoryId = bookViewModel.CategoryId;
			// bookModel.ImageUrl = "";
		}

		public static void Map(TbBook bookModel, BookDetailsForAdminViewModel bookDetailsViewModel)
		{
			bookDetailsViewModel.Id = bookModel.Id;
			bookDetailsViewModel.Title = bookModel.Title;
			bookDetailsViewModel.Description = bookModel.Description;
			bookDetailsViewModel.ISBN = bookModel.ISBN;
			bookDetailsViewModel.Author = bookModel.Author;
			bookDetailsViewModel.ListPrice = bookModel.ListPrice;
			bookDetailsViewModel.Price = bookModel.Price;
			bookDetailsViewModel.Price50 = bookModel.Price50;
			bookDetailsViewModel.Price100 = bookModel.Price100;
			bookDetailsViewModel.ImageUrl = bookModel.ImageUrl;
			bookDetailsViewModel.Category = bookModel.Category.Name;
		}

		public static void Map(TbCompany companyModel, CompanyViewModel companyViewModel)
		{
			companyViewModel.Id = companyModel.Id;
			companyViewModel.Name = companyModel.Name;
			companyViewModel.PhoneNumber = companyModel.PhoneNumber;
			companyViewModel.StreetAddress = companyModel.AddressInfo.StreetAddress;
			companyViewModel.City = companyModel.AddressInfo.City;
			companyViewModel.State = companyModel.AddressInfo.State;
			companyViewModel.PostalCode = companyModel.AddressInfo.PostalCode;
		}

        public static void Map(CompanyViewModel companyViewModel, TbCompany companyModel)
        {
            companyModel.Name = companyViewModel.Name;
            companyModel.PhoneNumber = companyViewModel.PhoneNumber;
			companyModel.AddressInfo = new AddressInfo
			{
                StreetAddress = companyViewModel.StreetAddress,
				City = companyViewModel.City,
				State = companyViewModel.State,
				PostalCode = companyViewModel.PostalCode
			};
        }

		public static void Map(TbBook bookModel, BookDetailsViewModel shoppingCartViewModel)
		{
			shoppingCartViewModel.BookDetails = new();
			Map(bookModel, shoppingCartViewModel.BookDetails);
		}

		public static void Map(ApplicationUser user, TbOrder order)
		{
			order.Name = user.Name;
			order.PhoneNumber = user.PhoneNumber;
			order.City = user.AddressInfo.City;
			order.StreetAddress = user.AddressInfo.StreetAddress;
			order.State = user.AddressInfo.State;
			order.State = user.AddressInfo.State;
			order.PostalCode = user.AddressInfo.PostalCode;
		}
	}
}
