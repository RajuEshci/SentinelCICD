using Microsoft.Extensions.Configuration;
using SentinelApp.Application.Core;
using SentinelApp.Application.DTO;
using SentinelApp.Application.Interfaces;
using SentinelApp.Domain.Entities;
using SentinelApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelApp.Application.Services
{
	public class InformativePagesService : IInformativeService
	{
		private readonly IInformativePages _informativePages;
		private readonly IConfiguration _config;
		private readonly ICurrentUserService _currentUserService;
		private readonly int _userId;
		private readonly ICheckDuplicateRepository _checkDuplicateRepository;
		private readonly int _questionnarieId;
		private readonly string _gender;
		public InformativePagesService(IConfiguration config, ICurrentUserService currentUserService, IInformativePages informativePages, ICheckDuplicateRepository checkDuplicateRepository)
		{
			_config = config;
			_currentUserService = currentUserService;
			_informativePages = informativePages;
			_userId = int.TryParse(_currentUserService.UserId, out var id) ? id : 0;
			_checkDuplicateRepository = checkDuplicateRepository;
			_questionnarieId = _currentUserService.QuestionnaireId;
			_gender = _currentUserService.Gender;
		}
		public async Task<ServiceResponse<PaginatedResponseDto<InformativePagesDto>>> GetAllPagesAsync(InformativePagesFilterDto filterDto)
		{
			return await ServiceResponseExceptionHandler.Handle<PaginatedResponseDto<InformativePagesDto>>(async () =>
			{
				var filter = new InformativePagesFilter
				{
					SearchText = filterDto.SearchText,
					Status = filterDto.Status,
					SortColumn = filterDto.SortColumn,
					SortDirection = filterDto.SortDirection,
					PageNo = filterDto.Page,
					PerPage = filterDto.PerPage,
				};
				var cardCount = await _informativePages.GetAllCardCounts(filter);

				var pagesData = await _informativePages.GetAllAsync(filter);

				if (pagesData.Count() == 0)
					throw new ServiceResponseException(System.Net.HttpStatusCode.Conflict, "No data found.");

				var totalPages = (int)Math.Ceiling(cardCount.Total / (double)filterDto.PerPage);

				var currencyDtos = pagesData.Select(MapToPagesDto).ToList();

				return new PaginatedResponseDto<InformativePagesDto>
				{
					Page = filterDto.Page,
					PerPage = filterDto.PerPage,
					Total = cardCount.Total,
					TotalPages = totalPages,
					Data = currencyDtos,
					CardCount = new CardCountDto
					{
						Total = cardCount.Total,
						ActiveCount = cardCount.ActiveCount,
						InactiveCount = cardCount.InactiveCount
					}
				};
			}, "Data retrieved successfully.");
		}
		public async Task<ServiceResponse<int>> CreateInformativePagesAsync(CreateInformativePagesDto createInformativePagesDto)
		{
			return await ServiceResponseExceptionHandler.Handle<int>(async () =>
			{
				var duplicateMultiMessage = await _checkDuplicateRepository.CheckDuplicateAsync("InformativePages", new Dictionary<string, object?>
				{
					{ "Name", createInformativePagesDto.Name },
					{ "PageCode", createInformativePagesDto.PageCode }
				}, "multiple");

				if (duplicateMultiMessage != null)
				{
					throw new ServiceResponseException(System.Net.HttpStatusCode.Conflict, duplicateMultiMessage);
				}

				var pages = new InformativePagesMaster
				{
					Name = createInformativePagesDto.Name,
					Description = createInformativePagesDto.Description,
					Image = createInformativePagesDto.Image,
					QuestionnaireId = createInformativePagesDto.QuestionnaireId,
					AndriodUrl = createInformativePagesDto.AndriodUrl,
					IosUrl = createInformativePagesDto.IosUrl,
					SortOrder = createInformativePagesDto.SortOrder,
					ParentPageId = createInformativePagesDto.ParentPageId,
					IsApplication = createInformativePagesDto.IsApplication,
					PageCode = createInformativePagesDto.PageCode,
					LanguageId = createInformativePagesDto.LanguageId,
					IsMenu = createInformativePagesDto.IsMenu,
					CreatedBy = _userId,
					IsActive = createInformativePagesDto.IsActive
				};
				return await _informativePages.AddAsync(pages);
			}, "Data added successfully.");
		}
		public async Task<ServiceResponse<InformativePagesDto?>> GetPagesByIdAsync(int id)
		{
			return await ServiceResponseExceptionHandler.Handle<InformativePagesDto?>(async () =>
			{
				var currency = await _informativePages.GetByIdAsync(id);
				return currency != null ? MapToPagesDto(currency) : null;
			}, "Data retrieved successfully.");
		}
		public async Task<ServiceResponse<bool>> UpdatePagesAsync(int id, UpdateInformativePagesDto updateInformativePagesDto)
		{
			return await ServiceResponseExceptionHandler.Handle<bool>(async () =>
			{
				var duplicateMultiMessage = await _checkDuplicateRepository.CheckDuplicateAsync("InformativePages", new Dictionary<string, object?>
				{
					{ "Name", updateInformativePagesDto.Name },
					{ "PageCode", updateInformativePagesDto.PageCode },
				}, mode: "multi", excludeIdColumn: "Id", excludeIdValue: id);

				if (duplicateMultiMessage != null)
				{
					throw new ServiceResponseException(System.Net.HttpStatusCode.Conflict, duplicateMultiMessage);
				}

				var pageData = await _informativePages.GetByIdAsync(id);
				if (pageData == null)
				{
					throw new KeyNotFoundException($"Informative page with ID {id} not found");
				}

				if (!string.IsNullOrEmpty(updateInformativePagesDto.Name))
					pageData.Name = updateInformativePagesDto.Name;

				if (!string.IsNullOrEmpty(updateInformativePagesDto.Description))
					pageData.Description = updateInformativePagesDto.Description;

				if (!string.IsNullOrEmpty(updateInformativePagesDto.Image))
					pageData.Image = updateInformativePagesDto.Image;

				if (updateInformativePagesDto.QuestionnaireId > 0)
					pageData.QuestionnaireId = updateInformativePagesDto.QuestionnaireId;

				if (!string.IsNullOrEmpty(updateInformativePagesDto.AndriodUrl))
					pageData.AndriodUrl = updateInformativePagesDto.AndriodUrl;

				if (!string.IsNullOrEmpty(updateInformativePagesDto.IosUrl))
					pageData.IosUrl = updateInformativePagesDto.IosUrl;

				if (updateInformativePagesDto.SortOrder > 0)
					pageData.SortOrder = updateInformativePagesDto.SortOrder;

				if (updateInformativePagesDto.ParentPageId > 0)
					pageData.ParentPageId = updateInformativePagesDto.ParentPageId;

				if (!string.IsNullOrEmpty(updateInformativePagesDto.PageCode))
					pageData.PageCode = updateInformativePagesDto.PageCode;

				if (updateInformativePagesDto.LanguageId > 0)
					pageData.LanguageId = updateInformativePagesDto.LanguageId;

				if (updateInformativePagesDto.IsMenu > 0)
					pageData.IsMenu = updateInformativePagesDto.IsMenu;

				pageData.IsApplication = updateInformativePagesDto.IsApplication;
				pageData.IsActive = updateInformativePagesDto.IsActive;
				pageData.UpdatedBy = _userId;

				return await _informativePages.UpdateAsync(pageData);
			}, "Data updated successfully.");
		}
		public async Task<ServiceResponse<bool>> DeletePageAsync(int id)
		{
			return await ServiceResponseExceptionHandler.Handle<bool>(async () =>
			{
				return await _informativePages.DeleteAsync(id, _userId);
			}, "Data deleted successfully.");
		}
		public async Task<ServiceResponse<InformativePagesResponseDto>> GetByPageCodeIdAsync(InformativePagesRequestDto informativePagesRequestDto)
		{
			return await ServiceResponseExceptionHandler.Handle<InformativePagesResponseDto>(async () =>
			{
                var allowedCodes = new[] { "TNC", "PPL" };
                if (!allowedCodes.Contains(informativePagesRequestDto.PageCode, StringComparer.OrdinalIgnoreCase))
                {
                    var userId = _currentUserService.UserId;
                    if (string.IsNullOrEmpty(userId))
                        throw new ServiceResponseException(System.Net.HttpStatusCode.Unauthorized, "Authentication required" );
                }
			    var data = await _informativePages.GetByPageCodeIdAsync(informativePagesRequestDto.PageCode, _questionnarieId != 0 ? _questionnarieId : 1);
				if (data == null)
				{
					throw new ServiceResponseException(System.Net.HttpStatusCode.OK, "Data Not Found");
				}
				return data != null ? MapToDto(data) : null;
			}, "Data retrieved successfully.");
		}
		public async Task<ServiceResponse<List<InformativePagesDto>>> GetAllPagesListAsync()
		{
			return await ServiceResponseExceptionHandler.Handle<List<InformativePagesDto>>(async () =>
			{
				var pagesData = await _informativePages.GetInformativePagesListAsync(_questionnarieId);

				if (pagesData?.Count() == 0)
					throw new ServiceResponseException(System.Net.HttpStatusCode.Conflict, "Data not found.");

				var infoPageDtos = pagesData.Select(MapToPagesDto).ToList();
				return infoPageDtos;
			}, "Data retrieved successfully.");
		}
		public async Task<ServiceResponse<List<InformativePagesDto>>> GetMenuListAsync()
		{
			return await ServiceResponseExceptionHandler.Handle<List<InformativePagesDto>>(async () =>
			{
				var pagesData = await _informativePages.GetMenuListAsync(_questionnarieId, _gender);

				if (pagesData?.Count() == 0)
					throw new ServiceResponseException(System.Net.HttpStatusCode.Conflict, "Data not found.");

				var infoPageDtos = pagesData.Select(MapToMenuPagesDto).ToList();
				return infoPageDtos;
			}, "Data retrieved successfully.");
		}

		private InformativePagesResponseDto MapToDto(InformativePagesMaster informativePagesMaster)
		{
			return new InformativePagesResponseDto
			{
				Id = informativePagesMaster.Id,
				Name = informativePagesMaster.Name,
				Description = informativePagesMaster.Description,
				Image = informativePagesMaster.Image,
				QuestionnaireId = informativePagesMaster.QuestionnaireId,
				SortOrder = informativePagesMaster.SortOrder,
				ParentPageId = informativePagesMaster.ParentPageId,
				PageCode = informativePagesMaster.PageCode,
				LanguageId = informativePagesMaster.LanguageId,
				IsMenu = informativePagesMaster.IsMenu,
			};
		}
		private InformativePagesDto MapToPagesDto(InformativePagesMaster informativePagesMaster)
		{
			return new InformativePagesDto
			{
				Id = informativePagesMaster.Id,
				Name = informativePagesMaster.Name,
				Description = informativePagesMaster.Description,
				Image = informativePagesMaster.Image,
				QuestionnaireId = informativePagesMaster.QuestionnaireId,
				SortOrder = informativePagesMaster.SortOrder,
				ParentPageId = informativePagesMaster.ParentPageId,
				PageCode = informativePagesMaster.PageCode,
				LanguageId = informativePagesMaster.LanguageId,
				IsMenu = informativePagesMaster.IsMenu,
				IsApplication = informativePagesMaster.IsApplication
			};
		}
		private InformativePagesDto MapToMenuPagesDto(InformativePagesMaster informativePagesMaster)
		{
			return new InformativePagesDto
			{
				Id = informativePagesMaster.Id,
				Name = informativePagesMaster.Name,
				Image = informativePagesMaster.Image,
				QuestionnaireId = informativePagesMaster.QuestionnaireId,
				SortOrder = informativePagesMaster.SortOrder,
				ParentPageId = informativePagesMaster.ParentPageId,
				PageCode = informativePagesMaster.PageCode,
				LanguageId = informativePagesMaster.LanguageId,
				IsMenu = informativePagesMaster.IsMenu,
				IsApplication = informativePagesMaster.IsApplication
			};
		}
	}
}
