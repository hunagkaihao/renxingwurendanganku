export type CellStockAction = 'in' | 'out';

export interface CellStockRecord {
  id: number;
  cellCode: string;
  cellType?: string;
  cellModel?: string;
  materialCode?: string;
}

export function validateCellStockAction(
  cell: CellStockRecord | undefined,
  action: CellStockAction
): string | undefined {
  if (!cell) {
    return '请先选择库位';
  }

  const hasMaterial = Boolean(cell.materialCode && cell.materialCode.trim());
  if (action === 'in' && hasMaterial) {
    return '该库位已有物料，无法入库';
  }
  if (action === 'out' && !hasMaterial) {
    return '该库位无物料，无法出库';
  }

  return undefined;
}

export function buildCellInboundDefaults(cell: CellStockRecord) {
  return {
    endCellId: cell.id,
    endCellCode: cell.cellCode,
    materialType: cell.cellModel,
  };
}

export function buildCellOutboundInput(cell: CellStockRecord) {
  return {
    cellCode: cell.cellCode,
    materialCode: cell.materialCode?.trim(),
  };
}
