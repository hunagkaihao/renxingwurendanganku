import { FormSchema } from '/@/components/Table';
import { BasicColumn } from '/@/components/Table';
import moment from 'moment';
import {
  // PagingUserListInput,
  // UsersServiceProxy,
  TaskHissServiceProxy,
  CellsServiceProxy,
  PagingTaskHisListInput,
  TaskHisDtoPagedResultDto,
  PagingTaskHisDetailInput,
  TaskHisDetailDtoPagedResultDto,
  CellDtoListResultDto,
  TaskType,
  TaskStatus,
} from '/@/services/ServiceProxies';
import { useI18n } from '/@/hooks/web/useI18n';
import { SelectItem } from '/@/utils/SelectItem';
const { t } = useI18n();

export const manageTypeCodeSelectItem: SelectItem[] = [
  {
    label: '物料入库',
    value: TaskType[TaskType.NPFullStockIn],
    key: TaskType.NPFullStockIn,
  },
  {
    label: '无计划出库',
    value: TaskType[TaskType.NpFullStockOut],
    key: TaskType.NpFullStockOut,
  },
  {
    label: '物料出库',
    value: TaskType[TaskType.NPSortStockOut],
    key: TaskType.NPSortStockOut,
  },
  {
    label: '借用出库',
    value: TaskType[TaskType.HPSortStockOut],
    key: TaskType.HPSortStockOut,
  },
  {
    label: '批量盘点',
    value: TaskType[TaskType.HpAnnualCheckDown],
    key: TaskType.HpAnnualCheckDown,
  },
  {
    label: '批量入库',
    value: TaskType[TaskType.HPBatchStockIn],
    key: TaskType.HPBatchStockIn,
  },
  {
    label: '盘盈入库',
    value: TaskType[TaskType.SurplusIn],
    key: TaskType.SurplusIn,
  },
];

export const manageStatusSelectItem: SelectItem[] = [
  {
    label: '等待执行',
    value: TaskStatus[TaskStatus.WaitingExecute],
    key: TaskStatus.WaitingExecute,
  },
  {
    label: '取消',
    value: TaskStatus[TaskStatus.Cancel],
    key: TaskStatus.Cancel,
  },
  {
    label: '完成',
    value: TaskStatus[TaskStatus.Complete],
    key: TaskStatus.Complete,
  },
  {
    label: '错误',
    value: TaskStatus[TaskStatus.Error],
    key: TaskStatus.Error,
  },
  {
    label: '已下达',
    value: TaskStatus[TaskStatus.OrderCatched],
    key: TaskStatus.OrderCatched,
  },
];

function getSelectLabel(options: SelectItem[], value: unknown): string {
  const item = options.find((option) => option.key == value || option.value == value);
  return item?.label ?? (value === null || value === undefined || value === '' ? '-' : String(value));
}

export const tableColumns: BasicColumn[] = [
  {
    title: t('routes.stockTask.stockTaskManagement_stockTaskBarcode'),
    dataIndex: 'materialBarcode',
  },
  {
    title: t('routes.stockTask.stockTaskManagement_manageTypeCode'),
    dataIndex: 'taskTypeCode',
    customRender: ({ text }) => {
      return getSelectLabel(manageTypeCodeSelectItem, text);
    },
  },
  {
    title: t('routes.stockTask.stockTaskManagement_manageStatus'),
    dataIndex: 'taskStatus',
    customRender: ({ text }) => {
      return getSelectLabel(manageStatusSelectItem, text);
    },
  },
  {
    title: t('routes.stockTask.stockTaskManagement_startCellCode'),
    dataIndex: 'startCellPosition',
  },
  {
    title: t('routes.stockTask.stockTaskManagement_endCellCode'),
    dataIndex: 'endCellPosition',
  },
  {
    title: t('routes.stockTask.stockTaskManagement_creationTime'),
    dataIndex: 'creationTime',
    customRender: ({ text }) => {
      return moment(text).format('YYYY-MM-DD HH:mm:ss');
    },
  },
];

export const tableDetailColumns: BasicColumn[] = [
  {
    title: t('物料标签'),
    dataIndex: 'goodsCode',
  },
  {
    title: t('物料题名'),
    dataIndex: 'goodsName',
  },
  // {
  //   title: t('档案'),
  //   dataIndex: 'goodsProperty1',
  // },

  
  // {
  //   title: t('routes.stockTask.stockTaskManagement_creationTime'),
  //   dataIndex: 'creationTime',
  //   customRender: ({ text }) => {
  //     return moment(text).format('YYYY-MM-DD HH:mm:ss');
  //   },
  // },
];

export const searchFormSchema: FormSchema[] = [
  {
    field: 'filter',
    label: t('routes.stockTask.stockTaskManagement_stockTaskBarcode'),
    component: 'Input',
    colProps: { span: 6 },
  },
  {
    field: 'time',
    component: 'RangePicker',
    label: '创建时间:',
    labelWidth: 80,
    colProps: { span: 6 },
    defaultValue: [moment().subtract(7, 'days'), moment().add(1, 'days')],
  },
  {
    field: 'manageTypeCode',
    component: 'Select',
    label: t('任务类型'),
    labelWidth: 130,
    defaultValue: 'All', //设置默认值
    //required: true,
    colProps: {
      span: 6,
    },
    componentProps: {
      //设置选项值
      options: [
        {
          label: '全部',
          value: 'All',
        },
        {
          label: '物料出库',
          value: 'CTUNpFullStockOut',
        },
        {
          label: '组盘入库',
          value: 'CTUNPFullStockIn',
        },
        {
          label: '空容器入库',
          value: 'CTUStockIn',
        },
        {
          label: '空容器出库',
          value: 'CTUStockOut',
        },
      ],
      //disabled: true,
    },
  },
  
  {
    field: 'taskStatus',
    component: 'Select',
    label: t('任务状态'),
    labelWidth: 130,
    defaultValue: 'All', //设置默认值
    required: true,
    colProps: {
      span: 6,
    },
    componentProps: {
      //设置选项值
      options: [
        {
          label: '全部',
          value: 'All',
        },
        {
          label: '已取消',
          value: 'Cancel',
        },
        {
          label: '执行中',
          value: 'Executing',
        },
        {
          label: '已完成',
          value: 'Complete',
          // key: 2,
        },
      ],
      //disabled: true,
    },
  },
];

/**
 * 分页列表
 * @param params
 * @returns
 */
export async function getTableListAsync(
  params: PagingTaskHisListInput
): Promise<TaskHisDtoPagedResultDto> {
  const _taskHissServiceProxy = new TaskHissServiceProxy();
  return _taskHissServiceProxy.page(params);
}

/**
 * 分页明细列表
 * @param params
 * @returns
 */
export async function getDetaiTableListAsync(
  params: PagingTaskHisDetailInput
): Promise<TaskHisDetailDtoPagedResultDto> {
  const _taskHissServiceProxy = new TaskHissServiceProxy();
  return _taskHissServiceProxy.pageDetail(params);
}

export async function getAllCellAsync(): Promise<CellDtoListResultDto> {
  const _cellServiceProxy = new CellsServiceProxy();
  return _cellServiceProxy.all();
}
