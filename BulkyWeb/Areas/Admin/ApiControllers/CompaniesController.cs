using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.ViewModels.Admin;
using Bulky.Utility;
using BulkyWeb.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.ApiControllers
{
	[Route("api/admin/[controller]")]
	[ApiController]
	// [Authorize(Roles = SD.Role_Admin)]
	public class CompaniesController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;

		public CompaniesController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
            try
            {
                var lstCompaniesModel = await _unitOfWork.Company.GetAllAsync();

                var lstCompaniesViewModel = new List<CompanyViewModel>();

                foreach (var companyModel in lstCompaniesModel)
                {
                    var companyViewModel = new CompanyViewModel();
                    Mapper.Map(companyModel, companyViewModel);
                    lstCompaniesViewModel.Add(companyViewModel);
                }

                return Ok(new { success = true, data = lstCompaniesViewModel });
            }
			catch (Exception ex)
			{
				// Log the exception details (optional)
				return StatusCode(500, new { success = false, message = "An error occurred while retrieving companies." });
			}
		}

		[HttpDelete("{id:int}")]
		public async Task<IActionResult> Delete(int id)
		{
			if (id <= 0)
				return BadRequest(new { success = false, message = $"({id}) is an invalid Id" });

			try
			{
				var company = await _unitOfWork.Company.GetByIdAsync(id);

				if (company == null)
					return NotFound(new { success = false, message = $"There's no company with Id = ({id})" });

				_unitOfWork.Company.Remove(company);
				await _unitOfWork.SaveAsync();
				return Ok(new { success = true, message = "Company deleted successfully!" });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { success = false, message = "An error occurred while processing your request." });
			}
		}
	}
}
