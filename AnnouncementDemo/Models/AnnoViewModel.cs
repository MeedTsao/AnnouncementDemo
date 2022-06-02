using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AnnouncementDemo.Models
{
    public class AnnoViewModel
    {
		/// <summary>
		/// 查詢request
		/// </summary>
		public class QueryIn
		{
			/// <summary>
			/// 公告項目
			/// </summary>
			public string AnnoSubject { get; set; }
			/// <summary>
			/// 公告狀態
			/// </summary>
			public string AnnoStatus { get; set; }
			/// <summary>
			/// 分頁
			/// </summary>
			public PaginationModel pagination { get; set; }
		}

		/// <summary>
		/// 查詢response
		/// </summary>
		public class QueryOut 
		{
			/// <summary>
			/// 查詢結果
			/// </summary>
			public List<AnnoModel> Grid { get; set; }
			/// <summary>
			/// 分頁
			/// </summary>
			public PaginationModel pagination { get; set; }
		}

		/// <summary>
		/// 查詢資料庫對應的欄位
		/// </summary>
		public class AnnoModel
		{
			public string Pkey { get; set; }
			public string AnnoDate { get; set; }
			public string AnnoSubject { get; set; }
			public string AnnoContent { get; set; }
			public string AnnoStatus { get; set; }
			public string AnnoStatusName { get; set; }
		}

		/// <summary>
		/// 儲存request 欄位
		/// </summary>
		public class AddSaveIn
		{
			[Required]
			public string AnnoDate { get; set; }
			[Required]
			public string AnnoSubject { get; set; }
			[Required]
			public string AnnoContent { get; set; }
			[Required]
			public string AnnoStatus { get; set; }
		}

		/// <summary>
		/// 儲存response 欄位
		/// </summary>
		public class AddSaveOut : BaseResponseModel { }

		/// <summary>
		/// 編輯request 欄位
		/// </summary>
		public class EditSaveIn
		{
			[Required]
			public string Pkey { get; set; }
			[Required]
			public string AnnoDate { get; set; }
			[Required]
			public string AnnoSubject { get; set; }
			[Required]
			public string AnnoContent { get; set; }
			[Required]
			public string AnnoStatus { get; set; }
		}
		/// <summary>
		/// 編輯response 欄位
		/// </summary>
		public class EditSaveOut : BaseResponseModel { }

		/// <summary>
		/// 刪除request 欄位
		/// </summary>
		public class DelCheckIn
		{
			public List<AnnoModel> checks { get; set; }
		}
		/// <summary>
		/// 刪除response 欄位
		/// </summary>
		public class DelCheckOut : BaseResponseModel { }

		/// <summary>
		/// 分頁 Model
		/// </summary>
		public class PaginationModel
		{
			public List<string> pages { get; set; }
			public int pageNo { get; set; }
			public int pageSize { get; set; }
			public int totalPage { get; set; }
			public int totalCount { get; set; }
		}
	}
}
