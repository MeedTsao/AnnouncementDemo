using AnnouncementDemo.Extensions;
using AnnouncementDemo.Models;
using AnnouncementDemo.Services;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Caching;

namespace AnnouncementDemo.Repository
{
    public class AnnoRepository : IAnnoServices
    {
        private readonly AppCache _cache = new AppCache();
        /// <summary>
        /// 查詢
        /// </summary>
        /// <param name="_configuration">DemoDB</param> 
        /// <param name="inModel">QueryIn</param>
        /// <returns></returns>
        public AnnoViewModel.QueryOut Query(IConfiguration _configuration, AnnoViewModel.QueryIn inModel)
        {
            AnnoViewModel.QueryOut outModel = new AnnoViewModel.QueryOut();
            outModel.Grid = new();
            List<AnnoViewModel.AnnoModel> query_List = new();
            List<AnnoViewModel.AnnoModel> result_List = new();
            // 資料庫連線字串
            string connStr = _configuration.GetConnectionString("DemoDB");

            using (var cn = new SqlConnection(connStr))
            {
                // 主要查詢 SQL
                string sql = @"SELECT Pkey, CONVERT(varchar(12) , AnnoDate, 111 ) as AnnoDate, AnnoSubject, AnnoContent, AnnoStatus, Case AnnoStatus when '1' then '顯示' when '0' then '隱藏' end As AnnoStatusName
						FROM Announcement 
						WHERE 1=1 ";

                if (!string.IsNullOrEmpty(inModel.AnnoSubject))
                {
                    sql += " AND AnnoSubject LIKE @AnnoSubject ";
                }
                if (!string.IsNullOrEmpty(inModel.AnnoStatus))
                {
                    sql += " AND AnnoStatus = @AnnoStatus ";
                }
                sql += " ORDER BY AnnoDate desc, AnnoStatus ";

                object param = new
                {
                    AnnoSubject = "%" + inModel.AnnoSubject + "%",
                    AnnoStatus = inModel.AnnoStatus
                };
                // 檢查Cache 是否存在
                switch (inModel.AnnoStatus)
                {
                    case "0":
                        if (_cache.IsSet("QueryHide") == false)
                        {
                            // 使用 Dapper 查詢
                            query_List = cn.Query<AnnoViewModel.AnnoModel>(sql, param).ToList();

                            // 設定新 Cache ， 60 分鐘後就回收快取
                            _cache.Set("QueryHide", query_List, "Absolute", 60);
                        }
                        else
                        {
                            // 取得 Cache 
                            query_List = (List<AnnoViewModel.AnnoModel>)_cache.Get("QueryHide");
                        }
                        break;
                    case "1":
                        if (_cache.IsSet("Query") == false)
                        {
                            // 使用 Dapper 查詢
                            query_List = cn.Query<AnnoViewModel.AnnoModel>(sql, param).ToList();

                            // 設定新 Cache ， 60 分鐘後就回收快取
                            _cache.Set("Query", query_List, "Absolute", 60);
                        }
                        else
                        {
                            // 取得 Cache 
                            query_List = (List<AnnoViewModel.AnnoModel>)_cache.Get("Query");
                        }
                        break;
                }
            }
            if (query_List.Count > 0)
            {
                // 分頁處理
                var itemCount = query_List.Count;
                var pageNumber = inModel.pagination.pageNo;
                var count = inModel.pagination.pageSize;
                var index = (pageNumber - 1) * count;
                var startIndex = pageNumber <= 0 ? 0 : index <= 0 ? 0 : index;
                var range = itemCount - startIndex >= count ? count : itemCount - startIndex;
                result_List.AddRange(query_List.GetRange(startIndex, range));
                // 輸出物件
                foreach (var item in result_List)
                {
                    outModel.Grid.Add(item);
                }
            }
            // 計算畫面分頁
            outModel.pagination = this.PreparePage(inModel.pagination, query_List.Count);
            return outModel;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="_configuration">DemoDB</param>
        /// <param name="inModel">AddSaveIn</param>
        /// <returns></returns>
        public AnnoViewModel.AddSaveOut Add(IConfiguration _configuration, AnnoViewModel.AddSaveIn inModel)
        {
            AnnoViewModel.AddSaveOut outModel = new();
            // 資料庫連線字串
            string connStr = _configuration.GetConnectionString("DemoDB");

            using (var conn = new SqlConnection(connStr))
            {

                string sql = @"INSERT INTO Announcement(AnnoDate, AnnoSubject, AnnoContent, AnnoStatus)
							VALUES (@AnnoDate, @AnnoSubject, @AnnoContent, @AnnoStatus)";
                var param = new
                {
                    AnnoDate = inModel.AnnoDate,
                    AnnoSubject = inModel.AnnoSubject,
                    AnnoContent = inModel.AnnoContent,
                    AnnoStatus = inModel.AnnoStatus
                };
                // 使用 Dapper
                conn.Execute(sql, param);
            }

            outModel.ResultMsg = "新增完成";
            // 移除 Cache 
            switch (inModel.AnnoStatus)
            {
                case "0":
                    _cache.Remove("QueryHide");
                    break;
                case "1":
                    _cache.Remove("Query");
                    break;
            }
            return outModel;
        }

        /// <summary>
        /// 編輯
        /// </summary>
        /// <param name="_configuration"></param>
        /// <param name="inModel">EditSaveIn</param>
        /// <returns></returns>
        public AnnoViewModel.EditSaveOut Edit(IConfiguration _configuration, AnnoViewModel.EditSaveIn inModel)
        {
            AnnoViewModel.EditSaveOut outModel = new();
            // 資料庫連線字串
            string connStr = _configuration.GetConnectionString("DemoDB");

            using (var conn = new SqlConnection(connStr))
            {
                string sql = @"UPDATE Announcement
						SET    AnnoDate = @AnnoDate, AnnoSubject = @AnnoSubject, AnnoContent = @AnnoContent, AnnoStatus = @AnnoStatus
						WHERE  Pkey = @Pkey";
                var param = new
                {
                    AnnoDate = inModel.AnnoDate,
                    AnnoSubject = inModel.AnnoSubject,
                    AnnoContent = inModel.AnnoContent,
                    AnnoStatus = inModel.AnnoStatus,
                    Pkey = inModel.Pkey
                };

                // 使用 Dapper
                int ret = conn.Execute(sql, param);
                if (ret > 0)
                {
                    outModel.ResultMsg = "修改完成";
                    // 移除 Cache 
                    switch (inModel.AnnoStatus)
                    {
                        case "0":
                            _cache.Remove("QueryHide");
                            break;
                        case "1":
                            _cache.Remove("Query");
                            break;
                    }
                }
                return outModel;
            }
        }

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="_configuration">DemoDB</param>
        /// <param name="inModel">DelCheckIn</param>
        /// <returns></returns>
        public AnnoViewModel.DelCheckOut Delete(IConfiguration _configuration, AnnoViewModel.DelCheckIn inModel)
        {
            AnnoViewModel.DelCheckOut outModel = new();
            // 資料庫連線字串
            string connStr = _configuration.GetConnectionString("DemoDB");

            using (var conn = new SqlConnection(connStr))
            {
                int ret = 0;
                foreach (AnnoViewModel.AnnoModel model in inModel.checks)
                {
                    string sql = @"DELETE Announcement WHERE  Pkey = @Pkey";
                    var param = new
                    {
                        Pkey = model.Pkey
                    };

                    // 使用 Dapper
                    ret += conn.Execute(sql, param);
                }

                if (ret > 0)
                {
                    outModel.ResultMsg = "成功刪除 " + ret + " 筆資料";
                    // 移除 Cache 
                    _cache.Remove("QueryHide");
                    _cache.Remove("Query");
                }
            }
            return outModel;
        }

        /// <summary>
        /// 計算分頁
        /// </summary>
        /// <param name="model"></param>
        /// <param name="TotalRowCount"></param>
        /// <returns></returns>
        public AnnoViewModel.PaginationModel PreparePage(AnnoViewModel.PaginationModel model, int TotalRowCount)
        {
            //只顯示5頁
            List<string> pages = new List<string>();
            int pageStart = ((model.pageNo - 1) / 5) * 5;
            model.totalCount = TotalRowCount;
            model.totalPage =
                    Convert.ToInt16(Math.Ceiling(
                     double.Parse(model.totalCount.ToString()) / double.Parse(model.pageSize.ToString())
                    ));

            if (model.pageNo > 5)
                pages.Add("<<");
            if (model.pageNo > 1)
                pages.Add("<");
            for (int i = 1; i <= 5; ++i)
            {
                if (pageStart + i > model.totalPage)
                    break;
                pages.Add((pageStart + i).ToString());
            }
            if (model.pageNo < model.totalPage)
                pages.Add(">");
            if ((pageStart + 5) < model.totalPage)
                pages.Add(">>");
            model.pages = pages;
            return model;
        }

    }
}
