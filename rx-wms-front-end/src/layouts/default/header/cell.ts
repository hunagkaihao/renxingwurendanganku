

import {

  WarehousesServiceProxy,
  PagingWarehouseListInput,
  WarehouseDtoPagedResultDto
} from '/@/services/ServiceProxies';


/**
 * 分页列表
 * @param params
 * @returns
 */
export async function getWareListAsync(
    params: PagingWarehouseListInput
  ): Promise<WarehouseDtoPagedResultDto> {
    const _cellsServiceProxy = new WarehousesServiceProxy();
    return _cellsServiceProxy.page(params);
  }