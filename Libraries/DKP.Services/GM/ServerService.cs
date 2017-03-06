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

namespace DKP.Services.GM
{
    #region 接口
    public interface IServerService
    {
        JsonModel ServerList(int type = 1);
        JsonModel ServerAuthList();
    }
    #endregion
    public class ServerService : IServerService
    {
        public ServerService()
        {
        }

        public JsonModel ServerList(int type = 1)
        {
            var model = CommonService.POST("/api/server/info");
            var servers = new List<Dictionary<string,object>>();
            var json = new JsonModel();
            if (model.code == 200)
            {
                try
                {
                    var currentDate = DateTime.Now;
                    var data = model.data;
                    foreach (var ser in data)
                    {
                        if(type == 1)
                        {
                            if(ser["Type"].ToString() != "1")
                            {
                                continue;
                            }
                        }
                        servers.Add(new Dictionary<string, object>
                        {
                            { "HostName",ser["HostName"] },
                            { "Type",ser["Type"].ToString()=="1"?"游戏服务器":"日志服务器" },
                            { "IP",ser["IP"] },
                            { "GameSerPort",ser["Port"] },
                            { "User",ser["User"] },
                            { "IPadUsers",ser["IPadUsers"] },
                            { "IPhoneUsers",ser["IPhoneUsers"] },
                            { "FerrMem",ser["FreeMem"] },
                            { "UsedMem",ser["UsedMem"] },
                            { "TotalMem",ser["TotalMem"] },
                            { "CPU_Used",ser["CPU_Used"] },
                            { "RefreshDate",currentDate.ToString("yyyy-MM-dd HH:mm:ss") },
                            { "Kernal",ser["Version"] }
                        });
                    }
                    json.total = servers.Count();
                    json.rows = servers;
                    json.status = true;
                }
                catch(Exception e)
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

        public JsonModel ServerAuthList()
        {
            var model = CommonService.POST("/api/game_server/game_servers");
            var servers = new List<Dictionary<string, object>>();
            var json = new JsonModel();
            if (model.code == 200)
            {
                try
                {
                    var currentDate = DateTime.Now;
                    var data = model.data;
                    foreach (var ser in data)
                    {
                        var status = ser["status"].ToString();
                        servers.Add(new Dictionary<string, object>
                        {
                            { "id",ser["id"] },
                            { "ip",ser["ip"] },
                            { "name",ser["name"] },
                            { "status",ser["status"] },
                            { "color",status=="0"?"primary":(status == "1"?"warning":"danger")}
                        });
                    }
                    json.total = servers.Count();
                    json.rows = servers;
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

    }
}
