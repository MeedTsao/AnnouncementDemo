using AnnouncementDemo.Models;
using AnnouncementDemo.Repository;
using AnnouncementDemo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace AnnouncementDemo.Controllers
{

    public class AnnoController : Controller
    {
        public static readonly IAnnoServices _annoService = new AnnoRepository();
        private readonly IConfiguration _configuration;

        public AnnoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 查詢公告頁面
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 查詢公告
        /// </summary>
        /// <param name="inModel">參數</param>
        /// <returns></returns>
        public IActionResult Query(AnnoViewModel.QueryIn inModel)=>Json(_annoService.Query(_configuration, inModel));

        /// <summary>
        /// 新增公告
        /// </summary>
        /// <param name="inModel">參數</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        public IActionResult AddSave(AnnoViewModel.AddSaveIn inModel)
        {
            AnnoViewModel.AddSaveOut outModel = new ();
            // 檢查參數
            if (ModelState.IsValid == false)
            {
                outModel.ErrMsg = string.Join("\n", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                return Json(outModel);
            }

            return Json(_annoService.Add(_configuration, inModel));
        }

        /// <summary>
        /// 修改公告
        /// </summary>
        /// <param name="inModel">參數</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        public IActionResult EditSave(AnnoViewModel.EditSaveIn inModel)
        {
            AnnoViewModel.EditSaveOut outModel = new ();

            // 檢查參數
            if (ModelState.IsValid == false)
            {
                outModel.ErrMsg = string.Join("\n", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                return Json(outModel);
            }
            return Json(_annoService.Edit(_configuration, inModel));
        }

        /// <summary>
        /// 刪除公告
        /// </summary>
        /// <param name="inModel">參數</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        public IActionResult DelCheck(AnnoViewModel.DelCheckIn inModel)
        {
            AnnoViewModel.DelCheckOut outModel = new ();

            // 檢查參數
            if (inModel.checks.Count == 0)
            {
                outModel.ErrMsg = "缺少輸入資料";
                return Json(outModel);
            }
            return Json(_annoService.Delete(_configuration, inModel));
        }
    }
}
