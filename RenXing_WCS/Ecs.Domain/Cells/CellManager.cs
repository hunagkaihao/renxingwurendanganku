using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ecs.Cells.Models;
using Ecs.DahSpecss;
using Ecs.LogTool;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Ecs.Cells;

public class CellManager : ISingletonDependency
{
    private ICellRepository _cellRepository;
    private IDahSpecsRepository _dahSpecRepository;
    private ILogger<CellManager> _logger;

    public CellManager(
        ICellRepository cellRepository,
        IDahSpecsRepository dahSpecRepository,
        ILogger<CellManager> logger)
    {
        _cellRepository = cellRepository;
        _dahSpecRepository = dahSpecRepository;
        _logger = logger;
    }

    public async Task<List<DispatchCell>> CreateCells(
        int rowNo,
        int colCnt,
        int startLayerNo,
        int layerCnt,
        int rowNoForPlc,
        bool layerConsistence,
        bool sectConsistence,
        bool colConsistence,
        int wareHouseId,
        string specsCode,
        List<int> colCntOfEverySection,
        string relativeNode = "15001")
    {
        try
        {
            var result = await _cellRepository.GetListAsync(
                o => o.Row == rowNo &&
                o.Layer >= startLayerNo &&
                o.Layer < startLayerNo + layerCnt).ConfigureAwait(false);
            if (result.Count > 0)
                throw new Exception($"排为{rowNo}，起始层为{startLayerNo}，层数为{layerCnt}的仓位已经存在");

            Check.Positive(rowNo, nameof(rowNo));
            Check.Positive(colCnt, nameof(colCnt));
            Check.Range(startLayerNo, nameof(startLayerNo), 1);
            Check.Positive(layerCnt, nameof(layerCnt));
            Check.NotNullOrEmpty(specsCode, nameof(specsCode));
            Check.Positive(rowNoForPlc, nameof(rowNoForPlc));

            if (colCntOfEverySection.Count == 0)
                throw new Exception("节的数量不能为0");

            foreach (int colCntInSect in colCntOfEverySection)
            {
                if (colCntInSect <= 0)
                    throw new Exception("存在节内的列数小于0的节");
            }

            int temp = 0;
            foreach (int colCount in colCntOfEverySection)
            {
                temp += colCount;
            }
            if (colCnt != temp)
                throw new Exception($"各节列数和{temp}与总列数{colCnt}不一致");

            var specs = await _dahSpecRepository.FindBySpecsCodeAsync(specsCode).ConfigureAwait(false);
            if (specs == null)
                throw new Exception($"规格号为{specsCode}的规格不存在");

            int[] sectNoArray = new int[colCnt];
            int[] colNoInSectArray = new int[colCnt];

            int startIndex = 0;
            if (sectConsistence)
            {
                for (int i = 0; i < colCntOfEverySection.Count; i++)
                {
                    if (colConsistence)
                    {
                        for (int j = 0; j < colCntOfEverySection[i]; j++)
                        {
                            sectNoArray[startIndex + j] = i + 1;
                            colNoInSectArray[startIndex + j] = j + 1;
                        }
                    }
                    else
                    {
                        for (int j = colCntOfEverySection[i] - 1; j >= 0; j--)
                        {
                            sectNoArray[startIndex + j] = i + 1;
                            colNoInSectArray[startIndex + j] = j + 1;
                        }
                    }

                    startIndex = startIndex + colCntOfEverySection[i];
                }
            }
            else
            {
                for (int i = colCntOfEverySection.Count - 1; i >= 0; i--)
                {
                    if (colConsistence)
                    {
                        for (int j = 0; j < colCntOfEverySection[i]; j++)
                        {
                            sectNoArray[startIndex + j] = i + 1;
                            colNoInSectArray[startIndex + j] = j + 1;
                        }
                    }
                    else
                    {
                        for (int j = colCntOfEverySection[i] - 1; j >= 0; j--)
                        {
                            sectNoArray[startIndex + j] = i + 1;
                            colNoInSectArray[startIndex + j] = j + 1;
                        }
                    }

                    startIndex = startIndex + colCntOfEverySection[i];
                }
            }

            List<DispatchCell> cells = new List<DispatchCell>();
            for (int j = 1; j <= colCnt; j++)
            {
                for (int k = startLayerNo; k < startLayerNo + layerCnt; k++)
                {
                    DispatchCell cell = new DispatchCell(
                        wareHouseId,
                        rowNo,
                        j,
                        k,
                        rowNoForPlc,
                        layerConsistence ? k : 6 - k,
                        sectNoArray[j - 1],
                        colNoInSectArray[j - 1],
                        specsCode,
                        relativeNode);
                    cells.Add(cell);
                }
            }

            return cells;
        }
        catch (Exception e)
        {
            _logger.Error(e.Message);
            return null;
        }
    }

    /// <summary>
    /// 根据wms的排查询每一层的列数
    /// </summary>
    /// <param name="row"></param>
    /// <returns>列的数量，如果查询失败，返回-1</returns>
    public async Task<int> GetColCntOfOneLayerInRow(int row)
    {
        try
        {

            var result = await _cellRepository.GetListAsync(o => o.Row == row && o.Layer == 1).ConfigureAwait(false);
            return result.Count();
        }
        catch (Exception e)
        {
            _logger.Error(e.Message);
            return -1;
        }
    }
}