using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wcs.Cells;
using Wcs.Cells.Models;
using Wcs.LogTool;
using Microsoft.Extensions.Logging;
using Volo.Abp.Domain.Repositories;

namespace Wcs.Dispatch;

public class CellService : WcsAppService, ICellService
{
    private readonly CellManager _cellManager;
    private readonly ICellRepository _cellRepository;
    private ILogger<CellService> _logger;

    public CellService(
        CellManager cellManager, 
        ICellRepository cellRepository,
        ILogger<CellService> logger)
    {
        _cellManager = cellManager;
        _cellRepository = cellRepository;
        _logger = logger;
    }

    public async Task<ResponseDto> CellSeedsAsync(AddCellsDto cellsDto)
    {
        try
        {
            List<DispatchCell> cells = await _cellManager.CreateCells(
                cellsDto.rowNo, 
                cellsDto.colCnt, 
                cellsDto.startLayerNo,
                cellsDto.layerCnt, 
                cellsDto.rowNoForPlc,
                cellsDto.layerConsistence,
                cellsDto.sectConsistence,
                cellsDto.colConsistence,
                cellsDto.warehouseId, 
                cellsDto.cellSpecs,
                cellsDto.colCntInSect,
                cellsDto.relativeNode).ConfigureAwait(false);
            if(cells == null)
                return new ResponseDto(){ success = false, message = "创建失败" };
            foreach(var cell in cells)
            {
                await Task.Delay(1).ConfigureAwait(false);
                await _cellRepository.InsertAsync(cell).ConfigureAwait(false);
            }
            return new ResponseDto(){success = true, message = "创建成功"};
        }
        catch(Exception e)
        {
            _logger.Error(e.Message);
            return new ResponseDto(){ success = false, message = e.Message };
        }
    }

    public async Task<ResponseDto> CellsAllClearAsync()
    {
        try
        {
            await _cellRepository.DeleteAsync(o => o.Id > 0).ConfigureAwait(false);
            return new ResponseDto(){ success = true, message = "删除成功" };
        }
        catch(Exception e)
        {
            _logger.Error(e.Message);
            return new ResponseDto(){ success = false, message = e.Message };
        }
    }

}