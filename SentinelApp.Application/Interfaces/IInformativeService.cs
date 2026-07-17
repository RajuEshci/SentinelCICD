using SentinelApp.Application.Core;
using SentinelApp.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelApp.Application.Interfaces
{
	public interface IInformativeService
	{
		Task<ServiceResponse<PaginatedResponseDto<InformativePagesDto>>> GetAllPagesAsync(InformativePagesFilterDto filterDto);
		Task<ServiceResponse<int>> CreateInformativePagesAsync(CreateInformativePagesDto createInformativePagesDto);
		Task<ServiceResponse<InformativePagesDto?>> GetPagesByIdAsync(int id);
		Task<ServiceResponse<bool>> UpdatePagesAsync(int id, UpdateInformativePagesDto updateInformativePagesDto);
		Task<ServiceResponse<bool>> DeletePageAsync(int id);
		Task<ServiceResponse<InformativePagesResponseDto>> GetByPageCodeIdAsync(InformativePagesRequestDto informativePagesRequestDto);
		Task<ServiceResponse<List<InformativePagesDto>>> GetAllPagesListAsync();
		Task<ServiceResponse<List<InformativePagesDto>>> GetMenuListAsync();
	}
}
