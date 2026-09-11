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
  TaskType,
  TaskStatus,
  WcsTasksServiceProxy,
  OpenDoorDto,
  CreateStockTaskDto,
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
    label: '物料入库',
    value: TaskType[TaskType.NPFullStockIn],
    key: TaskType.NPFullStockIn,
  },
  {
    label: '物料出库',
    value: TaskType[TaskType.NpFullStockOut],
    key: TaskType.NpFullStockOut,
  },
  {
    label: '借用出库',
    value: TaskType[TaskType.HPSortStockOut],
    key: TaskType.HPSortStockOut,
  },
  {
    label: '盘点任务',
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
    label: 'All',
    value: 'All',
    key: 99,
  },
  {
    label: '等待执行',
    value: TaskStatus[TaskStatus.WaitingExecute],
    key: TaskStatus.WaitingExecute,
  },
  {
    label: '已下达',
    value: TaskStatus[TaskStatus.OrderCatched],
    key: TaskStatus.OrderCatched,
  },
  {
    label: '龙门抓取中',
    value: TaskStatus[TaskStatus.RobotPlace],
    key: TaskStatus.RobotPlace,
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
  {
    label: '等待确认',
    value: TaskStatus[TaskStatus.WaitingConfirm],
    key: TaskStatus.WaitingConfirm,
  },
  {
    label: '执行中',
    value: TaskStatus[TaskStatus.Executing],
    key: TaskStatus.Executing,
  },
];

export const materialTypeSelectItem: SelectItem[] = [
  { label: '膜料', value: 'ML', key: 1 },
  { label: '粒料', value: 'LL', key: 0 },
];

function getSelectLabel(options: SelectItem[], value: unknown): string {
  const item = options.find((option) => option.key == value || option.value == value);
  return item?.label ?? (value === null || value === undefined || value === '' ? '-' : String(value));
}

export const tableColumns: BasicColumn[] = [
  {
    title: t('任务编号'),
    dataIndex: 'id',
  },
  {
    title: t('物料码'),
    dataIndex: 'materialBoxBarcode',
    width: 150,
    defaultHidden: false,
    customRender: ({ record }) => {
      return record.materialBoxBarcode || record.stockBarcode || '-';
    },
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
      options: manageStatusSelectItem,

    },
  },
];

export const createFormSchema: FormSchema[] = [
  {
    field: 'materialCode',
    component: 'Input',
    label: '物料条码',
    labelWidth: 100,
    required: true,
    colProps: {
      span: 12,
    },
    componentProps: {
      autocomplete: 'off',
    },
  },
  {
    field: 'materialName', component: 'Input', label: '物料名称', labelWidth: 100, required: true,
    colProps: { span: 12 }, componentProps: { autocomplete: 'off' },
  },
  {
    field: 'materialType', component: 'Select', label: '物料类型', labelWidth: 100, required: true,
    colProps: { span: 12 }, componentProps: { options: materialTypeSelectItem },
  },
  {
    field: 'materialUnit', component: 'Input', label: '单位', labelWidth: 100, required: true,
    colProps: { span: 12 }, componentProps: { autocomplete: 'off' },
  },
  {
    field: 'validityDays', component: 'InputNumber', label: '有效期（天）', labelWidth: 100, required: true,
    colProps: { span: 12 }, componentProps: { min: 0, precision: 0 },
  },
  {
    field: 'creatorUserCode', component: 'Input', label: '创建用户 ID', labelWidth: 100, required: true,
    colProps: { span: 12 }, componentProps: { autocomplete: 'off' },
  },
  {
    field: 'materialCreateTime', component: 'DatePicker', label: '创建时间', labelWidth: 100, required: true,
    colProps: { span: 12 }, componentProps: { showTime: true, valueFormat: 'YYYY-MM-DD HH:mm:ss', format: 'YYYY-MM-DD HH:mm:ss' },
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
  params.hideCompletedTasks = true;
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

export async function createWCSInAsync({ request, changeOkLoading, validate, closeModal, resetFields }) {
  changeOkLoading(true);
  await validate();
  await new StockTasksServiceProxy().createWCSIn(new CreateStockTaskDto(request));
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
