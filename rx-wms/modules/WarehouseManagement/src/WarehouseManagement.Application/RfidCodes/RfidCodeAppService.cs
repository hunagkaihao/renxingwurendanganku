using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Uow;
using WarehouseManagement.RfidCodes.Aggregates;
using WarehouseManagement.RfidCodes.Dto;

namespace WarehouseManagement.RfidCodes
{
    public class RfidCodeAppService : WarehouseManagementAppService, IRfidCodeAppService
    {
        //private readonly RfidCodeManager _rfidCodeManager;
        private readonly IRfidRepository _rfidRepository;
        public RfidCodeAppService(
            //RfidCodeManager rfidCodeManager,
            IRfidRepository rfidCodeRepository)
        {
            //_rfidCodeManager = rfidCodeManager;
            _rfidRepository = rfidCodeRepository;
        }
        public async Task<RfidCodeDto> CreateAsync(CreateRfidCodeDto input)
        {
            var rfidobj =await _rfidRepository.GetListAsync(x => x.RfidCode == input.RfidCode && x.RfidTypeCode == input.RfidTypeCode);
            if (rfidobj.Count != 0)
            {
                throw new UserFriendlyException("数据库中已存在标签" + input.RfidCode);
            }
            if (input.RfidTypeCode == 0)
            {
                throw new UserFriendlyException("未选择标签类型");
            }
            //var entity = base.ObjectMapper.Map<CreateRfidCodeDto, Rfid>(input);
            var entity = new Rfid(input.RfidCode, input.RfidTypeCode);
            var rifd = await _rfidRepository.InsertAsync(entity);
            return base.ObjectMapper.Map<Rfid, RfidCodeDto>(rifd);
        }

        /// <summary>
        /// EXCEL批量导入标签
        /// </summary>
        /// <param name="goodsBaseDtos"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        [UnitOfWork]
        public async Task CreateManyAsync(List<CreateRfidCodeDto> rfidDtos)
        {
            List<string> rfidCodes = rfidDtos.Select(d => d.RfidCode).ToList();
            var existingRfids = await _rfidRepository.GetListAsync(f => rfidCodes.Contains(f.RfidCode));
            if (existingRfids.Count > 0)
            {
                throw new UserFriendlyException(message: "创建标签失败，存在重复条码");
            }
            List<Rfid> rfids = new List<Rfid>();
            for (int i = 0; i < rfidDtos.Count; i++)
            {
                if (rfids.Any(f => f.RfidCode == rfidDtos[i].RfidCode))
                    throw new UserFriendlyException(message: "创建标签失败，导入数据中存在重复条码");
                var entity = new Rfid(rfidDtos[i].RfidCode, rfidDtos[i].RfidTypeCode);
                rfids.Add(entity);
            }
            await _rfidRepository.InsertManyAsync(rfids, true);
        }
        public async Task<PagedResultDto<RfidCodeDto>> PageAsync(PagingRfidListInput input)
        {
            var queryable = await _rfidRepository.GetQueryableAsync();

            //Prepare a query to join books and authors
            var query = from rfid in queryable
                        where rfid.RfidCode.Contains(input.Filter.IsNullOrEmpty() ? "" : input.Filter.Trim())
                        select new { rfid };

            //Paging
            query = query
                .OrderByDescending(f => f.rfid.Id)
                .Skip(input.SkipCount)
                .Take(1000);
            //.Take(input.MaxResultCount);

            //Execute the query and get a list
            var queryResult = await AsyncExecuter.ToListAsync(query);

            //Convert the query result to a list of BookDto objects
            var rfidDtos = queryResult.Select(x =>
            {
                var rfidDtos = ObjectMapper.Map<Rfid, RfidCodeDto>(x.rfid);

                return rfidDtos;
            }).Take(input.PageSize).ToList();

            var totalCount = queryResult.Count() + input.SkipCount;

            return new PagedResultDto<RfidCodeDto>(
                totalCount,
                rfidDtos
            );
        }
        public async Task DeleteAsync(CreateRfidCodeDto input)
        {
            //await _rfidCodeManager.DeleteAsync(input.Id);
            await _rfidRepository.DeleteAsync(input.Id);
        }
    }
}
