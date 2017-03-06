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
    public interface IMemberService
    {
        JsonModel Add(string account_id, string platform, string password);
        JsonModel Update(string account_id, string platform, string password);
        void Delete(int id);
        Mail GetById(int id);
        IPagedList<Mail> getAll(NameValueCollection query, PageParam pageParam);
        JsonModel AgentList();
        JsonModel ServerList(string agentid);
        JsonModel MemberList(NameValueCollection query,int page,int limit);
        JsonModel MemberList(string account_id,string platform);
        JsonModel ItemList(string typeid);
        JsonModel JY(string id, string status);
        JsonModel SH(string id, string status);

    }
    #endregion
    public class MemberService : IMemberService
    {
        private readonly IRepository<Mail> _mailRepository;
        public MemberService(IRepository<Mail> mailRepository)
        {
            _mailRepository = mailRepository;
        }

        public JsonModel Add(string account_id, string platform, string password)
        {
            var model = CommonService.POST("/api/user/add_user", "account_id="+account_id+"&platform="+platform+"&password="+password);
            var types = new List<Dictionary<string, object>>();
            var jsonModel = new JsonModel();
            if (model.code == 200)
            {
                try
                {
                    jsonModel.status = true;
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

        public JsonModel Update(string account_id, string platform, string password)
        {
            var model = CommonService.POST("/api/user/mod_user", "account_id=" + account_id + "&platform=" + platform + "&password=" + password);
            var types = new List<Dictionary<string, object>>();
            var jsonModel = new JsonModel();
            if (model.code == 200)
            {
                try
                {
                    jsonModel.status = true;
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

        public JsonModel MemberList(string account_id, string platform)
        {
            var model = CommonService.POST("/api/user/query_all_user", "account_id=" + account_id + "&platform="+platform);
            var types = new List<Dictionary<string, object>>();
            var json = new JsonModel();
            if (model.code == 200)
            {
                try
                {
                    types = model.data;
                    json.total = model.count == 0 ? types.Count() : model.count;
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

        public JsonModel MemberList(NameValueCollection query, int page, int limit)
        {
            var q = "";
            var account_id = query["account_id"];
            var user_id = query["account_id"];
            if (!string.IsNullOrEmpty(account_id))
            {
                q += "&account_id=" + account_id;
            }
            if (!string.IsNullOrEmpty(user_id))
            {
                q += "&user_id=" + user_id;
            }

            var model = CommonService.POST("/api/user/query_all_user","page="+page+"&limit="+limit+q);
            var types = new List<Dictionary<string, object>>();
            var json = new JsonModel();
            if (model.code == 200)
            {
                try
                {
                    types = model.data;
                    json.total = model.count == 0 ? types.Count():model.count;
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

        public JsonModel JY(string id,string status)
        {
            var account_id = id.Split(',')[0];
            var platform = id.Split(',')[1];
            var player_id = id.Split(',')[2];
            var server_id = id.Split(',')[3];
            var model = CommonService.POST("/api/user/mod_player_status", "account_id=" + account_id + "&platform=" + platform + "&server_id=" + server_id + "&status="+status+ "&player_id="+ player_id);
            var types = new List<Dictionary<string, object>>();
            var jsonModel = new JsonModel();
            if (model.code == 200)
            {
                try
                {
                    jsonModel.status = true;
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

        public JsonModel SH(string id, string status)
        {
            var account_id = id.Split(',')[0];
            var platform = id.Split(',')[1];
            var player_id = id.Split(',')[2];
            var server_id = id.Split(',')[3];
            var model = CommonService.POST("/api/user/mod_player_status", "account_id=" + account_id + "&platform=" + platform + "&server_id=" + server_id + "&status=" + status + "&player_id=" + player_id);
            var types = new List<Dictionary<string, object>>();
            var jsonModel = new JsonModel();
            if (model.code == 200)
            {
                try
                {
                    jsonModel.status = true;
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
