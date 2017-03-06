using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Dynamic;
using DKP.Core;
using DKP.Core.Data;
using DKP.Core.Extensions;
using DKP.Core.Infrastructure;
using DKP.Core.Models;
using DKP.Data;
using DKP.Data.Models;
using DKP.Services.GM;

namespace DKP.Services.GM
{
    #region 接口
    public interface INoticeService
    {
        void Add(Notice notice);
        void Update(Notice notice);
        void Delete(int id);
        Notice GetById(int id);
        IPagedList<Notice> getAll(NameValueCollection query, PageParam pageParam);
    }
    #endregion
    public class NoticeService : INoticeService
    {
        private readonly IRepository<Notice> _noticeRepository;
        public NoticeService(IRepository<Notice> noticeRepository)
        {
            _noticeRepository = noticeRepository;
        }

        public void Add(Notice notice)
        {
            _noticeRepository.Insert(notice);
            _noticeRepository.Save();
        }

        public void Update(Notice notice)
        {
            _noticeRepository.Update(notice);
            _noticeRepository.Save();
        }

        public void Delete(int id)
        {
            _noticeRepository.Delete(id);
            _noticeRepository.Save();
        }

        public Notice GetById(int id)
        {
            return _noticeRepository.GetById(id);
        }

        public IPagedList<Notice> getAll(NameValueCollection query, PageParam pageParam)
        {
            var search = query["search"];
            var result = _noticeRepository.TableNoTracking;
            if (search != null && search.ToString() != "")
            {
                result = result.Where(t => t.Contents.Contains(search));
            }
            result = result.OrderBy(pageParam.ToString());
            return new PagedList<Notice>(result,pageParam);
        }

    }
}
