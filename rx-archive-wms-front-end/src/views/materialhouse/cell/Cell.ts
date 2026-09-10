import { FormSchema } from '/@/components/Table';
import { BasicColumn } from '/@/components/Table';
import moment from 'moment';
import {
  // PagingUserListInput,
  // UsersServiceProxy,
  CellsServiceProxy,
  PagingCellListInput,
  CellDtoPagedResultDto,
  CellDtoListResultDto,
  IdIntInput,
  WarehousesServiceProxy,
  PagingWarehouseListInput,
  WarehouseDtoPagedResultDto,
  UpdateCellDto,
} from '/@/services/ServiceProxies';
import { message } from 'ant-design-vue';
import { useLoading } from '/@/components/Loading';
import { useI18n } from '/@/hooks/web/useI18n';
import { SelectItem } from '/@/utils/SelectItem';
import{ useUserStore } from '/@/store/modules/user'
import { reactive } from 'vue';

const { t } = useI18n();
const cellStore = useUserStore()
const option = reactive([
  {
    value: 1,
    label: '树脂留样间',
  }
]);

const [openFullLoading, closeFullLoading] = useLoading({
  tip: 'Loading...',
});

export const cellTypeSelectItem: SelectItem[] = [
  {
    label: '库位',
    value: 'Cell',
    key: 0,
  },
  {
    label: '柜门',
    value: 'Station',
    key: 1,
  }
];
export const cellModelSelectItem : SelectItem[] = [
  {
    label: '树脂颗粒',
    value: 'LL',
    key: 0,
  },
  {
    label: '薄膜',
    value: 'ML',
    key: 1,
  }
]
export const cellStatusSelectItem: SelectItem[] = [
  {
    label: '满货',
    value: 'Full',
    key: 0,
  },
  {
    label: '有货',
    value: 'Have',
    key: 1,
  },
  {
    label: '无货',
    value: 'Nohave',
    key: 2,
  },
  {
    label: '空容器',
    value: 'Pallet',
    key: 3,
  },
];
export const runStatusSelectItem: SelectItem[] = [
  {
    label: '禁用',
    value: 'Disable',
    key: 0,
  },
  {
    label: '可用',
    value: 'Enable',
    key: 1,
  },
  {
    label: '运行',
    value: 'Run',
    key: 2,
  },
  {
    label: '选定',
    value: 'Selected',
    key: 3,
  },
];
export const warehouseSelectItem: SelectItem[] = [
  {
    label: '树脂留样间',
    value: "1",
    key: 1,
  }
]

export const WareColumns: BasicColumn[] = [
  {
    title: t('仓库编号'),
    dataIndex: 'warehouseCode',
  },
  {
    title: t('仓库名称'),
    dataIndex: 'warehouseName',
  },
  {
    title: t('仓库类型'),
    dataIndex: 'warehouseType',
  },
  {
    title: t('创建时间'),
    dataIndex: 'creationTime',
    customRender: ({ text }) => {
      return moment(text).format('YYYY-MM-DD HH:mm:ss');
    },
  },
]
export const tableColumns: BasicColumn[] = [
  {
    title: t('routes.warehouse.cellManagement_cellCode'),
    dataIndex: 'cellCode',
  },
  {
    title: t('routes.warehouse.cellManagement_name'),
    dataIndex: 'cellName',
  },
  {
    title: t('物料码'),
    dataIndex: 'materialCode',
    customRender: ({ text }) => (text && String(text).trim() ? text : '-'),
  },
  {
    title: t('库位规格'),
    dataIndex: 'cellModel',
    customRender: ({ text }) => {
      const item = cellModelSelectItem.find((f) => f.value == text);
      return item ? item.label : text;  // 找不到时直接显示原始值
    },
  },
  {
    title: t('routes.warehouse.cellManagement_cellType'),
    dataIndex: 'cellType',
    customRender: ({ text }) => {
      return cellTypeSelectItem.filter((f) => f.value == text)[0].label;
    },
  },
  {
    title: t('routes.warehouse.cellManagement_x'),
    dataIndex: 'cell_x',
    width: 50,
  },
  {
    title: t('routes.warehouse.cellManagement_y'),
    dataIndex: 'cell_y',
    width: 50,
  },
  {
    title: t('routes.warehouse.cellManagement_z'),
    dataIndex: 'cell_z',
    width: 50,
  },
  {
    title: t('库位状态'),
    dataIndex: 'cellStatus',
    customRender: ({ text }) => {
      return cellStatusSelectItem.filter((f) => f.value == text)[0].label;
    },
  },
  {
    title: t('运行状态'),
    dataIndex: 'runStatus',
    customRender: ({ text }) => {
      return runStatusSelectItem.filter((f) => f.value == text)[0].label;
    },
  },
  {
    title: t('routes.warehouse.cellManagement_createTime'),
    dataIndex: 'creationTime',
    customRender: ({ text }) => {
      return moment(text).format('YYYY-MM-DD HH:mm:ss');
    },
  },
];
export const WaresearchFormSchema: FormSchema[] = [
  {
    field: 'filter',
    label: t('仓库编码'),
    component: 'Input',
    colProps: { span: 8 },
  },
];
export const searchFormSchema: FormSchema[] = reactive([
  {
    field: 'filter',
    label: t('库位编码:'),
    component: 'Input',
    colProps: { span: 8 },
  },
  {
    field: 'materialCode',
    label: t('物料码'),
    component: 'Input',
    colProps: { span: 8 },
  },
  {
    field: 'Warehouseld',
    label: t('所属仓库:'),
    component: 'Select',
    defaultValue: cellStore.getCell,
    colProps: { span: 8 },
    componentProps:{
      options:warehouseSelectItem
    }
  },
]);
export const WareFormSchema: FormSchema[] = [
    {
        field: 'warehouseCode',
        component: 'Input',
        label: t('仓库编号'),
        labelWidth: 85,
        required: true,
        colProps: {
          span: 12,
        },
    },    
    {
      field: 'warehouseName',
      component: 'Input',
      label: t('仓库名称'),
      labelWidth: 85,
      required: true,
      //defaultValue: 'Cell',
      colProps: {
        span: 12,
      },  
    },
    {
      field: 'warehouseType',
      component: 'Input',
      label: t('仓库类型'),
      labelWidth: 85,
      required: true,
      defaultValue: 'CTU',
      colProps: {
        span: 12,
      },
      componentProps: {
        //autocomplete: 'off',
        disabled: true,
      },
  }, 
  
]
  
