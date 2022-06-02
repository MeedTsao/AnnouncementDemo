using AnnouncementDemo.Models;
using Microsoft.Extensions.Configuration;

namespace AnnouncementDemo.Services
{
    public interface IAnnoServices
    {
        public AnnoViewModel.QueryOut Query(IConfiguration _configuration, AnnoViewModel.QueryIn inModel);
        public AnnoViewModel.AddSaveOut Add(IConfiguration _configuration, AnnoViewModel.AddSaveIn inModel);
        public AnnoViewModel.EditSaveOut Edit(IConfiguration _configuration, AnnoViewModel.EditSaveIn inModel);
        public AnnoViewModel.DelCheckOut Delete(IConfiguration _configuration, AnnoViewModel.DelCheckIn inModel);
    }
}
