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
using Newtonsoft.Json;

namespace DKP.Services.GM
{
    #region 接口
    public interface IMailService
    {
        void Add(Mail obj);
        void Update(Mail obj);
        void Delete(int id);
        Mail GetById(int id);
        IPagedList<Mail> getAll(NameValueCollection query, PageParam pageParam);
        JsonModel AgentList();
        JsonModel ServerList(string agentid);
        JsonModel TypeList();
        JsonModel ItemList(string typeid);

        JsonModel SendMail(int id);

    }
    #endregion
    public class MailService : IMailService
    {
        private readonly IRepository<Mail> _mailRepository;
        public MailService(IRepository<Mail> mailRepository)
        {
            _mailRepository = mailRepository;
        }

        public void Add(Mail obj)
        {
            _mailRepository.Insert(obj);
            _mailRepository.Save();
        }

        public void Update(Mail obj)
        {
            _mailRepository.Update(obj);
            _mailRepository.Save();
        }

        public void Delete(int id)
        {
            _mailRepository.Delete(id);
            _mailRepository.Save();
        }

        public Mail GetById(int id)
        {
            return _mailRepository.GetById(id);
        }

        public IPagedList<Mail> getAll(NameValueCollection query, PageParam pageParam)
        {
            var result = _mailRepository.TableNoTracking;

            var CharacterId = query["CharacterId"];
            var Start = query["Start"];
            var End = query["End"];

            if (CharacterId != null && CharacterId.ToString() != "")
            {
                var cid = CharacterId.ToString();
                result = result.Where(t => t.CharacterId == cid);
            }

            if (Start != null && Start.ToString() != "")
            {
                var startDate = DateTime.Parse(Start);
                result = result.Where(t => t.CreateTime >= startDate);
            }

            if (End != null && End.ToString() != "")
            {
                var endDate = DateTime.Parse(End);
                result = result.Where(t => t.CreateTime <= endDate);
            }

            result = result.OrderBy(pageParam.ToString());
            return new PagedList<Mail>(result,pageParam);
        }

        public JsonModel AgentList()
        {
            var model = CommonService.POST("/api/game_server/channel_list");
            var types = new List<Dictionary<string, object>>();
            var json = new JsonModel();
            if (model.code == 200)
            {
                try
                {
                    types = model.data;
                    json.total = types.Count();
                    json.rows = types;
                    json.status = true;
                }
                catch (Exception e)
                {
                    json.status = false;
                    json.msg = (e.InnerException != null ? e.InnerException.Message : e.Message);
                }

            }
            else
            {
                json.status = false;
                json.msg = model.msg;
            }
            return json;
        }

        public JsonModel TypeList()
        {
            var model = CommonService.POST("/api/reward/reward_type");
            var types = new List<Dictionary<string, object>>();
            var json = new JsonModel();
            if (model.code == 200)
            {
                try
                {
                    types = model.data;
                    json.total = types.Count();
                    json.rows = types;
                    json.status = true;
                }
                catch (Exception e)
                {
                    json.status = false;
                    json.msg = (e.InnerException != null ? e.InnerException.Message : e.Message);
                }

            }
            else
            {
                json.status = false;
                json.msg = model.msg;
            }
            return json;
        }
    
        public JsonModel ServerList(string agentid)
        {
            var model = CommonService.POST("/api/game_server/server_list", "channel=" + agentid);
            var types = new List<Dictionary<string, object>>();
            var json = new JsonModel();
            if (model.code == 200)
            {
                try
                {
                    types = model.data;
                    json.total = types.Count();
                    json.rows = types;
                    json.status = true;
                }
                catch (Exception e)
                {
                    json.status = false;
                    json.msg = (e.InnerException != null ? e.InnerException.Message : e.Message);
                }

            }
            else
            {
                json.status = false;
                json.msg = model.msg;
            }
            return json;
        }

        public JsonModel ItemList(string typeid)
        {
            var model = CommonService.POST("/api/reward/reward_config", "reward_type="+typeid);
            var types = new List<Dictionary<string, object>>();
            var json = new JsonModel();
            if (model.code == 200)
            {
                try
                {
                    types = model.data;
                    json.total = types.Count();
                    json.rows = types;
                    json.status = true;
                }
                catch (Exception e)
                {
                    json.status = false;
                    json.msg = (e.InnerException != null ? e.InnerException.Message : e.Message);
                }

            }
            else
            {
                json.status = false;
                json.msg = model.msg;
            }
            return json;
        }

        public JsonModel SendMail(int id)
        {
            var obj = _mailRepository.GetById(id);
            var json = new
            {
                user_id = obj.CharacterId,
                agent_id = obj.AgentId,
                server_id = obj.ServerId,
                mail_id = obj.Id,
                content = obj.Contents,
                title = obj.Title,
                reward_info_list = obj.MailItems.Select(d => new { reward_type = d.TypeId, id = d.ItemId, count = d.Count }).ToList()
            };
            var model = CommonService.POST("/api/mail/gmmail", "data=" + JsonConvert.SerializeObject(json));
            var types = new List<Dictionary<string, object>>();
            var jsonModel = new JsonModel();
            if (model.code == 200)
            {
                try
                {
                    jsonModel.status = true;
                    obj.Status = 1;
                    Update(obj);
                }
                catch (Exception e)
                {
                    jsonModel.status = false;
                    jsonModel.msg = (e.InnerException != null ? e.InnerException.Message : e.Message);
                }

            }
            else
            {
                jsonModel.status = false;
                jsonModel.msg = model.msg;
            }
            return jsonModel;
        }

    }
}
