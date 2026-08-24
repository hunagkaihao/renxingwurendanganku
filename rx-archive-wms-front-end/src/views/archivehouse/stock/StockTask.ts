import { FormSchema } from '/@/components/Table';
import { BasicColumn } from '/@/components/Table';
import moment from 'moment';
import {
  // PagingUserListInput,
  // UsersServiceProxy,
  StockTasksServiceProxy,
  CellsServiceProxy,
  PagingStockTaskListInput,
  StockTaskDtoPagedResultDto,
  PagingStockTaskDetailInput,
  StockTaskDetailDtoPagedResultDto,
  CellDtoListResultDto,
  IdIntInput,
  ManageType,
  ManageStatus,
  WcsTasksServiceProxy,
  OpenDoorDto,
} from '/@/services/ServiceProxies';
import { message } from 'ant-design-vue';
import { useLoading } from '/@/components/Loading';

import { useI18n } from '/@/hooks/web/useI18n';
import { SelectItem } from '/@/utils/SelectItem';
const { t } = useI18n();
const [openFullLoading, closeFullLoading] = useLoading({
  tip: 'Loading...',
});

export const manageTypeCodeSelectItem: SelectItem[] = [
  {
    label: '档案入库',
    value: ManageType[ManageType.NPFullStockIn],
    key: ManageType.NPFullStockIn,
  },
  {
    label: '档案出库',
    value: ManageType[ManageType.NpFullStockOut],
    key: ManageType.NpFullStockOut,
  },
  {
    label: '借阅出库',
    value: ManageType[ManageType.HPSortStockOut],
    key: ManageType.HPSortStockOut,
  },
  {
    label: '盘点任务',
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
    label: 'All',
    value: 'All',
    key: 99,
  },
  {
    label: '等待执行',
    value: ManageStatus[ManageStatus.WaitingExecute],
    key: ManageStatus.WaitingExecute,
  },
  {
    label: '已下达',
    value: ManageStatus[ManageStatus.OrderCatched],
    key: ManageStatus.OrderCatched,
  },
  {
    label: '龙门抓取中',
    value: ManageStatus[ManageStatus.RobotPlace],
    key: ManageStatus.RobotPlace,
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
  {
    label: '等待确认',
    value: ManageStatus[ManageStatus.WaitingConfirm],
    key: ManageStatus.WaitingConfirm,
  },
  {
    label: '执行中',
    value: ManageStatus[ManageStatus.Executing],
    key: ManageStatus.Executing,
  },
];

export const tableColumns: BasicColumn[] = [
  {
    title: t('任务编号'),
    dataIndex: 'id',
  },
  {
    title: t('档案盒条码'),
    dataIndex: 'archiveBoxRfid',
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
    dataIndex: 'startCellCode',
  },
  {
    title: t('routes.stockTask.stockTaskManagement_endCellCode'),
    dataIndex: 'endCellCode',
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
    title: t('routes.stockTask.stockTaskManagement_stockTaskBarcode'),
    dataIndex: 'stockBarcode',
  },
  {
    title: t('routes.warehouse.goodsManagement_goodsCode'),
    dataIndex: 'goodsCode',
  },
  {
    title: t('routes.material.goodsManagement_name'),
    dataIndex: 'goodsName',
  },
  {
    title: t('routes.material.goodsManagement_goodsSpec'),
    dataIndex: 'goodsSpec',
  },
  {
    title: t('routes.material.goodsManagement_goodsBand'),
    dataIndex: 'goodsBand',
  },
  // {
  //   title: t('routes.material.goodsManagement_goodsBatchNo'),
  //   dataIndex: 'goodsBatchNo',
  // },
  {
    title: t('routes.stockTask.stockTaskManagement_quantity'),
    dataIndex: 'quantity',
  },
  // {
  //   title: t('routes.material.goodsManagement_goodsUnits'),
  //   dataIndex: 'goodsUnits',
  //   width: 50,
  // },
  {
    title: t('routes.stockTask.stockTaskManagement_creationTime'),
    dataIndex: 'creationTime',
    customRender: ({ text }) => {
      return moment(text).format('YYYY-MM-DD HH:mm:ss');
    },
  },
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
  // {
  //   field: 'manageType',
  //   component: 'Select',
  //   label: t('任务类型'),
  //   labelWidth: 130,
  //   defaultValue: 'All', 
  //   required: true,
  //   colProps: {
  //     span: 6,
  //   },
  //   componentProps: {

  //   options: manageStatusSelectItem,
  //   }
  // },
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
      options: manageStatusSelectItem,

    },
  },
];

export const createFormSchema: FormSchema[] = [
  {
    field: 'stockTaskBarcode',
    component: 'Input',
    label: t('routes.stockTask.stockTaskManagement_stockTaskBarcode'),
    labelWidth: 85,
    required: true,
    colProps: {
      span: 12,
    },
    componentProps: {
      autocomplete: 'off',
    },
  },
];

export const editFormSchema: FormSchema[] = [
  {
    field: 'stockTaskBarcode',
    component: 'Input',
    label: t('routes.stockTask.stockTaskManagement_stockTaskBarcode'),
    labelWidth: 85,
    required: true,
    colProps: {
      span: 12,
    },
    componentProps: {
      autocomplete: 'off',
    },
  },
];

