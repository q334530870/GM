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

namespace DKP.Services.Security
{
    #region 接口
    public interface IDictionaryService
    {
        IPagedList<DictionaryGroup> GetAllDictionaryGroups(NameValueCollection form, PageParam pageParam);
        IPagedList<Dictionary> GetAllDictionarys(NameValueCollection form, PageParam pageParam);
        Dictionary<string, string> GetGroups();
        Dictionary<string, string> GetDictionaryByCode(string groupCode);
        void Create(Dictionary dic);
        void Update(Dictionary dic);
        string GetNameByCode(string groupCode,string code);
        DictionaryGroup GetById(int id);
        Dictionary GetDicById(int id);
        string GetDicNameByCode(string code);
        DictionaryGroup GetByCode(string code);
        void DeleteDictionary(int id);
        void Delete(int id);
    }
    #endregion
    public class DictionaryService : IDictionaryService
    {
        #region 注入
        private readonly IRepository<DictionaryGroup> _dictionaryGroupRepository;
        private readonly IRepository<Dictionary> _dictionaryRepository;
        public DictionaryService(
            IRepository<DictionaryGroup> dictionaryGroupRepository, 
            IRepository<Dictionary> dictionaryRepository)
        {
            _dictionaryGroupRepository = dictionaryGroupRepository;
            _dictionaryRepository = dictionaryRepository;
        }
        #endregion

        public IPagedList<Dictionary> GetAllDictionarys(NameValueCollection form, PageParam pageParam)
        {
            string order = pageParam.ToString();
            if (pageParam.sort == "id" && pageParam.order == "desc")
            {
                order = "DictionaryGroupId desc,id desc";
            }
            var query = _dictionaryRepository.Table.Where(form.ResolveToLinq()).OrderBy(order);
            return new PagedList<Dictionary>(query, pageParam);
        }

        public IPagedList<DictionaryGroup> GetAllDictionaryGroups(NameValueCollection form, PageParam pageParam)
        {
            var query = _dictionaryGroupRepository.Table.Where(form.ResolveToLinq()).OrderBy(pageParam.ToString());
            return new PagedList<DictionaryGroup>(query, pageParam);
        }

        public void Create(Dictionary dic)
        {
            _dictionaryRepository.Insert(dic);
            _dictionaryRepository.Save();
        }

        public Dictionary<string, string> GetGroups()
        {
            var group = _dictionaryGroupRepository.Table;
            if (group == null)
            {
                return new Dictionary<string, string>();
            }
            else
            {
                return group.ToDictionary(d => d.Id+"", d => d.Name);
            }
        }

        public Dictionary<string, string> GetDictionaryByCode(string groupCode)
        {
            var group = _dictionaryGroupRepository.Table.FirstOrDefault(g => g.Code == groupCode);
            if (group == null)
            {
                return new Dictionary<string, string>();
            }
            else
            {
                return group.Dictionaries.ToDictionary(d=>d.Code,d=>d.Name);
            }
        }

        public string GetNameByCode(string groupCode, string code)
        {
            var group = _dictionaryGroupRepository.Table.FirstOrDefault(g => g.Code == groupCode);
            if (group == null)
            {
                return "";
            }
            else
            {
                var dic = @group.Dictionaries.FirstOrDefault(g => g.Code == code);
                if (dic == null)
                {
                    return "";
                }
                else
                {
                    return dic.Name;
                }
            }
        }

        public Dictionary GetDicById(int id)
        {
            return _dictionaryRepository.GetById(id);
        }

        public string GetDicNameByCode(string code)
        {
            return _dictionaryRepository.Table.Where(d => d.Code == code).First().Name;
        }

        public DictionaryGroup GetById(int id)
        {
            return _dictionaryGroupRepository.GetById(id);
        }

        public DictionaryGroup GetByCode(string code)
        {
            return _dictionaryGroupRepository.Table.FirstOrDefault(t => t.Code == code);
        }

        public void DeleteDictionary(int id)
        {
            var dictionaries = _dictionaryRepository.GetById(id);
            _dictionaryRepository.Delete(dictionaries);
            _dictionaryRepository.Save();
        }

        public void Update(Dictionary dic)
        {
            _dictionaryRepository.Update(dic);
            _dictionaryRepository.Save();
        }

        public void Delete(int id)
        {
            var dic = _dictionaryRepository.Table.Where(t=>t.DictionaryGroupId == id);
            _dictionaryRepository.Delete(dic);
            _dictionaryGroupRepository.Delete(id);
            _dictionaryGroupRepository.Save();
        }

    }
}
