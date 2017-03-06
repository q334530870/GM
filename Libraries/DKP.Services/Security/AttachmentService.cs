using DKP.Core;
using DKP.Core.Data;
using DKP.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace DKP.Services.Security
{
    #region 接口
    public interface IAttachmentService
    {
        IList<Attachment> GetAttachment(int foreignKey, int category, string name);

        IList<Attachment> GetAttachment(int foreignKey, int category);

        IList<Attachment> GetLesseeAttachment(int lesseeId, int category);

        Upload Upload(HttpPostedFileBase file, int category, string name);

        string UpdateForienKey(List<int> ids, int forienKey, bool isDelete = true);

        string UpdateForienKey(int id, int forienKey, bool isDelete = true);

        void DeleteByForienKey(int forienKey, int category, string name);

        Attachment GetById(int id);

        void DeleteById(int id);
    }
    #endregion
    public class AttachmentService : IAttachmentService
    {
        #region 注入

        private readonly IRepository<Attachment> _attachmentRepository;
        private readonly IWorkContext _workContext;

        public AttachmentService(
            IRepository<Attachment> attachmentRepository,
            IWorkContext workContext)
        {
            _attachmentRepository = attachmentRepository;
            _workContext = workContext;
        }

        #endregion

        public IList<Attachment> GetAttachment(int foreignKey, int category, string name)
        {
            var query = _attachmentRepository.Table.Where(a => a.Category == category && a.ForeignKey == foreignKey && a.Name == name);
            if (query.Count() > 0)
            {
                return query.OrderByDescending(t => t.Id).ToList();
            }
            else
            {
                return new List<Attachment>();
            }
        }

        public IList<Attachment> GetAttachment(int foreignKey, int category)
        {
            return _attachmentRepository.Table.Where(t => t.Category == category && t.ForeignKey == foreignKey).ToList();
        }

        public IList<Attachment> GetLesseeAttachment(int lesseeId, int category)
        {
            var query = _attachmentRepository.Table.Where(a => a.ForeignKey == lesseeId && a.Category == category);
            return query.OrderByDescending(t => t.Id).ToList();
        }

        public string UpdateForienKey(List<int> ids, int forienKey, bool isDelete = true)
        {
            bool save = false;
            string path = "";
            if (ids != null && ids.Any())
            {

                foreach (var id in ids)
                {
                    if (id > 0)
                    {
                        var attachment = _attachmentRepository.GetById(id);
                        path = attachment.FilePath;
                        if (attachment != null && attachment.ForeignKey == null)
                        {
                            if (isDelete)
                            {
                                var oldAttachments = _attachmentRepository.Table.Where(a => a.Category == attachment.Category && a.Name == attachment.Name && a.ForeignKey == forienKey && !ids.Contains(a.Id));
                                if (oldAttachments.Any())
                                {
                                    foreach (var att in oldAttachments)
                                    {
                                        var tempPath = att.FilePath;
                                        try { File.Delete(HttpContext.Current.Server.MapPath(tempPath)); }
                                        catch
                                        {
                                            // ignored
                                        }
                                    }
                                    _attachmentRepository.Delete(oldAttachments);
                                }
                            }
                            attachment.ForeignKey = forienKey;
                            _attachmentRepository.Update(attachment);
                            save = true;
                        }
                    }
                }
                if (save)
                {
                    _attachmentRepository.Save();
                }
            }
            return path;
        }


        public void DeleteByForienKey(int forienKey, int category, string name)
        {
            var attachments = _attachmentRepository.Table.Where(a => a.ForeignKey == forienKey && a.Category == category && a.Name == name);
            if (attachments.Any())
            {
                foreach (var att in attachments)
                {
                    var path = att.FilePath;
                    try
                    {
                        File.Delete(HttpContext.Current.Server.MapPath(path));
                    }
                    catch
                    {
                        // ignored
                    }
                }
                _attachmentRepository.Delete(attachments);
            }
            _attachmentRepository.Save();
        }

        public string UpdateForienKey(int id, int forienKey, bool isDelete = true)
        {
            return UpdateForienKey(new List<int>() { id }, forienKey, isDelete);
        }

        public Upload Upload(HttpPostedFileBase file, int category, string name)
        {
            var upload = new Upload();
            try
            {
                string folder = "/Uploads/";
                var date = DateTime.Now;
                folder += date.Year + "/" + date.Month + "/" + date.Day + "/";
                string saveName = System.Guid.NewGuid().ToString();
                string ext = Path.GetExtension(file.FileName);
                if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(folder)))
                {
                    Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(folder));
                }
                var path = folder + saveName + ext;
                file.SaveAs(System.Web.HttpContext.Current.Server.MapPath(path));

                var attachment = new Attachment();
                attachment.Name = name;
                attachment.Category = category;
                attachment.UploadDate = DateTime.Now;
                attachment.FileName = Path.GetFileNameWithoutExtension(file.FileName);
                attachment.FilePath = path;
                attachment.FileType = ext;
                attachment.FileSize = file.ContentLength / 1024;
                attachment.UserId = _workContext.CurrentUserId;
                _attachmentRepository.Insert(attachment);
                _attachmentRepository.Save();

                upload.attachmentId = attachment.Id;
                upload.path = path;
                return upload;
            }
            catch (Exception e)
            {
                upload.errorInfo = e.StackTrace;
                return upload;
            }
        }

        public Attachment GetById(int id)
        {
            return _attachmentRepository.GetById(id);
        }

        public void DeleteById(int id)
        {
            var att = _attachmentRepository.Table.Where(a => a.Id == id).FirstOrDefault();
            if (att != null)
            {
                var path = att.FilePath;
                try
                {
                    File.Delete(HttpContext.Current.Server.MapPath(path));
                }
                catch
                {
                    // ignored
                }
                _attachmentRepository.Delete(att);
                _attachmentRepository.Save();
            }
        }
    }

    public class Upload
    {
        public string path { get; set; }
        public int attachmentId { get; set; }
        public string errorInfo { get; set; }
    }

}