export const editSCellFormSchema: FormSchema[] = [
  {
    field: 'stockBarcode',
    component: 'Input',
    label: t('routes.stockTask.stockTaskManagement_stockTaskBarcode'),
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
    field: 'startCellId',
    component: 'Select',
    label: t('routes.admin.bookManagement_type'),
    labelWidth: 130,
    // defaultValue: 'Undefined', //设置默认值
    required: true,
    colProps: {
      span: 12,
    },
    componentProps: {
      showSearch: true,
      optionFilterProp: 'label', //通过label进行查询
      //设置选项值
      // options:
      //disabled: true,
    },
  },
];

export const editECellFormSchema: FormSchema[] = [
  {
    field: 'stockBarcode',
    component: 'Input',
    label: t('routes.stockTask.stockTaskManagement_stockTaskBarcode'),
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
    field: 'EndCellId',
    component: 'Select',
    label: t('routes.admin.bookManagement_type'),
    labelWidth: 130,
    // defaultValue: 'Undefined', //设置默认值
    required: true,
    colProps: {
      span: 12,
    },
    componentProps: {
      showSearch: true,
      optionFilterProp: 'label', //通过label进行查询
      //设置选项值
      // options:
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
  params: PagingStockTaskListInput
): Promise<StockTaskDtoPagedResultDto> {
  const _stockTasksServiceProxy = new StockTasksServiceProxy();
  return _stockTasksServiceProxy.page(params);
}

/**
 * 分页明细列表
 * @param params
 * @returns
 */
export async function getDetaiTableListAsync(
  params: PagingStockTaskDetailInput
): Promise<StockTaskDetailDtoPagedResultDto> {
  const _stockTasksServiceProxy = new StockTasksServiceProxy();
  return _stockTasksServiceProxy.pageDetail(params);
}

export async function getAllCellAsync(): Promise<CellDtoListResultDto> {
  const _cellServiceProxy = new CellsServiceProxy();
  return _cellServiceProxy.all();
}

export async function taskCompletedAsync({ id, reload }) {
  try {
    const _stockTasksServiceProxy = new StockTasksServiceProxy();
    openFullLoading();
    const request = new IdIntInput();
    request.id = id;
    await _stockTasksServiceProxy.taskCompleted(request);
    closeFullLoading();
    message.success(t('common.operationSuccess'));
    reload();
  } catch (error) {
    closeFullLoading();
  }
}

export async function executeTaskAsync({ id, reload }) {
  try {
    const _stockTasksServiceProxy = new StockTasksServiceProxy();
    openFullLoading();
    const request = new IdIntInput();
    request.id = id;
    await _stockTasksServiceProxy.taskExecute(request);
    closeFullLoading();
    message.success(t('common.operationSuccess'));
    reload();
  } catch (error) {
    closeFullLoading();
  }
}
export async function cancelTaskAsync({ id, reload }) {
  try {
    const _stockTasksServiceProxy = new StockTasksServiceProxy();
    openFullLoading();
    const request = new IdIntInput();
    request.id = id;
    await _stockTasksServiceProxy.taskCancel(request);
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
export async function updateStockTaskAsync({
  request,
  changeOkLoading,
  validate,
  closeModal,
  resetFields,
}) {
  changeOkLoading(true);
  await validate();

  const _stockTasksServiceProxy = new StockTasksServiceProxy();
  await _stockTasksServiceProxy.update(request);
  changeOkLoading(false);
  resetFields();
  message.success(t('common.operationSuccess'));
  closeModal();
}

export async function SetStartCellAsync({
  request,
  changeOkLoading,
  validate,
  closeModal,
  resetFields,
}) {
  changeOkLoading(true);
  await validate();

  const _stockTasksServiceProxy = new StockTasksServiceProxy();
  await _stockTasksServiceProxy.updateSCell(request);
  changeOkLoading(false);
  resetFields();
  message.success(t('common.operationSuccess'));
  closeModal();
}

export async function SetEndCellAsync({
  request,
  changeOkLoading,
  validate,
  closeModal,
  resetFields,
}) {
  changeOkLoading(true);
  await validate();

  const _stockTasksServiceProxy = new StockTasksServiceProxy();
  await _stockTasksServiceProxy.updateECell(request);
  changeOkLoading(false);
  resetFields();
  message.success(t('common.operationSuccess'));
  closeModal();
}
//档案入库分配下达WCS
export async function wcsInSetCell({ id, reload }){
  try {
    const _stockTasksServiceProxy = new StockTasksServiceProxy();
    openFullLoading();
    await _stockTasksServiceProxy.wcsInSetCell(id);
    closeFullLoading();
    message.success(t('common.operationSuccess'));
    reload();
  } catch (error) {
    closeFullLoading();
  }
}
//手动下达开门指令
export async function wcsOpenDoor({ id, reload }){
  try {
    const _wcsTasksServiceProxy = new WcsTasksServiceProxy();
    openFullLoading();
    const param = new OpenDoorDto()
    param.orderCode = id.toString();
    await _wcsTasksServiceProxy.openDoorForOrder(param);
    closeFullLoading();
    message.success(t('common.operationSuccess'));
    reload();
  } catch (error) {
    closeFullLoading();
  }
}