export const createFormSchema: FormSchema[] = reactive([
  {
    field: 'cellName',
    component: 'Input',
    label: t('库位名称'),
    labelWidth: 85,
    required: true,
    colProps: {
      span: 12,
    },
  },
  {
    field: 'cellType',
    component: 'Select',
    label: t('routes.warehouse.cellManagement_cellType'),
    labelWidth: 85,
    required: true,
    defaultValue: 'Cell',
    colProps: {
      span: 12,
    },
    componentProps: {
      options:cellTypeSelectItem,
    }
  },
  {
    field: 'cellCode',
    component: 'Input',
    label: t('routes.warehouse.cellManagement_cellCode'),
    labelWidth: 85,
    required: true,
    colProps: {
      span: 12,
    },
    componentProps: {
      autocomplete: 'off',
    },
  },
  {
    field: 'cellModel',
    component: 'Select',
    label: t('库位规格'),
    labelWidth: 85,
    required: true,
    defaultValue: 'LL',
    colProps: {
      span: 12,
    },
    componentProps: {
      options:cellModelSelectItem,
    }
  },
  {
    field: 'warehouseId',
    component: 'Select',
    label: t('所属仓库'),
    labelWidth: 85,
    defaultValue: '1',
    colProps: {
      span: 12,
    },
    componentProps: {
      options:warehouseSelectItem
    },
  },
]);
export const createCellBatFormSchema: FormSchema[] =[
  {
    field: 'cell_x',
    component: 'Input',
    label: t('排数'),
    labelWidth: 85,
    required: true,
    colProps: {
      span: 12,
    },
    componentProps: {
      autocomplete: 'off',
    },
  },
  {
    field: 'cell_y',
    component: 'Input',
    label: t('列数'),
    labelWidth: 85,
    required: true,
    colProps: {
      span: 12,
    },
    componentProps: {
      autocomplete: 'off',
    },
  },
  {
    field: 'cell_z',
    component: 'Input',
    label: t('层数'),
    labelWidth: 85,
    required: true,
    colProps: {
      span: 12,
    },
    componentProps: {
      autocomplete: 'off',
    },
  }
]
export const editFormSchema: FormSchema[] = [
  {
    field: 'cellName',
    component: 'Input',
    label: t('库位名称'),
    labelWidth: 85,
    required: true,
    colProps: {
      span: 12,
    },
    componentProps: {
      autocomplete: 'off',
    },
  },
  {
    field: 'cellCode',
    component: 'Input',
    label: t('routes.warehouse.cellManagement_cellCode'),
    labelWidth: 85,
    required: true,
    colProps: {
      span: 12,
    },
    componentProps: {
      autocomplete: 'off',
    },
  },
  {
    field: 'cellType',
    component: 'Select',
    label: t('routes.warehouse.cellManagement_cellType'),
    labelWidth: 85,
    required: true,
    colProps: {
      span: 12,
    },
    componentProps: {
      options:cellTypeSelectItem,
    },
  },
  {
    field: 'cellModel',
    component: 'Select',
    label: t('库位规格'),
    labelWidth: 85,
    required: true,
    defaultValue: 'LL',
    colProps: {
      span: 12,
    },
    componentProps: {
      options:cellModelSelectItem,
    }
  },
];

