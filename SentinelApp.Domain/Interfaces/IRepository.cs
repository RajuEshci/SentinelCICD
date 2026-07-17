using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelApp.Domain.Interfaces
{
	public interface IRepository<T, TFilter> where T : class where TFilter : class
	{
		Task<T?> GetByIdAsync(int id);
		Task<IEnumerable<T>> GetAllAsync(TFilter filter);
		Task<int> AddAsync(T entity);
		Task<bool> UpdateAsync(T entity);
		Task<bool> DeleteAsync(int id, int userId);
		//Task<string?> CheckDuplicateAsync(string tableName, Dictionary<string, object?> fieldValues);
	}

}
