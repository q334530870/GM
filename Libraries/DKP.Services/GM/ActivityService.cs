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
    public interface IActivityService
    {
        void Add(Activity obj);
        void Update(Activity obj, List<ActivityItem> mi, List<ActivityServer> smi);
        JsonModel Delete(int id);
        JsonModel DeleteInfo(int id);
        Activity GetById(int id);
        ActivityInfo GetInfoById(int id);
        bool IsRepeat(string name);
        IPagedList<Activity> getAll(NameValueCollection query, PageParam pageParam,string type,int activityInfoId);
        IPagedList<ActivityInfo> getAllInfo(NameValueCollection query, PageParam pageParam, string type);
        JsonModel AgentList();
        JsonModel ServerList(string agentid);
        JsonModel TypeList();
        JsonModel ItemList(string typeid);

        JsonModel SendActivity(int id);
    }
    #endregion
    public class ActivityService : IActivityService
    {
        private readonly IRepository<Activity> _activityRepository;
        private readonly IRepository<ActivityServer> _activityServerRepository;
        private readonly IRepository<ActivityItem> _activityItemRepository;
        private readonly IRepository<ActivityInfo> _activityInfoRepository;
        public ActivityService(IRepository<Activity> activityRepository, IRepository<ActivityServer> activityServerRepository, IRepository<ActivityItem> activityItemRepository, IRepository<ActivityInfo> activityInfoRepository)
        {
            _activityRepository = activityRepository;
            _activityServerRepository = activityServerRepository;
            _activityItemRepository = activityItemRepository;
            _activityInfoRepository = activityInfoRepository;
        }

        public bool IsRepeat(string name)
        {
            if(_activityInfoRepository.Table.Where(t=>t.Name == name).Any())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Add(Activity obj)
        {
            if (obj.ActivityInfoId == 0)
            {
                var info = new ActivityInfo();
                info.Name = obj.Name;
                info.Type = obj.Type;
                info.StartDate = obj.StartDate;
                info.EndDate = obj.EndDate;
                info.ActType = obj.ActType;
                info.Desc = obj.Desc;
                info.Status = obj.Status;
                _activityInfoRepository.Insert(info);
                obj.ActivityInfoId = info.Id;
            }
            else
            {
                var old = _activityInfoRepository.Table.Where(t => t.Id == obj.ActivityInfoId).FirstOrDefault();
                old.ActType = obj.ActType;
                old.StartDate = obj.StartDate;
                old.EndDate = obj.EndDate;
                old.Type = obj.Type;
                old.Name = obj.Name;
                old.Desc = obj.Desc;
                _activityInfoRepository.Update(old);
                var list = _activityRepository.Table.Where(d => d.ActivityInfoId == old.Id && d.Id != obj.Id);
                foreach (var a in list)
                {
                    a.ActType = obj.ActType;
                    a.StartDate = obj.StartDate;
                    a.EndDate = obj.EndDate;
                    a.Type = obj.Type;
                    a.Name = obj.Name;
                    a.Desc = obj.Desc;
                    _activityRepository.Update(a);
                }
            }
            _activityRepository.Insert(obj);
            _activityRepository.Save();
        }

        public void Update(Activity obj,List<ActivityItem> mi,List<ActivityServer> smi)
        {
            foreach (var m in mi)
            {
                m.ActivityId = obj.Id;
                _activityItemRepository.Insert(m);
            }
            foreach (var s in smi)
            {
                s.ActivityId = obj.Id;
                _activityServerRepository.Insert(s);
            }
            _activityItemRepository.Delete(_activityItemRepository.Table.Where(c => c.ActivityId == obj.Id));
            _activityServerRepository.Delete(_activityServerRepository.Table.Where(c => c.ActivityId == obj.Id));

            var info = _activityInfoRepository.Table.Single(t => t.Id == obj.ActivityInfoId);
            var list = _activityRepository.Table.Where(d => d.ActivityInfoId == info.Id && d.Id != obj.Id);
            info.ActType = obj.ActType;
            info.StartDate = obj.StartDate;
            info.EndDate = obj.EndDate;
            info.Type = obj.Type;
            info.Name = obj.Name;
            info.Desc = obj.Desc;
            _activityInfoRepository.Update(info);
            foreach(var a in list)
            {
                a.ActType = obj.ActType;
                a.StartDate = obj.StartDate;
                a.EndDate = obj.EndDate;
                a.Type = obj.Type;
                a.Name = obj.Name;
                a.Desc = obj.Desc;
                _activityRepository.Update(a);
            }

            _activityRepository.Update(obj);
            _activityRepository.Save();
        }

        public JsonModel DeleteInfo(int id)
        {
            var json = new { activityinfo_id = id };
            var model = CommonService.POST("/api/carnival/del", "data=" + JsonConvert.SerializeObject(json));
            var types = new List<Dictionary<string, object>>();
            var jsonModel = new JsonModel();
            if (model.code == 200)
            {
                try
                {
                    jsonModel.status = true;
                    _activityInfoRepository.Delete(id);
                    _activityInfoRepository.Save();
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

        public JsonModel Delete(int id)
        {
            var json = new { activity_id = id };
            var model = CommonService.POST("/api/carnival/del_sub_activity", "data=" + JsonConvert.SerializeObject(json));
            var types = new List<Dictionary<string, object>>();
            var jsonModel = new JsonModel();
            if (model.code == 200)
            {
                try
                {
                    jsonModel.status = true;
                    var activity = _activityRepository.GetById(id);
                    var infoId = activity.ActivityInfoId;
                    _activityRepository.Delete(id);
                    _activityRepository.Save();
                     if (!_activityRepository.Table.Where(a => a.ActivityInfoId == infoId).Any())
                    {
                        _activityInfoRepository.Delete(infoId);
                        _activityInfoRepository.Save();
                    }
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

        public Activity GetById(int id)
        {
            return _activityRepository.GetById(id);
        }

        public ActivityInfo GetInfoById(int id)
        {
            return _activityInfoRepository.GetById(id);
        }

        public IPagedList<Activity> getAll(NameValueCollection query, PageParam pageParam, string type, int activityInfoId)
        {
            var result = _activityRepository.TableNoTracking.Where(r=>r.ActType == type && r.ActivityInfoId == activityInfoId);

            result = result.OrderBy(pageParam.ToString());
            return new PagedList<Activity>(result,pageParam);
        }

        public IPagedList<ActivityInfo> getAllInfo(NameValueCollection query, PageParam pageParam, string type)
        {
            var result = _activityInfoRepository.TableNoTracking.Where(r => r.ActType == type);

            result = result.OrderBy(pageParam.ToString());
            return new PagedList<ActivityInfo>(result, pageParam);
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

        public JsonModel SendActivity(int id)
        {
            var info = _activityInfoRepository.GetById(id);
            var acts = _activityRepository.Table.Where(d => d.ActivityInfoId == id).ToList();
            var actJsons = new List<object>();
            foreach(var obj in acts)
            {
                var json = new
                {
                    activity_id = obj.Id,
                    type = obj.ActType,
                    type_code = obj.Type,
                    name = obj.Name,
                    start = obj.StartDate.ToString("yyyy-MM-dd hh:mm:ss"),
                    end = obj.EndDate.ToString("yyyy-MM-dd hh:mm:ss"),
                    reward_type = obj.JtypeId,
                    item_id = obj.JitemId,
                    count = obj.Jcount,
                    desc = obj.Desc,
                    servers = obj.ActivityServers.Select(d => new
                    {
                        agent_id = d.AgentId,
                        server_id = d.ServerId
                    }).ToList(),
                    reward_info_list = obj.ActivityItems.Select(d => new
                    {
                        reward_type = d.TypeId,
                        id = d.ItemId,
                        count = d.Count,
                        exchange_count =d.ExchangeCount,
                        amount = d.Amount,
                        start_rank = d.StartRank,
                        end_rank = d.EndRank,
                        discount = d.Discount,
                        price = d.Price,
                        number = d.Number,
                        desc =d.Desc

                    }).ToList()
                };
                actJsons.Add(json);
            }
            
            var infoJson = new
            {
                activityinfo_id = info.Id,
                type = info.ActType,
                type_code = info.Type,
                name = info.Name,
                start = info.StartDate.ToString("yyyy-MM-dd hh:mm:ss"),
                end = info.EndDate.ToString("yyyy-MM-dd hh:mm:ss"),
                desc = info.Desc,
                activitys = actJsons
            };
            
            var model = CommonService.POST("/api/carnival/sync", "data=" + JsonConvert.SerializeObject(infoJson));
            var types = new List<Dictionary<string, object>>();
            var jsonModel = new JsonModel();
            if (model.code == 200)
            {
                try
                {
                    jsonModel.status = true;
                    info.Status = 1;
                    _activityInfoRepository.Update(info);
                    foreach(var obj in acts)
                    {
                        obj.Status = 1;
                        _activityRepository.Update(obj);
                    }
                    _activityInfoRepository.Save();

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