// export const cellChartFormSchema: FormSchema[] = [
//   {
//     field: 'rowcn',
//     component: 'Select',
//     label: t('选择货架') + ':',
//     labelWidth: 85,
//     colProps: {
//       span: 12,
//     },
//     componentProps: {
//       options: [],
//     },
//   },
//   {
//     field: 'cellCode',
//     component: 'Input',
//     label: t('库位') + ':',
//     labelWidth: 85,
//     colProps: {
//       span: 12,
//     },
//     componentProps: {
//       autocomplete: 'off',
//       disabled: true,
//     },
//   },
// ];

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


export async function getTableListAsync(
  params: PagingCellListInput
): Promise<CellDtoPagedResultDto> {
  //params.warehouseId = cellStore.getWareIdByName(params.warehouseId)
    option.length = 0   
    for (let index = 0; index < cellStore.getWare.length; index++) {
      const b = ({value:0,label:''}) ;
      b.value = cellStore.getWare[index].wareid 
      b.label = cellStore.getWare[index].warename 
      option.push(b) 
    }
    console.log(option)
  const _cellsServiceProxy = new CellsServiceProxy();
  return _cellsServiceProxy.page(params);
}

export async function getTableListByZAsync(
  params: PagingCellListInput
): Promise<CellDtoListResultDto> {
  const _cellsServiceProxy = new CellsServiceProxy();
  return _cellsServiceProxy.getCellsByZ(params);
}

/**
 * 创建库位
 * @param param0
 */
export async function createCellAsync({
  request,
  changeOkLoading,
  validate,
  closeModal,
  resetFields,
}) {
  changeOkLoading(true);
  await validate();
  const _cellsServiceProxy = new CellsServiceProxy();
  await _cellsServiceProxy.create(request);
  changeOkLoading(false);
  message.success(t('common.operationSuccess'));
  resetFields();
  closeModal();
}
//批量创建cell
export async function createCellBatAsync({
  request,
  changeOkLoading,
  validate,
  closeModal,
  resetFields,
}) {
  changeOkLoading(true);
  await validate();
  const _cellsServiceProxy = new CellsServiceProxy();
  await _cellsServiceProxy.initCreateCell(request);
  changeOkLoading(false);
  message.success(t('common.operationSuccess'));
  resetFields();
  closeModal();
}
/**
 * 创建仓库
 * @param param0
 */
export async function createWareAsync({
  request,
  changeOkLoading,
  validate,
  closeModal,
  resetFields,
}) {
  changeOkLoading(true);
  await validate();
  const _WarehousesServiceProxy = new WarehousesServiceProxy();
  await _WarehousesServiceProxy.create(request);
  changeOkLoading(false);
  message.success(t('common.operationSuccess'));
  resetFields();
  closeModal();
}

/**
 * 删除用户
 * @param param0
 */
export async function deleteCellAsync({ id, reload }) {
  try {
    const _cellsServiceProxy = new CellsServiceProxy();
    openFullLoading();
    const request = new IdIntInput();
    request.id = id;
    await _cellsServiceProxy.delete(request);
    closeFullLoading();
    message.success(t('common.operationSuccess'));
    reload();
  } catch (error) {
    closeFullLoading();
  }
}

/**
 * 设置库位可用
 * @param param0
 */
export async function setCellEnable({ id, reload }) {
  try {
    const _cellsServiceProxy = new CellsServiceProxy();
    openFullLoading();
    const request = new UpdateCellDto();
    request.cellCode = id;
    await _cellsServiceProxy.setCellEnable(request);
    closeFullLoading();
    message.success(t('common.operationSuccess'));
    reload();
  } catch (error) {
    closeFullLoading();
  }
}

/**
 * 设置库位可用
 * @param param0
 */
export async function setCellDisable({ id, reload }) {
  try {
    const _cellsServiceProxy = new CellsServiceProxy();
    openFullLoading();
    const request = new UpdateCellDto();
    request.cellCode = id;
    await _cellsServiceProxy.setCellDisable(request);
    closeFullLoading();
    message.success(t('common.operationSuccess'));
    reload();
  } catch (error) {
    closeFullLoading();
  }
}

/**
 * 编辑用户
 * @param param0
 */
export async function updateCellAsync({
  request,
  changeOkLoading,
  validate,
  closeModal,
  resetFields,
}) {
  changeOkLoading(true);
  await validate();

  const _cellsServiceProxy = new CellsServiceProxy();
  await _cellsServiceProxy.update(request);
  changeOkLoading(false);
  resetFields();
  message.success(t('common.operationSuccess'));
  closeModal();
}
