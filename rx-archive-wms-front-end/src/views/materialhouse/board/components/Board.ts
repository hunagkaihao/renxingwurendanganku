import {
    BoardsServiceProxy,

    SevenDayTasksDto,
    StockInfoDto,
  } from '/@/services/ServiceProxies';
  
  
  const _BoardsServiceProxy = new BoardsServiceProxy();

  export async function getSevenDayTasks(
    
  ): Promise<SevenDayTasksDto> {
    return await _BoardsServiceProxy.getSevenDayTasks();
  }

  export async function getStockInfo(

  ): Promise<StockInfoDto> {
    return await _BoardsServiceProxy.getStockInfo();
  }



  