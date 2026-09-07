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
  ManageType,
  ManageStatus,
} from '/@/services/ServiceProxies';
import { useI18n } from '/@/hooks/web/useI18n';
import { SelectItem } from '/@/utils/SelectItem';
const { t } = useI18n();

export const manageTypeCodeSelectItem: SelectItem[] = [
  {
    label: '档案入库',
    value: ManageType[ManageType.NPFullStockIn],
    key: ManageType.NPFullStockIn,
  },
  {
    label: '无计划出库',
    value: ManageType[ManageType.NpFullStockOut],
    key: ManageType.NpFullStockOut,
  },
  {
    label: '档案出库',
    value: ManageType[ManageType.NPSortStockOut],
    key: ManageType.NPSortStockOut,
  },
  {
    label: '借阅出库',
    value: ManageType[ManageType.HPSortStockOut],
    key: ManageType.HPSortStockOut,
  },
  {
    label: '批量盘点',
    value: ManageType[ManageType.HpAnnualCheckDown],
    key: ManageType.HpAnnualCheckDown,
  },
  {
    label: '批量入库',
    value: ManageType[ManageType.HPBatchStockIn],
    key: ManageType.HPBatchStockIn,
  },
  {
    label: '盘盈入库',
    value: ManageType[ManageType.SurplusIn],
    key: ManageType.SurplusIn,
  },
];

export const manageStatusSelectItem: SelectItem[] = [
  {
    label: '等待执行',
    value: ManageStatus[ManageStatus.WaitingExecute],
    key: ManageStatus.WaitingExecute,
  },
  {
    label: '取消',
    value: ManageStatus[ManageStatus.Cancel],
    key: ManageStatus.Cancel,
  },
  {
    label: '完成',
    value: ManageStatus[ManageStatus.Complete],
    key: ManageStatus.Complete,
  },
  {
    label: '错误',
    value: ManageStatus[ManageStatus.Error],
    key: ManageStatus.Error,
  },
  {
    label: '已下达',
    value: ManageStatus[ManageStatus.OrderCatched],
    key: ManageStatus.OrderCatched,
  },
];

export const tableColumns: BasicColumn[] = [
  {
    title: t('routes.stockTask.stockTaskManagement_stockTaskBarcode'),
    dataIndex: 'stockBarcode',
  },
  {
    title: t('routes.stockTask.stockTaskManagement_manageTypeCode'),
    dataIndex: 'manageTypeCode',
    customRender: ({ text }) => {
      return manageTypeCodeSelectItem.filter((f) => f.key == text)[0].label;
    },
  },
  {
    title: t('routes.stockTask.stockTaskManagement_manageStatus'),
    dataIndex: 'manageStatus',
    customRender: ({ text }) => {
      return manageStatusSelectItem.filter((f) => f.key == text)[0].label;
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
    title: t('档案标签'),
    dataIndex: 'goodsCode',
  },
  {
    title: t('档案题名'),
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
    field: 'manageStatus',
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
